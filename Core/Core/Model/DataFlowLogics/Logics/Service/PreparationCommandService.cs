using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.CodeExecution.Repository;

namespace Core.Model.DataFlowLogics.Logics.Service
{
	/// <summary>
	/// Сервис подготовки команд к исполнению.
	/// </summary>
	public class PreparationCommandService : IPreparationCommandService
	{

		public Action<Command> OnPreparedCommand { get; set; }

		private readonly IDataCellRepository _dataCellRepository;

		private readonly IFunctionRepository _functionRepository;

		private readonly ConcurrentDictionary<string, string> _preparingCommands = new ConcurrentDictionary<string, string>();
		private readonly ConcurrentDictionary<string, string> _waitConditionCommands = new ConcurrentDictionary<string, string>();

		private readonly ConcurrentDictionary<string, List<string>> _dataLinksWithCommands = new ConcurrentDictionary<string, List<string>>();

		public PreparationCommandService(IDataCellRepository data_cell_repository, IFunctionRepository function_repository)
		{
			_dataCellRepository = data_cell_repository;
			_functionRepository = function_repository;
		}

		private void AddCommandToPreparing(Token command_token, IEnumerable<string> input_data_tokens)
		{
			if (_preparingCommands.TryAdd(command_token, null))
			{
				//throw new NotImplementedException("PreparationCommandService.PrepareCommand не удалось добавить.");
			}

			foreach (var input_data in input_data_tokens)
			{
				if (_dataLinksWithCommands.ContainsKey(input_data))
				{
					_dataLinksWithCommands[input_data].Add(command_token);
				}
				else
				{
					_dataLinksWithCommands[input_data] = new List<string> { command_token };
				}
			}
		}
		/*
		private DataCell GetOutputData(DataCellHeader data_cell_header)
		{
			return _dataCellRepository.Get(data_cell_header.Token).FirstOrDefault() ?? new DataCell
			{
				Header = data_cell_header,
				HasValue = null,
				Data = null
			};
		}*/

		/// <summary>
		/// Проверяет, что для команды либо нет условий, либо хотя бы одно условие уже получено.
		/// </summary>
		/// <param name="condition_data_tokens"></param>
		/// <param name="condition_data_cells">Условия.</param>
		/// <param name="command_token"></param>
		/// <returns>True, если нет условий или хотя бы одно условие уже получено.</returns>
		private bool CheckCommandCodition(Token command_token, IEnumerable<string> condition_data_tokens, out List<DataCell> condition_data_cells)
		{
			condition_data_cells = new List<DataCell>();
			var condition_data_cells_out = _dataCellRepository.Get(condition_data_tokens.ToArray()).ToList();
			if (condition_data_tokens.Any() && !condition_data_cells_out.Any(x => x?.HasValue != null && x.HasValue.Value))
			{
				if (_waitConditionCommands.TryAdd(command_token, null))
				{
					//throw new NotImplementedException("PreparationCommandService.CheckCommandCodition не удалось добавить.");
				}

				foreach (var input_dataa in condition_data_tokens)
				{
					if(!_dataLinksWithCommands.TryGetValue(input_dataa, out List<string> list))
					{
						list = new List<string>();
						_dataLinksWithCommands.TryAdd(input_dataa, list);
					}

					list.Add(command_token);
				}
				return false;
			}
			condition_data_cells.AddRange(condition_data_cells_out);
			return true;
		}

		/// <summary>
		/// Пытается созадть готовую к исполнению команду. Если команда не может быть исполнена на данный момент, то возвращает false. 
		/// </summary>
		/// <param name="command_token"></param>
		/// <param name="new_command">Созданная команда.</param>
		/// <returns>Удалось ли создать готовую к исполнению команду.</returns>
		private bool TryCreateCommand(Token command_token, out Command new_command)
		{
			new_command = null;
			var last_token_part = command_token.Last();
			var token_part = Token.Parse(last_token_part);
			var function = _functionRepository.Get(token_part.Name).FirstOrDefault();
			var prev_function_token = command_token.Prev();
			var prev_function_token_part = Token.Parse(command_token.Prev().Last());
			var prev_function = (ControlFunction)_functionRepository.Get(prev_function_token_part.Name).FirstOrDefault();

			/////////////

			string[] condition_data_tokens;

			if (prev_function == null)
			{
				condition_data_tokens = new string[0];
			}
			else
			{
				var commands = prev_function.Commands.ToList();

				var command_template = prev_function.Commands.ToArray()[token_part.Index.Value];
				var condition_ids = command_template.ConditionId;

				condition_data_tokens = new string[condition_ids.Count];

				for (var i = 0; i < condition_ids.Count; i++)
				{
					var id = condition_ids[i];
					if (id <= prev_function.InputDataCount)
					{
						condition_data_tokens[i] = command_token.Next($"InputData{i}");
					}
					else if (id >= prev_function.InputDataCount + prev_function.Commands.Count())
					{
						condition_data_tokens[i] = new Token(prev_function_token_part.Name).Next($"const_{id - (prev_function.InputDataCount + prev_function.Commands.Count())}");
					}
					else
					{
						var funcc = commands.First(x => x.OutputDataId == id);
						condition_data_tokens[i] = prev_function_token.Next($"{funcc.FunctionHeader.Token}<{commands.IndexOf(funcc)}>").Next("result");
					}
				}
			}

			/////////////

			// Если условия ещё не готовы, то возвращаем null.
			if (!CheckCommandCodition(command_token, condition_data_tokens, out List<DataCell> condition_data))
			{
				return false;
			}

			// Если нет ни одного истинного условия, то возвращаем null.
			if (condition_data.Any(x => x?.HasValue != null && x.HasValue.Value && !(bool)x.Data))
			{
				return false;
			}

			// Подготавливае места для ячеек с выходными данными.

			// TODO: сделать возможным получение ячейки данных по маске.
			var count = function.InputDataCount;
			string[] input_data_tokens;

			if (prev_function == null)
			{
				input_data_tokens = new string[count];
				for (int i = 0; i < count; i++)
				{
					input_data_tokens[i] = command_token.Next($"InputData{i}");
				}
			}
			else
			{
				var commands = prev_function.Commands.ToList();

				var command_template = commands[token_part.Index.Value];
				var input_ids = command_template.InputDataIds;

				count = input_ids.Count;
				input_data_tokens = new string[count];

				for (var i = 0; i < input_ids.Count; i++)
				{
					var id = input_ids[i];
					if (id <= prev_function.InputDataCount)
					{
						input_data_tokens[i] = command_token.Next($"InputData{i}");
					}
					else if(id >= prev_function.InputDataCount + prev_function.Commands.Count())
					{
						input_data_tokens[i] = new Token(prev_function_token_part.Name).Next($"const_{id - (prev_function.InputDataCount + prev_function.Commands.Count())}");
					}
					else
					{
						var funcc = commands.First(x => x.OutputDataId == id);
						input_data_tokens[i] = prev_function_token.Next($"{funcc.FunctionHeader.Token}<{commands.IndexOf(funcc)}>").Next("result");
					}
				}
			}
			
			var inputs = _dataCellRepository.Get(input_data_tokens).ToArray();

			var all_ready = true;

			switch (((FunctionHeader)function.Header).Condition)
			{
				case InputParamCondition.All:
					if (inputs.Any(x => x == null))
					{
						all_ready = false;
					}
					break;
				case InputParamCondition.Any:
					if (!inputs.Any(x => x?.HasValue != null && x.HasValue.Value))
					{
						all_ready = false;
					}
					break;
				default:
					throw new Exception($"CreateNewCommand Неизвестный тип: {((FunctionHeader)function.Header).Condition}");
			}

			if (!all_ready)
			{
				AddCommandToPreparing(command_token, input_data_tokens);
			}

			var command_header = new CommandHeader
			{
				ConditionDataHeaders = _dataCellRepository.Get(condition_data_tokens).Select(x => (DataCellHeader)x.Header).ToList(),
				InputDataHeaders = input_data_tokens.Select(x => new DataCellHeader { Token = x }).ToList(),
				OutputDataHeader = new DataCellHeader { Token = command_token.Next("result") },
				Token = command_token
			};
			command_header.Header = command_header;

			new_command = new Command()
			{
				Header = command_header,
				InputData = inputs.ToList(),
				OutputData = new DataCell
				{
					Header = command_header.OutputDataHeader,
					HasValue = null,
					Data = null
				},
				ConditionData = condition_data,
				Function = function
			};

			return all_ready;
		}

		public void PrepareCommand(Token command_header_token)
		{
			if (_preparingCommands.ContainsKey(command_header_token))
			{
				throw new NotImplementedException("PreparationCommandService.PrepareCommand Такая команда уже находится в подготовке");
			}
			
			// Добавляем новую команду, находящуюся в процессе подготовки.
			if (TryCreateCommand(command_header_token, out Command new_command))
			{
				OnPreparedCommand(new_command);
			}
		}

		/// <summary>
		/// Отправляет зависимые команды на проверку готовности к исполнению.
		/// </summary>
		/// <param name="command_token">Токен выполненной команды, зависимости с которой необходимо проверить.</param>
		/// <param name="data_cell_token">Токен ячейки с данными.</param>
		private void CheckPreparingCommands(Token command_token, string data_cell_token)
		{
			if (!_preparingCommands.TryGetValue(command_token, out string command_header)) return;
			if (!TryCreateCommand(command_token, out Command new_command)) return;

			OnPreparedCommand(new_command);

			if (!_preparingCommands.TryRemove(command_token, out command_header)) return;

			_dataLinksWithCommands[data_cell_token].Remove(command_token);

			if (_dataLinksWithCommands[data_cell_token].Count == 0)
			{
				_dataLinksWithCommands.TryRemove(data_cell_token, out List<string> str);
			}
		}

		/// <summary>
		/// Отправляет команды прошедшие проверку условия на проверку готовности к исполнению.
		/// </summary>
		/// <param name="command_token">Токен команды.</param>
		private void CheckWaitConditionCommands(Token command_token)
		{
			if (!_waitConditionCommands.TryGetValue(command_token, out string _)) return;
			if (!TryCreateCommand(command_token, out Command new_command)) return;

			OnPreparedCommand(new_command);
			_waitConditionCommands.TryRemove(command_token, out string token);
		}

		/// <summary>
		/// Метод вызываемый в момент, когда получен результат вычислений функции.
		/// Ожидается, что ячейка с данными уже должна содержать значение.
		/// </summary>
		/// <param name="data_cell_token">Токен ячейки с данными.</param>
		public void OnDataReady(string data_cell_token)
		{
			if (!_dataLinksWithCommands.TryGetValue(data_cell_token, out List<string> command_tokens)) return;

			foreach (var command_token in command_tokens.ToList())
			{
				CheckPreparingCommands(command_token, data_cell_token);
				CheckWaitConditionCommands(command_token);
			}
		}
		/*
		public void OnFunctionReady(FunctionHeader function_header)
		{
			var function = _functionRepository.Get(function_header.Token).FirstOrDefault();
			if (function == null)
			{
				_functionRepository.Subscribe(new[] { function_header }, OnFunctionReady);
			}
			else
			{
				if (_preparingCommands.TryGetValue(function_header.Token, out CommandHeader command_header))
				{
					if (TryCreateCommand(command_header.Token, null, out Command new_command))
					{
						if (_preparingCommands.TryRemove(function_header.Token, out command_header))
						{
							OnPreparedCommand(new_command);
						}
					}
				}
			}
		}*/

		public void Clear(string path)
		{
			var key_list = _preparingCommands.Keys.ToList().Where(x => x.StartsWith(path));

			foreach (var ke in key_list)
			{
				_preparingCommands.TryRemove(ke, out string _);
			}

			key_list = _waitConditionCommands.Keys.ToList().Where(x => x.StartsWith(path));
			foreach (var ke in key_list)
			{
				_waitConditionCommands.TryRemove(ke, out string _);
			}

			key_list = _dataLinksWithCommands.Keys.ToList().Where(x => x.StartsWith(path));
			foreach (var ke in key_list)
			{
				_dataLinksWithCommands.TryRemove(ke, out List<string> _);
			}
		}
	}
}

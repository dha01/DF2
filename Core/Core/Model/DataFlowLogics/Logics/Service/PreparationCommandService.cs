using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
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

		private IDataCellRepository _dataCellRepository;

		private IFunctionRepository _functionRepository;

		private ConcurrentDictionary<string, CommandHeader> _preparingCommands { get; set; }
		private ConcurrentDictionary<string, CommandHeader> _waitConditionCommands { get; set; } = new ConcurrentDictionary<string, CommandHeader>();

		private ConcurrentDictionary<string, List<string>> _dataLinksWithCommands = new ConcurrentDictionary<string, List<string>>();

		public PreparationCommandService(IDataCellRepository data_cell_repository, IFunctionRepository function_repository)
		{
			_preparingCommands = new ConcurrentDictionary<string, CommandHeader>();
			_dataCellRepository = data_cell_repository;
			_functionRepository = function_repository;
		}

		private void AddCommandToPreparing(CommandHeader new_command_header)
		{
			if (_preparingCommands.TryAdd(new_command_header.Token, new_command_header))
			{
				//throw new NotImplementedException("PreparationCommandService.PrepareCommand не удалось добавить.");
			}

			foreach (var input_data in new_command_header.InputDataHeaders)
			{
				if (_dataLinksWithCommands.ContainsKey(input_data.Token))
				{
					_dataLinksWithCommands[input_data.Token].Add(new_command_header.Token);
				}
				else
				{
					_dataLinksWithCommands[input_data.Token] = new List<string> { new_command_header.Token };
				}
			}
		}
		
		private DataCell GetOutputData(DataCellHeader data_cell_header)
		{
			var output_data = _dataCellRepository.Get(data_cell_header.Token).FirstOrDefault();
			if (output_data == null)
			{
				output_data = new DataCell()
				{
					Header = data_cell_header,
					HasValue = null,
					Data = null
				};
				//_dataCellRepository.Add(new[] { output_data });
			}

			return output_data;
		}

		private DataCell GetDataCell(DataCellHeader data_cell_header)
		{
			var data_cell = _dataCellRepository.Get(data_cell_header.Token).FirstOrDefault();
			if (data_cell == null)
			{
				return new DataCell()
				{
					Header = data_cell_header,
					HasValue = null,
					Data = null
				};

				//_dataCellRepository.Subscribe(data_cell_header, OnDataReady);
				// TODO: нужно отправлять запросы за другие узлы для получения данных.
			}
			else
			{
				return data_cell;
			}
		}

		/// <summary>
		/// Проверяет, что для команды либо нет условий, либо хотя бы одно условие уже получено.
		/// </summary>
		/// <param name="command_header">Заголовок команды.</param>
		/// <param name="condition_data_cells">Условия.</param>
		/// <returns>True, если нет условий или хотя бы одно условие уже получено.</returns>
		private bool CheckCommandCodition(CommandHeader command_header, out List<DataCell> condition_data_cells)
		{
			//command_header.Token

			condition_data_cells = new List<DataCell>();
			var condition_data_cells_out = _dataCellRepository.Get(command_header.ConditionDataHeaders.Select(x => (string)x.Token).ToArray()).ToList();
			if (command_header.ConditionDataHeaders.Any() && !condition_data_cells_out.Any(x =>x != null && x.HasValue != null && x.HasValue.Value))
			{
				if (_waitConditionCommands.TryAdd(command_header.Token, command_header))
				{
					//throw new NotImplementedException("PreparationCommandService.CheckCommandCodition не удалось добавить.");
				}

				foreach (var input_dataa in command_header.ConditionDataHeaders)
				{
					if(!_dataLinksWithCommands.TryGetValue(input_dataa.Token, out List<string> list))
					{
						list = new List<string>();
						_dataLinksWithCommands.TryAdd(input_dataa.Token, list);
					}

					list.Add(command_header.Token);
				}
				return false;
			}
			condition_data_cells.AddRange(condition_data_cells_out);
			return true;
		}

		/// <summary>
		/// Пытается созадть готовую к исполнению команду. Если команда не может быть исполнена на данный момент, то возвращает false. 
		/// </summary>
		/// <param name="command_header">Заголовок команды.</param>
		/// <param name="new_command">Созданная команда.</param>
		/// <returns>Удалось ли создать готовую к исполнению команду.</returns>
		private bool TryCreateCommand(CommandHeader command_header, out Command new_command)
		{
			new_command = null;

			// Если условия ещё не готовы, то возвращаем null.
			if (!CheckCommandCodition(command_header, out List<DataCell> condition_data))
			{
				return false;
			}

			// Если нет ни одного истинного условия, то возвращаем null.
			if (condition_data.Any(x => x != null && x.HasValue.HasValue && x.HasValue.Value && !(bool)x.Data))
			{
				return false;
			}

			// Подготавливае места для ячеек с выходными данными.
			var count = command_header.InputDataHeaders.Count;
			var input_data = new DataCell[count];

			new_command = new Command()
			{
				Header = command_header,
				InputData = input_data.ToList(),
				OutputData = GetOutputData(command_header.OutputDataHeader),
				ConditionData = condition_data
			};

			var all_ready = true;

			switch (command_header.FunctionHeader.Condition)
			{
				case InputParamCondition.All:

					var in_data = _dataCellRepository.Get(command_header.InputDataHeaders.Select(x => (string)x.Token).ToArray()).ToList();
					new_command.InputData = in_data;
					if (in_data.Any(x => x == null))
					{
						all_ready = false;
					}

					// Получаем или подписываемся на получение входных параметров.
					/*for (int i = 0; i < command_header.InputDataHeaders.Count; i++)
					{
						var result = GetDataCell(command_header.InputDataHeaders[i]);
						new_command.InputData[i] = result;
						
						if (result.HasValue == null)
						{
							all_ready = false;
						}
					}*/
					break;
				case InputParamCondition.Any:
					var any = false;

					var in_data2 = _dataCellRepository.Get(command_header.InputDataHeaders.Select(x => (string)x.Token).ToArray()).ToList();
					new_command.InputData = in_data2;
					if (in_data2.Any(x => x != null && x.HasValue != null && x.HasValue.Value))
					{
						any = true;
					}
					// Получаем или подписываемся на получение входных параметров.
					/*for (int i = 0; i < command_header.InputDataHeaders.Count; i++)
					{
						var result = GetDataCell(command_header.InputDataHeaders[i]);
						new_command.InputData[i] = result;

						if (result.HasValue != null && result.HasValue.Value)
						{
							any = true;
						}
					}*/

					if (!any)
					{
						all_ready = false;
					}
					break;
				default:
					throw new Exception($"CreateNewCommand Неизвестный тип: {command_header.FunctionHeader.Condition}");
			}

			if (!all_ready)
			{
				AddCommandToPreparing(command_header);
			}
			//_dataCellRepository.Add(new_command.InputData.Where(x => x.HasValue == null), false);

			// Получаем или подписываемся на получение функций.
			var function = _functionRepository.Get(command_header.FunctionHeader.Token).FirstOrDefault();
			if (function == null)
			{
				if (all_ready)
				{
					all_ready = false;
					AddCommandToPreparing(command_header);
				}
				//_functionRepository.Subscribe(function_header, OnFunctionReady);
				// TODO: нужно отправлять запросы на другие узлы для получения функции.
			}
			else
			{
				new_command.Function = function;
			}

			return all_ready;
		}

		public void PrepareCommand(CommandHeader command_header)
		{
			if (_preparingCommands.ContainsKey(command_header.Token))
			{
				throw new NotImplementedException("PreparationCommandService.PrepareCommand Такая команда уже находится в подготовке");
			}
			
			// Добавляем новую команду, находящуюся в процессе подготовки.
			if (TryCreateCommand(command_header, out Command new_command))
			{
				OnPreparedCommand(new_command);
			}
		}

		/// <summary>
		/// Отправляет зависимые команды на проверку готовности к исполнению.
		/// </summary>
		/// <param name="command_token">Токен выполненной команды, зависимости с которой необходимо проверить.</param>
		/// <param name="data_cell_token">Токен ячейки с данными.</param>
		private void CheckPreparingCommands(string command_token, string data_cell_token)
		{
			if (_preparingCommands.TryGetValue(command_token, out CommandHeader command_header))
			{

				var input_data = _dataCellRepository.Get(command_header.InputDataHeaders.Select(x => (string)x.Token).ToArray()).ToList();

				if (_functionRepository.Get(command_header.FunctionHeader.Token) != null)
				{
					switch (command_header.FunctionHeader.Condition)
					{
						case InputParamCondition.All:
							if (input_data.All(x => x != null && x.HasValue != null && x.HasValue.Value))
							{
								break;
							}
							return;
						case InputParamCondition.Any:
							if (input_data.Any(x => x != null && x.HasValue != null && x.HasValue.Value))
							{
								break;
							}
							return;
						default:
							throw new Exception($"OnDataReady Неизвестный тип: {command_header.FunctionHeader.Condition}");
					}

					if (_preparingCommands.TryRemove(command_token, out command_header))
					{
						_dataLinksWithCommands[data_cell_token].Remove(command_token);
						if (_dataLinksWithCommands[data_cell_token].Count == 0)
						{
							_dataLinksWithCommands.TryRemove(data_cell_token, out List<string> str);
						}

						if (TryCreateCommand(command_header, out Command new_command))
						{
							OnPreparedCommand(new_command);
						}
					}
				}
			}
		}

		/// <summary>
		/// Отправляет команды прошедшие проверку условия на проверку готовности к исполнению.
		/// </summary>
		/// <param name="command_token">Токен команды.</param>
		/// <param name="condition_data_cell_token">Токен ячейки с данными с условием.</param>
		private void CheckWaitConditionCommands(string command_token, string condition_data_cell_token)
		{
			if (_waitConditionCommands.TryGetValue(command_token, out CommandHeader command_header))
			{
				if ((bool)_dataCellRepository.Get(condition_data_cell_token).First().Data)
				{
					if (TryCreateCommand(command_header, out Command new_command))
					{
						OnPreparedCommand(new_command);
					}
					_waitConditionCommands.TryRemove(command_token, out CommandHeader token);
				}
				else
				{
					if (command_header.ConditionDataHeaders.Count <= 1)
					{
						_waitConditionCommands.TryRemove(command_token, out CommandHeader token);
					}
				}
			}
		}

		/// <summary>
		/// Метод вызываемый в момент, когда получен результат вычислений функции.
		/// Ожидается, что ячейка с данными уже должна содержать значение.
		/// </summary>
		/// <param name="data_cell_token">Токен ячейки с данными.</param>
		public void OnDataReady(string data_cell_token)
		{
			if (_dataLinksWithCommands.TryGetValue(data_cell_token, out List<string> command_tokens))
			{
				foreach (var command_token in command_tokens.ToList())
				{
					CheckPreparingCommands(command_token, data_cell_token);
					CheckWaitConditionCommands(command_token, data_cell_token);
				}
			}
		}

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
					if (TryCreateCommand(command_header, out Command new_command))
					{
						if (_preparingCommands.TryRemove(function_header.Token, out command_header))
						{
							OnPreparedCommand(new_command);
						}
					}
				}
			}
		}

		public void Clear(string path)
		{
			CommandHeader removed_command_header;
			CommandHeader removed_commandd;
			List<string> removed_list;
			var key_list = _preparingCommands.Keys.ToList().Where(x => x.StartsWith(path));

			foreach (var ke in key_list)
			{
				_preparingCommands.TryRemove(ke, out removed_commandd);
			}
			

			key_list = _waitConditionCommands.Keys.ToList().Where(x => x.StartsWith(path));
			foreach (var ke in key_list)
			{
				_waitConditionCommands.TryRemove(ke, out removed_command_header);
			}

			key_list = _dataLinksWithCommands.Keys.ToList().Where(x => x.StartsWith(path));
			foreach (var ke in key_list)
			{
				_dataLinksWithCommands.TryRemove(ke, out removed_list);
			}
		}
	}
}

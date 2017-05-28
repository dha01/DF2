using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.Repository;

namespace Core.Model.DataFlowLogics.Logics.Service
{
	public class PreparationCommandService : IPreparationCommandService
	{

		public Action<Command> OnPreparedCommand { get; set; }

		private IDataCellRepository _dataCellRepository;

		private IFunctionRepository _functionRepository;

		private ConcurrentDictionary<string, Command> _preparingCommands { get; set; }

		private ConcurrentDictionary<string, List<string>> _dataLinksWithCommands = new ConcurrentDictionary<string, List<string>>();

		public PreparationCommandService(IDataCellRepository data_cell_repository, IFunctionRepository function_repository)
		{
			_preparingCommands = new ConcurrentDictionary<string, Command>();
			_dataCellRepository = data_cell_repository;
			_functionRepository = function_repository;
		}

		private void AddCommandToPreparing(Command new_command)
		{
			if (_preparingCommands.TryAdd(new_command.Header.Token, new_command))
			{
				//throw new NotImplementedException("PreparationCommandService.PrepareCommand не удалось добавить.");
			}

			foreach (var input_data in new_command.InputData)
			{
				if (_dataLinksWithCommands.ContainsKey(input_data.Header.Token))
				{
					_dataLinksWithCommands[input_data.Header.Token].Add(new_command.Header.Token);
				}
				else
				{
					_dataLinksWithCommands[input_data.Header.Token] = new List<string>() { new_command.Header.Token };
				}
			}
		}

		private DataCell GetOutputData(DataCellHeader data_cell_header)
		{
			var output_data = _dataCellRepository.Get(new[] { data_cell_header }).FirstOrDefault();
			if (output_data == null)
			{
				output_data = new DataCell()
				{
					Header = data_cell_header,
					HasValue = false,
					Data = null
				};
				_dataCellRepository.Add(new[] { output_data });
			}

			return output_data;
		}

		private Command CreateNewCommand(CommandHeader command_header)
		{
			// Получаем выходную ячейку данных.
			var output_data = GetOutputData(command_header.OutputDataHeader);

			// Подготавливае места для ячеек с выходными данными.
			var count = command_header.InputDataHeaders.Count;
			var input_data = new List<DataCell>(count);
			for (int i = 0; i < count; i++)
			{
				input_data.Add(null);
			}

			var new_command = new Command()
			{
				Header = new InvokeHeader()
				{
					CallStack = command_header.CallStack
				},
				InputData = input_data,
				OutputData = output_data
			};

			var all_ready = true;

			// Получаем или подписываемся на получение входных параметров.
			for (int i = 0; i < command_header.InputDataHeaders.Count; i++)
			{
				if (command_header.InputDataHeaders.Last().CallStack.Last().StartsWith("const"))
				{
					//var p = 1;
				}
				var data_cell = _dataCellRepository.Get(new[] { command_header.InputDataHeaders[i] }).FirstOrDefault();
				if (data_cell == null || !data_cell.HasValue)
				{
					all_ready = false;

					var new_data = new DataCell()
					{
						Header = command_header.InputDataHeaders[i],
						HasValue = false,
						Data = null
					};
					new_command.InputData[i] = new_data;

					//_dataCellRepository.Subscribe(data_cell_header, OnDataReady);
					// TODO: нужно отправлять запросы за другие узлы для получения данных.
				}
				else
				{
					new_command.InputData[i] = data_cell;
				}
			}

			if (!all_ready)
			{
				AddCommandToPreparing(new_command);
			}
			_dataCellRepository.Add(new_command.InputData.Where(x => !x.HasValue), false);

			// Получаем или подписываемся на получение функций.
			var function_header = new[] { command_header.FunctionHeader };
			var function = _functionRepository.Get(function_header).FirstOrDefault();
			if (function == null)
			{
				if (all_ready)
				{
					all_ready = false;
					AddCommandToPreparing(new_command);
				}
				//_functionRepository.Subscribe(function_header, OnFunctionReady);
				// TODO: нужно отправлять запросы за другие узлы для получения функции.
			}
			else
			{
				new_command.Function = function;
			}

			// Если команда готова, то возвращаем её.
			if (all_ready)
			{
				return new_command;
			}
			else
			{
				return null;
			}
		}

		public void PrepareCommand(CommandHeader command_header)
		{
			if (_preparingCommands.ContainsKey(command_header.Token))
			{
				throw new NotImplementedException("PreparationCommandService.PrepareCommand Такая команда уже находится в подготовке");
			}
			
			// Добавляем новую команду, находящуюся в процессе подготовки.
			var new_command = CreateNewCommand(command_header);
			if (new_command != null)
			{
				OnPreparedCommand(new_command);
			}
		}

		public void OnDataReady(DataCellHeader data_cell_header)
		{
			var data_cell_headers = new[] { data_cell_header };
			var data_cell = _dataCellRepository.Get(data_cell_headers).FirstOrDefault();

			if (data_cell != null && data_cell.HasValue)
			{
				if (_dataLinksWithCommands.ContainsKey(data_cell_header.Token))
				{
					var command_tokens = _dataLinksWithCommands[data_cell_header.Token].ToList();
					foreach (var command_token in command_tokens)
					{
						if (_preparingCommands.ContainsKey(command_token))
						{
							var command = _preparingCommands[command_token];
							command.InputData = _dataCellRepository.Get(command.InputData.Select(x => (DataCellHeader)x.Header)).ToList();

							if (command.InputData.All(x => x.HasValue) && command.Function != null && _preparingCommands.TryRemove(command_token, out command))
							{
								_dataLinksWithCommands[data_cell_header.Token].Remove(command_token);
								if (_dataLinksWithCommands[data_cell_header.Token].Count == 0)
								{
									_dataLinksWithCommands.TryRemove(data_cell_header.Token, out List<string> str);
								}
								OnPreparedCommand(command);
							}
						}
					}
				}
			}
		}

		public void OnFunctionReady(FunctionHeader function_header)
		{
			var function = _functionRepository.Get(new[] { function_header }).FirstOrDefault();
			if (function == null)
			{
				_functionRepository.Subscribe(new[] { function_header }, OnFunctionReady);
			}
			else
			{
				if (_preparingCommands.ContainsKey(function_header.Token))
				{
					var command = _preparingCommands[function_header.Token];
					command.Function = function;
					if (command.InputData.All(x => x.HasValue) && command.Function != null)
					{
						if (_preparingCommands.TryRemove(function_header.Token, out command))
						{
							OnPreparedCommand(command);
						}
					}
				}
			}
		}
	}
}

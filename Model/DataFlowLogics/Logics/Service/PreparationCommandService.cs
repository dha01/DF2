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

		//private ConcurrentDictionary<string, List<string>>

		public PreparationCommandService(IDataCellRepository data_cell_repository, IFunctionRepository function_repository)
		{
			_preparingCommands = new ConcurrentDictionary<string, Command>();
			_dataCellRepository = data_cell_repository;
			_functionRepository = function_repository;
		}

		public void PrepareCommand(CommandHeader command_header)
		{
			var key = command_header.CallstackToString();

			if (_preparingCommands.ContainsKey(key))
			{
				throw new NotImplementedException("PreparationCommandService.PrepareCommand Такая команда уже находится в подготовке");
			}

			// Получаем выходную ячейку данных.
			var output_data = _dataCellRepository.Get(new[] { command_header.OutputDataHeader }).FirstOrDefault();
			if (output_data == null)
			{
				output_data = new DataCell()
				{
					Header = command_header.OutputDataHeader,
					HasValue = false,
					Data = null
				};
				_dataCellRepository.Add(new [] { output_data });
			}

			// Подготавливае места для ячеек с выходными данными.
			var count = command_header.InputDataHeaders.Count;
			var input_data = new List<DataCell>(count);
			for (int i = 0; i < count; i++)
			{
				input_data.Add(null);
			}

			// Добавляем новую команду, находящуюся в процессе подготовки.
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
			for (int i = 0; i < count; i++)
			{
				var data_cell_header = new[] { command_header.InputDataHeaders[i] };
				var data_cell = _dataCellRepository.Get(data_cell_header).FirstOrDefault();
				if (data_cell == null || !data_cell.HasValue)
				{
					if (all_ready)
					{
						all_ready = false;
						if (_preparingCommands.TryAdd(key, new_command))
						{
							//throw new NotImplementedException("PreparationCommandService.PrepareCommand не удалось добавить.");
						}
					}
					
					var new_data = new DataCell()
					{
						Header = command_header.InputDataHeaders[i],
						HasValue = false,
						Data = null
					};
					new_command.InputData[i] = new_data;
					_dataCellRepository.Add(new []{ new_data }, false);
					//_dataCellRepository.Subscribe(data_cell_header, OnDataReady);
					// TODO: нужно отправлять запросы за другие узлы для получения данных.
				}
				else
				{
					new_command.InputData[i] = data_cell;
				}
			}

			// Получаем или подписываемся на получение функций.
			var function_header = new[] { command_header.FunctionHeader };
			var function = _functionRepository.Get(function_header).FirstOrDefault();
			if (function == null)
			{
				if (all_ready)
				{
					all_ready = false;
					if (_preparingCommands.TryAdd(key, new_command))
					{
						throw new NotImplementedException("PreparationCommandService.PrepareCommand не удалось добавить.");
					}
				}
				//_functionRepository.Subscribe(function_header, OnFunctionReady);
				// TODO: нужно отправлять запросы за другие узлы для получения функции.
			}
			else
			{
				new_command.Function = function;
			}

			if (all_ready)
			{
				OnPreparedCommand(new_command);
			}
		}

		public void OnDataReady(DataCellHeader data_cell_header)
		{
			Console.WriteLine("! OnDataReady1 {0}", data_cell_header.CallstackToString());
			
			var data_cell_headers = new[] { data_cell_header };
			var data_cell = _dataCellRepository.Get(data_cell_headers).FirstOrDefault();
			if (data_cell == null || !data_cell.HasValue)
			{
				//_dataCellRepository.Subscribe(data_cell_headers, OnDataReady);
			}
			else
			{
				Console.WriteLine("! OnDataReady2 {0} {1}", data_cell_header.CallstackToString(), data_cell.Data);
				
				// TODO: нужно сделать нормальный словарь.
				var command = _preparingCommands.Values.FirstOrDefault(x => x.InputData.Any(y => y !=null && y.Header.CallstackToString().Equals(data_cell_header.CallstackToString())));

				if (/*_preparingCommands.ContainsKey(key)*/ command != null)
				{
					var key = command.Header.CallstackToString();
					command.InputData = _dataCellRepository.Get(command.InputData.Select(x => (DataCellHeader) x.Header)).ToList();

					if (command.InputData.All(x => x.HasValue) && command.Function != null)
					{
						if (!_preparingCommands.TryRemove(key, out command))
						{
							throw new NotImplementedException("PreparationCommandService.OnDataReady не удалось извлеч.");
						}
						OnPreparedCommand(command);
					}
				}
			}
		}

		public void OnFunctionReady(FunctionHeader function_header)
		{
			Console.WriteLine("! OnFunctionReady {0}", function_header.CallstackToString());
			
			var function_headers = new[] { function_header };
			var function = _functionRepository.Get(function_headers).FirstOrDefault();
			if (function == null)
			{
				_functionRepository.Subscribe(function_headers, OnFunctionReady);
			}
			else
			{
				var key = function_header.CallstackToString();

				if (_preparingCommands.ContainsKey(key))
				{
					var command = _preparingCommands[key];
					command.Function = function;
					if (command.InputData.All(x => x.HasValue) && command.Function != null)
					{
						if (_preparingCommands.TryRemove(key, out command))
						{
							throw new NotImplementedException("PreparationCommandService.OnDataReady не удалось извлеч.");
						}
						OnPreparedCommand(command);
					}
				}
			}
		}
	}
}

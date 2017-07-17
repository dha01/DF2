using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model.CodeCompiler.Code;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.CodeExecution.Repository;
using Core.Model.DataFlowLogics.Logics.Service;
using Core.Model.NetworkLogic;

namespace Core.Model.CodeExecution.Service.Execution
{
	/// <summary>
	/// Исполняющий сервис управляющих функций.
	/// </summary>
	public class ControlExecutionService : IExecutionService
	{
		//private ICommandRepository _commandRepository;

		private IDataFlowLogicsService _dataFlowLogicsService;

		private IExecutionService _basicExecutionService;

		private IDataCellRepository _dataCellRepository;

		public ControlExecutionService(IDataCellRepository _data_cell_repository/*ICommandRepository command_repository*/)
		{
			//_commandRepository = command_repository;
			_dataCellRepository = _data_cell_repository;
		}

		/// <summary>
		/// TODO: костыль, но лучшего решения пока не придумал.
		/// </summary>
		/// <param name="data_flow_logics_service"></param>
		public void SetDataFlowLogicsService(IDataFlowLogicsService data_flow_logics_service)
		{
			_dataFlowLogicsService = data_flow_logics_service;
			_basicExecutionService = new BasicExecutionService();
		}

		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, Token? callstack = null)
		{
			var control_function = (ControlFunction)function;
			var input_data_count = input_data.Count();
			var tmp_count = control_function.Commands.Count() + input_data_count/* + 1*/;

		//	var token = new Token(string.Join("/", callstack.ToList()));

			// Локальный массив временных данных функции. Добавляем выходные данные нулевым элементом.
			var tmp_array = new List<DataCell>(tmp_count) { output };

			// Добавляем входные данные.
			tmp_array.AddRange(input_data);

			int count = input_data.Count() + 1;

			// Добавляем ячейки для всех остальных команд.
			for (int i = 0; i < control_function.Commands.Count() - 1; i++)
			{
				var data = new DataCell()
				{
					Header = new DataCellHeader()
					{
						Token = callstack.Value.Next($"tmp_var_{i + count}")
					},
					Data = null,
					HasValue = null
				};
				tmp_array.Add(data);
			}

			// Добавляем ячейки с константами.
			for (int i = 0; i < control_function.Constants.Count; i++)
			{
				var data = new DataCell()
				{
					Header = new DataCellHeader()
					{
						Token = function.Token.Next($"const_{i}")
					},
					Data = control_function.Constants[i],
					HasValue = true
				};
				tmp_array.Add(data);
			}

			// Создаем список новых команд.
			var command_list = control_function.Commands.ToList();

			if (command_list.All(x => x.FunctionHeader is BasicFunctionHeader))
			{
				// Исполняем базовые команды.

				var command_template = command_list.FirstOrDefault(
					y => tmp_array[y.OutputDataId].HasValue == null &&
					     y.ConditionId.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value) &&
					     y.InputDataIds.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value));

				while (command_template != null)
				{
					if (command_template.FunctionHeader is BasicFunctionHeader &&
					    command_template.ConditionId.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value) &&
					    command_template.InputDataIds.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value))
					{
						var func = BasicFunctionModel.AllMethods[((BasicFunctionHeader) command_template.FunctionHeader).Name]
							.BasicFunction;
						_basicExecutionService.Execute(func, command_template.InputDataIds.Select(x => tmp_array[x]),
							tmp_array[command_template.OutputDataId]);
					}


					command_template = command_template.TriggeredCommandIds.Select(x => command_list[x])
						.FirstOrDefault(
							y => tmp_array[y.OutputDataId].HasValue == null &&
							     y.ConditionId.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value) &&
							     y.InputDataIds.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value));
					if (command_template != null)
					{
						continue;
					}

					command_template = command_list.FirstOrDefault(
						y => tmp_array[y.OutputDataId].HasValue == null &&
						     y.ConditionId.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value) &&
						     y.InputDataIds.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value));
				}
			}
			else
			{
				List<Tuple<string, string>> dublicate = new List<Tuple<string, string>>();

				var index = 0;/*
				_dataCellRepository.Add(input_data.Select(x => new DataCell
				{
					Header = new DataCellHeader
					{
						Token = callstack.Value.Next($"InputData{index++}")
					},
					HasValue = x.HasValue,
					Data = x.Data
				}));*/
				
				List<CommandHeader> new_commands = new List<CommandHeader>();
				// Добаляем новые команды на исполнение
				foreach (var command_template in command_list)
				{
					var command_token = callstack.Value.Next($"{command_template.FunctionHeader.Token}<{command_template.OutputDataId}>");
					var input_datas = command_template.InputDataIds.Select(x => (DataCellHeader) tmp_array[x].Header).ToList();

					var input_index = 0;
					dublicate.AddRange(input_datas.Select(y => new Tuple<string, string>(y.Token, command_token.Next($"InputData{input_index++}"))).Where(x =>
						{
							var last = new Token(x.Item1).Last();
							return last.StartsWith("InputData") || last.StartsWith("const") || !new Token(x.Item1).ToString().StartsWith(callstack);
						})
					);

					var new_command_header = new CommandHeader
					{
						Token = command_token,
						InputDataHeaders = input_datas,
						OutputDataHeader = (DataCellHeader) tmp_array[command_template.OutputDataId].Header,
						TriggeredCommands = command_template.TriggeredCommandIds.Select(x => command_list[x].Header).ToList(),
						FunctionHeader = command_template.FunctionHeader,
						ConditionDataHeaders = command_template.ConditionId.Select(x => (DataCellHeader) tmp_array[x].Header).ToList()
					};
					new_commands.Add(new_command_header);

					_dataCellRepository.CreateDublicate(dublicate.ToArray());
					dublicate.Clear();
					Parallel.Invoke(() => { _dataFlowLogicsService.AddNewCommandHeader(new_command_header); });
				}

				//_dataCellRepository.CreateDublicate(dublicate.ToArray());
				//Parallel.Invoke(new_commands.Select(x => new Action(() => { _dataFlowLogicsService.AddNewCommandHeader(x); })).ToArray());
			}
		}
	}
}


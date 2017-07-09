using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model.CodeCompiler.Code;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
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

		public ControlExecutionService(/*ICommandRepository command_repository*/)
		{
			//_commandRepository = command_repository;
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

		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, string[] callstack = null)
		{
			var control_function = (ControlFunction)function;
			var input_data_count = input_data.Count();
			var tmp_count = control_function.Commands.Count() + input_data_count/* + 1*/;

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
						CallStack = callstack.Concat(new[] { function.Token, $"tmp_var_{i + count}" }).ToArray()
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
						CallStack = new []{ function.Token, $"const_{i}" }
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
						var func = BasicFunctionModel.AllMethods[((BasicFunctionHeader)command_template.FunctionHeader).Name].BasicFunction;
						_basicExecutionService.Execute(func, command_template.InputDataIds.Select(x => tmp_array[x]), tmp_array[command_template.OutputDataId]);
					}


					command_template = command_template.TriggeredCommandIds.Select(x => command_list[x]).FirstOrDefault(
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
			// Добаляем новые команды на исполнение
			foreach (var command_template in command_list)
			{
				var new_command_header = new CommandHeader
				{
					CallStack = callstack.Concat(new[] { $"{command_template.FunctionHeader.Token}<{command_template.OutputDataId}>" }).ToArray(),
					InputDataHeaders = command_template.InputDataIds.Select(x => (DataCellHeader)tmp_array[x].Header).ToList(),
					OutputDataHeader = (DataCellHeader)tmp_array[command_template.OutputDataId].Header,
					TriggeredCommands = command_template.TriggeredCommandIds.Select(x => command_list[x].Header).ToList(),
					FunctionHeader = command_template.FunctionHeader,
					ConditionDataHeaders = command_template.ConditionId.Select(x => (DataCellHeader)tmp_array[x].Header).ToList()
				};
				/*
				if (command_template.FunctionHeader is BasicFunctionHeader &&
				    command_template.ConditionId.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value) &&
					command_template.InputDataIds.Select(x => tmp_array[x]).All(x => x.HasValue != null && x.HasValue.Value))
				{
					//((BasicFunctionHeader)command_template.FunctionHeader).
					var func = BasicFunctionModel.AllMethods[((BasicFunctionHeader) command_template.FunctionHeader).Name].BasicFunction;

					_basicExecutionService.Execute(func, command_template.InputDataIds.Select(x => tmp_array[x]), tmp_array[command_template.OutputDataId]);

					Parallel.Invoke(() => { _dataFlowLogicsService.OnExecutedCommand(new Command
						{
							Header = new_command_header,
							InputData = command_template.InputDataIds.Select(x => tmp_array[x]).ToList(),
							OutputData = tmp_array[command_template.OutputDataId],
							TriggeredCommands = new List<CommandHeader>(),
							ConditionData = command_template.ConditionId.Select(x => tmp_array[x]).ToList(),
							Function = func
						});
					});
				}*/

				Parallel.Invoke(() => { _dataFlowLogicsService.AddNewCommandHeader(new_command_header); });
			}
		}
	}
}


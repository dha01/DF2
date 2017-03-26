using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.Repository;

namespace Core.Model.Execution
{
	/// <summary>
	/// Исполняющий сервис управляющих функций.
	/// </summary>
	public class ControlExecutionService : IExecutionService
	{
		private ICommandRepository _commandRepository;

		public ControlExecutionService(ICommandRepository command_repository)
		{
			_commandRepository = command_repository;
		}

		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null)
		{
			var control_function = (ControlFunction)function;
			var input_data_count = input_data.Count();
			var tmp_count = control_function.Commands.Count() + input_data_count + 1;

			// Локальный массив временных данных функции. Добавляем выходные данные нулевым элементом.
			var tmp_array = new List<DataCell>(tmp_count) { output };

			// Добавляем входные данные.
			tmp_array.AddRange(input_data);

			int count = input_data.Count() + 1;

			// Добавляем ячейки для всех остальных команд.
			for (int i = 0; i < control_function.Commands.Count(); i++)
			{
				var callstack = command_context.Header.CallStack.ToList();
				callstack.Add(function.GetHeader<FunctionHeader>().CallstackToString("."));
				callstack.Add(string.Format("tmp_var_{0}", i + count));
				var data = new DataCell()
				{
					Header = new DataCellHeader()
					{
						HasValue = new Dictionary<Owner, bool>(),
						Owners = new List<Owner>(),
						CallStack = callstack
					},
					Data = null,
					HasValue = false
				};
				tmp_array.Add(data);
			}

			// Создаем список новых команд.
			var command_list = control_function.Commands.ToList();

			// Добаляем новые команды на исполнение
			foreach (var command_template in command_list)
			{
				var call_stack_count = input_data.First().Header.CallStack.Count();
				var callstack = command_context.Header.CallStack.ToList(); //input_data.First().Header.CallStack.Take(call_stack_count - 1).ToList();
				//callstack.Add(function.GetHeader<FunctionHeader>().CallstackToString("."));
				callstack.Add(String.Format("{0}<{1}>",command_template.FunctionHeader.CallstackToString("."), command_template.OutputDataId));
				var new_command = new CommandHeader
				{
					Owners = new List<Owner>(),
					CallStack = callstack,
					InputDataHeaders = command_template.InputDataIds.Select(x => (DataCellHeader)tmp_array[x].Header).ToList(),
					OutputDataHeader = (DataCellHeader)tmp_array[command_template.OutputDataId].Header,
					TriggeredCommands = command_template.TriggeredCommandIds.Select(x => command_list[x].Header).ToList(),
					FunctionHeader = command_template.FunctionHeader
				};
				if (callstack.Last().StartsWith("User1.BasicFunctions.ControlCallFunction2")
				|| callstack.Last().StartsWith("User1.BasicFunctions.CallFunction2<4>"))
				{
					var c = 2;
				}

				//Console.WriteLine(string.Format("ControlExecutionService.Execute Callstack={0},  Function={1}", string.Join("/", function.Header.CallStack), ((FunctionHeader)function.Header).Name));

				Parallel.Invoke(() => { _commandRepository.Add(new[] { new_command }, command_template.InputDataIds.Any(x => x <= input_data_count)); });
			}
		}
	}
}


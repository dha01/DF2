using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
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

		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output)
		{
			var control_function = (ControlFunction)function;
			var tmp_count = control_function.Commands.Count() + input_data.Count() + 1;

			// Локальный массив временных данных функции. Добавляем выходные данные нулевым элементом.
			var tmp_array = new List<DataCell>(tmp_count) { output };

			// Добавляем входные данные.
			tmp_array.AddRange(input_data);

			int count = input_data.Count() + 1;

			// Добавляем ячейки для всех остальных команд.
			for (int i = 0; i < control_function.Commands.Count(); i++)
			{
				var callstack = function.Header.CallStack.ToList();
				callstack.Add((i + count).ToString());
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
				var callstack = function.Header.CallStack.ToList();
				callstack.Add(string.Format("{0}_{1}", command_template.OutputDataId, command_template.FunctionHeader.Name));

				var new_command = new CommandHeader
				{
					Owners = new List<Owner>(),
					CallStack = callstack,
					InputDataHeaders = command_template.InputDataIds.Select(x => (DataCellHeader)tmp_array[x].Header).ToList(),
					OutputDataHeader = (DataCellHeader)tmp_array[command_template.OutputDataId].Header,
					TriggeredCommands = command_template.TriggeredCommandIds.Select(x => command_list[x].Header).ToList(),
					FunctionHeader = command_template.FunctionHeader
				};

				//Console.WriteLine(string.Format("ControlExecutionService.Execute Callstack={0},  Function={1}", string.Join("/", function.Header.CallStack), ((FunctionHeader)function.Header).Name));
				
				Parallel.Invoke(()=> { _commandRepository.Add(new[] {new_command}); });
			}
		}
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Functions;

namespace Core.Model.Execution
{
	/// <summary>
	/// Исполняющий сервис базовых функций.
	/// </summary>
	public class BasicExecutionService : IExecutionService
	{
		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null)
		{
			var basic_function = (BasicFunction) function;
			var data_cell_list = input_data.ToList();
			switch (basic_function.Id)
			{
				case 1:
					output.Data = (int)data_cell_list[0].Data + (int)data_cell_list[1].Data;
					output.HasValue = true;
					break;
				case 2:
					output.Data = (int)data_cell_list[0].Data - (int)data_cell_list[1].Data;
					output.HasValue = true;
					break;
				case 3:
					output.Data = (int)data_cell_list[0].Data * (int)data_cell_list[1].Data;
					output.HasValue = true;
					break;
				case 4:
					output.Data = (int)data_cell_list[0].Data / (int)data_cell_list[1].Data;
					output.HasValue = true;
					break;
				case 5:
					output.Data = (int)data_cell_list[0].Data == (int)data_cell_list[1].Data;
					output.HasValue = true;
					break;
				case 6:
					output.Data = (int)data_cell_list[0].Data != (int)data_cell_list[1].Data;
					output.HasValue = true;
					break;
				case 9:
					output.Data = data_cell_list[0].Data;
					output.HasValue = true;
					break;
				case 10:
					output.Data = data_cell_list.First(x=>x.HasValue).Data;
					output.HasValue = true;
					break;
				default:
					throw new Exception(string.Format("Функция Id={0}, Name={1} не реализована.", basic_function.Id, ((FunctionHeader)basic_function.Header).Name));
			}

			//Console.WriteLine(string.Format("BasicExecutionService.Execute Callstack={0}, Function={1}. OutputData.Callstack={2}", string.Join("/", function.Header.CallStack), ((FunctionHeader)function.Header).Name, string.Join("/", output.Header.CallStack)));
		}

	}
}


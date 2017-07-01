using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model.CodeCompiler.Code;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;

namespace Core.Model.CodeExecution.Service.Execution
{
	/// <summary>
	/// Исполняющий сервис базовых функций.
	/// </summary>
	public class BasicExecutionService : IExecutionService
	{
		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null)
		{
			var func = ((BasicFunction) function).GetModel();
			var data = func.Invoke(input_data.ToArray());
			output.Data = data.Data;
			output.HasValue = data.HasValue;

			//Console.WriteLine(string.Format("BasicExecutionService.Execute Callstack={0}, Function={1}. OutputData.Callstack={2}", string.Join("/", function.Header.CallStack), ((FunctionHeader)function.Header).Name, string.Join("/", output.Header.CallStack)));
		}
	}
}


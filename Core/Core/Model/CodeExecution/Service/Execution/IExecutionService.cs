using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;

namespace Core.Model.CodeExecution.Service.Execution
{
	/// <summary>
	/// Интерфейс исполняющего сервиса.
	/// </summary>
	public interface IExecutionService
	{
		void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, Token? callstack = null);

	}
}


using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;

namespace Core.Model.CodeExecution.Service.Execution
{
	/// <summary>
	/// Интерфейс исполняющего сервиса.
	/// </summary>
	public interface IExecutionService
	{
		void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null);

	}
}


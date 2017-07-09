using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;

namespace Core.Model.CodeExecution.Service.Execution
{
	/// <summary>
	/// Сервис класса для управления исполняющими сервисами.
	/// </summary>
	public interface IExecutionManager : IExecutionService
	{
		void Execute(BasicFunction function, IEnumerable<DataCell> input_data, DataCell output, string[] callstack = null);
		void Execute(ControlFunction function, IEnumerable<DataCell> input_data, DataCell output, string[] callstack = null);
		void Execute(CSharpFunction function, IEnumerable<DataCell> input_data, DataCell output, string[] callstack = null);
	}
}

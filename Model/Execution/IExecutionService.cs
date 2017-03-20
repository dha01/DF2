using System.Collections.Generic;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;

namespace Core.Model.Execution
{
	/// <summary>
	/// Интерфейс исполняющего сервиса.
	/// </summary>
	public interface IExecutionService
	{
		void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output);

	}
}


using System.Collections.Generic;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;

namespace Core.Model.Execution
{
	/// <summary>
	/// Исполняющий сервис функций C#.
	/// </summary>
	public class CSharpExecutionService : IExecutionService
	{
		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output)
		{
			throw new System.NotImplementedException();
		}

	}
}


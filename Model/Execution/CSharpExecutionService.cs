using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Functions;

namespace Core.Model.Execution
{


	public class CSharpExecutionService : IExecutionService
	{
		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output)
		{
			throw new System.NotImplementedException();
		}

	}
}


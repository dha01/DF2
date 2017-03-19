using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Functions;

namespace Core.Model.Execution
{
	

	public interface IExecutionService
	{
		void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output);

	}
}


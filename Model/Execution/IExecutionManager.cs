using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;

namespace Core.Model.Execution
{
	public interface IExecutionManager : IExecutionService
	{
		void Execute(BasicFunction function, IEnumerable<DataCell> input_data, DataCell output);
		void Execute(ControlFunction function, IEnumerable<DataCell> input_data, DataCell output);
		void Execute(CSharpFunction function, IEnumerable<DataCell> input_data, DataCell output);
	}
}

using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeExecution.DataModel.Bodies.Functions
{
	public class CommandTemplate : Function
	{
		public FunctionHeader FunctionHeader { get; set; }
		public List<int> InputDataIds { get; set; }
		public int OutputDataId { get; set; }
		public List<int> TriggeredCommandIds { get; set; }
	}
}

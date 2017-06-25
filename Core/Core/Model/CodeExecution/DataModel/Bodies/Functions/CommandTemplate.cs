using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Headers.Functions;

namespace Core.Model.Bodies.Functions
{
	public class CommandTemplate : Function
	{
		public FunctionHeader FunctionHeader { get; set; }
		public List<int> InputDataIds { get; set; }
		public int OutputDataId { get; set; }
		public List<int> TriggeredCommandIds { get; set; }
	}
}

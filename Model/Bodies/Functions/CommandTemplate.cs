using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model.Bodies.Functions
{
	public class CommandTemplate : Function
	{
		public IEnumerable<int> InputDataIds { get; set; }
		public IEnumerable<int> TriggeredCommandIds { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Headers.Functions;

namespace Core.Model.Bodies.Functions
{
	public class ControlFunction : Function
	{
		public List<object> Constants { get; set; }  
		public IEnumerable<CommandTemplate> Commands { get; set; }
	}
}

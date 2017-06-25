﻿using System.Collections.Generic;

namespace Core.Model.CodeExecution.DataModel.Bodies.Functions
{
	public class ControlFunction : Function
	{
		public List<object> Constants { get; set; }  
		public IEnumerable<CommandTemplate> Commands { get; set; }
	}
}
﻿using System.Collections.Generic;

namespace Core.Model.CodeExecution.DataModel.Bodies.Functions
{
	/// <summary>
	/// Управляющая функция.
	/// </summary>
	public class ControlFunction : Function
	{

		/// <summary>
		/// Список констант.
		/// </summary>
		public List<object> Constants { get; set; }
		
		/// <summary>
		/// Список команд.
		/// </summary>
		public IEnumerable<CommandTemplate> Commands { get; set; }
	}
}

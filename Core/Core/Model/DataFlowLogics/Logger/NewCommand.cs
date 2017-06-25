using System;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;

namespace Core.Model.DataFlowLogics.Logger
{
	public class NewCommand
	{
		public DateTime DateTime { get; set; }
		public Command Command { get; set; }
	}
}

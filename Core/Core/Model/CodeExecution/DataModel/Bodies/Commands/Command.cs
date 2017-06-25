using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Commands;

namespace Core.Model.Bodies.Commands
{
	public class Command : ContainerBase
	{
		public Function Function { get; set; }

		public List<CommandHeader> TriggeredCommands { get; set; }

		public DataCell ConditionData { get; set; }

		public List<DataCell> InputData { get; set; }
		public DataCell OutputData { get; set; }
	}
}

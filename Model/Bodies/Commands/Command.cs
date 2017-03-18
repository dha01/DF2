using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Commands;

namespace Core.Model.Bodies.Commands
{
	public class Command
	{
		public Function Function { get; set; }

		public IEnumerable<CommandHeader> TriggeredCommands { get; set; }

		public IEnumerable<DataCell> InputData { get; set; }
		public DataCell OutputData { get; set; }
	}
}

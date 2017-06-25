using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Base;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Commands;

namespace Core.Model.CodeExecution.DataModel.Bodies.Commands
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

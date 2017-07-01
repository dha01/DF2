using System;
using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;

namespace Core.Model.DataFlowLogics.Logger
{
	public class FunctionCall
	{
		public FunctionCall Parent { get; set; } 
		
		public DateTime CallDateTime { get; set; }
		public List<string> CallStack { get; set; }

		public int Lvl { get; set; }
		public string LvlName { get; set; }

		public List<DataCell> InputDataNames { get; set; }
		public DataCell OutputDataName { get; set; }
		public string FunctionName { get; set; }

		public List<FunctionCall> Childs { get; set; }
		public Command Command { get; set; }
	}
}

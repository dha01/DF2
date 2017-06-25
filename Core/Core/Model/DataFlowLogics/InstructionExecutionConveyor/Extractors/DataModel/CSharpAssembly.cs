using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;

namespace Core.Model.DataFlowLogics.InstructionExecutionConveyor.Extractors.DataModel
{
	public class CSharpAssembly
	{
		public string Name { get; set; }
		public List<CSharpClass> CSharpClass { get; set; }
		public List<ControlFunction> ControlFunctions { get; set; }
	}
}

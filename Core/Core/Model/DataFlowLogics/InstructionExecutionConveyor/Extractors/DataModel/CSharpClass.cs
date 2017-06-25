using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;

namespace Core.Model.DataFlowLogics.InstructionExecutionConveyor.Extractors.DataModel
{
	public class CSharpClass
	{
		public string Name { get; set; }
		public List<CSharpFunction> CSharpFunction { get; set; }
	}
}

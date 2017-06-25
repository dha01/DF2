using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.Bodies.Functions;

namespace Core.Model.InstructionExecutionConveyor.Extractors.DataModel
{
	public class CSharpAssembly
	{
		public string Name { get; set; }
		public List<CSharpClass> CSharpClass { get; set; }
		public List<ControlFunction> ControlFunctions { get; set; }
	}
}

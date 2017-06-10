using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.Bodies.Functions;

namespace Core.Model.InstructionExecutionConveyor.Extractors.DataModel
{
	public class CSharpClass
	{
		public string Name { get; set; }
		public List<CSharpFunction> CSharpFunction { get; set; }
	}
}

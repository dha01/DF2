using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.Headers.Functions;

namespace Core.Model.Compiler.Build.DataModel
{
	public enum TemplateFunctionRowType
	{
		Output,
		Input,
		Func,
		Const,
		NotUse
	}

	public class TemplateFunctionRow
    {
	    public TemplateFunctionRowType Type { get; set; }

		public FunctionHeader FunctionHeader { get; set; }
		public List<TemplateFunctionRow> Input { get; set; }
	    public List<TemplateFunctionRow> Output { get; set; }
	    public List<TemplateFunctionRow> Triggered { get; set; }

		public object Value { get; set; }

		public bool IsOutput { get; set; }
	}
}

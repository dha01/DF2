using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Model.Headers.Functions;

namespace Core.Model.Bodies.Functions
{
	public class CSharpFunction : Function
	{
		public string FuncName { get; set; }
		public string ClassName { get; set; }
		public string Namespace { get; set; }

		public Assembly Assembly { get; set; }
	}
}

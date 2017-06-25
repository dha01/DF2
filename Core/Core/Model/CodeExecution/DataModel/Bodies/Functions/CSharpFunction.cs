using System.Reflection;

namespace Core.Model.CodeExecution.DataModel.Bodies.Functions
{
	public class CSharpFunction : Function
	{
		public string FuncName { get; set; }
		public string ClassName { get; set; }
		public string Namespace { get; set; }

		public Assembly Assembly { get; set; }
	}
}

using System.Linq;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;

namespace Core.Model.CodeExecution.DataModel.Headers.Functions
{
	public enum InputParamCondition
	{
		All,
		Any,
		Iif
	}

	/// <summary>
	/// Заголовок функции.
	/// </summary>
	public class FunctionHeader : InvokeHeader
	{
		public static implicit operator FunctionHeader(string name)
		{
			var split = name.Split('.');
			return CommandBuilder.BuildHeader(split.Last(), split.Take(split.Length - 1).ToList());
		}

		public string Name { get; set; }

		public InputParamCondition Condition { get; set; } = InputParamCondition.All;
	}
}


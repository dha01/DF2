using System;

namespace Core.Model.CodeCompiler.Build.Attributes
{
	/// <summary>
	/// Атрибут используемый для указания того, что предназначена для формированя управляющей функции.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ControlFunctionAttribute : Attribute
	{

	}
}

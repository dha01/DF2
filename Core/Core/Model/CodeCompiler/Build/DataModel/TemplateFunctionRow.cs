using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeCompiler.Build.DataModel
{
	/// <summary>
	/// Строка шаблона функции.
	/// </summary>
	public class TemplateFunctionRow
	{
		/// <summary>
		/// Тип строки с командой.
		/// </summary>
		public TemplateFunctionRowType Type { get; set; }

		/// <summary>
		/// Заголовок функции.
		/// </summary>
		public FunctionHeader FunctionHeader { get; set; }

		/// <summary>
		/// Входные параметры.
		/// </summary>
		public List<TemplateFunctionRow> Input { get; set; }

		/// <summary>
		/// Выходные параметры.
		/// </summary>
		public List<TemplateFunctionRow> Output { get; set; }

		/// <summary>
		/// Активируемы функции.
		/// </summary>
		public List<TemplateFunctionRow> Triggered { get; set; }

		/// <summary>
		/// Условия.
		/// </summary>
		public List<TemplateFunctionRow> Conditions { get; set; } = new List<TemplateFunctionRow>();

		/// <summary>
		/// Значение.
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Является ли выходным значением.
		/// </summary>
		public bool IsOutput { get; set; }
	}
}

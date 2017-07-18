namespace Core.Model.CodeCompiler.Build.DataModel
{
	/// <summary>
	/// Тип строки с командой.
	/// </summary>
	public enum TemplateFunctionRowType
	{

		/// <summary>
		/// Входное значение.
		/// </summary>
		Input,

		/// <summary>
		/// Возвращаемое значение.
		/// </summary>
		Output,

		/// <summary>
		/// Функция.
		/// </summary>
		Func,

		/// <summary>
		/// Константное значение.
		/// </summary>
		Const,

		/// <summary>
		/// Не используется.
		/// </summary>
		NotUse
	}
}

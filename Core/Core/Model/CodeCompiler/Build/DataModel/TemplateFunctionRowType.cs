namespace Core.Model.CodeCompiler.Build.DataModel
{
	/// <summary>
	/// Тип строки с командой.
	/// </summary>
	public enum TemplateFunctionRowType
	{
		/// <summary>
		/// Возвращаемое значение.
		/// </summary>
		Output,

		/// <summary>
		/// Входное значение.
		/// </summary>
		Input,

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

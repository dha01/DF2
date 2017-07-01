using System.Reflection;

namespace Core.Model.CodeExecution.DataModel.Bodies.Functions
{
	/// <summary>
	/// Функция C#.
	/// </summary>
	public class CSharpFunction : Function
	{
		/// <summary>
		/// Название функции.
		/// </summary>
		public string FuncName { get; set; }

		/// <summary>
		/// Название класса.
		/// </summary>
		public string ClassName { get; set; }

		/// <summary>
		/// Название пространства имен.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// Библиотека.
		/// </summary>
		public Assembly Assembly { get; set; }
	}
}

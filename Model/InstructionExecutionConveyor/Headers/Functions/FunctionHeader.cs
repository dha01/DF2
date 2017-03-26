using Core.Model.Headers.Base;

namespace Core.Model.Headers.Functions
{
	/// <summary>
	/// Заголовок функции.
	/// </summary>
	public class FunctionHeader : InvokeHeader
	{
		public string Name { get; set; }
	}
}


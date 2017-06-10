using System.Linq;
using Core.Model.Commands.Build;

namespace Core.Model.Headers.Functions
{
	/// <summary>
	/// Заголовок управляющей функции.
	/// </summary>
	public class ControlFunctionHeader : FunctionHeader
	{
		public virtual string FileName { get; set; }

		public virtual string FunctionName { get; set; }

		public virtual string Version { get; set; }

	}
}


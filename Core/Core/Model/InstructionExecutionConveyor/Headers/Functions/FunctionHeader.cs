using System.Linq;
using Core.Model.Commands.Build;
using Core.Model.Headers.Base;

namespace Core.Model.Headers.Functions
{
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
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model.Headers.Base
{
	/// <summary>
	/// Заголовок для исполнения.
	/// </summary>
	public class InvokeHeader : Header
	{
		public virtual IEnumerable<string> CallStack { get; set; }

		public string CallstackToString(string separator = "/")
		{
			return string.Join(separator, CallStack);
		}

		public bool Equals(InvokeHeader obj)
		{
			return CallStack.SequenceEqual(obj.CallStack);
		}
	}
}


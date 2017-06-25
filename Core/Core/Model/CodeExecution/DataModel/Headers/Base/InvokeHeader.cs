using System.Collections.Generic;
using System.Linq;

namespace Core.Model.CodeExecution.DataModel.Headers.Base
{
	/// <summary>
	/// Заголовок для исполнения.
	/// </summary>
	public class InvokeHeader : Header
	{
		public virtual IEnumerable<string> CallStack { get; set; }

		private string _token;

		public string Token
		{
			get
			{
				if (string.IsNullOrEmpty(_token))
				{
					_token = CallstackToString();
				}
				return _token;
			}
			set { _token = value; }
		}


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


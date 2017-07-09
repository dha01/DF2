using System.Collections.Generic;
using System.Linq;

namespace Core.Model.CodeExecution.DataModel.Headers.Base
{
	/// <summary>
	/// Заголовок для исполнения.
	/// </summary>
	public class InvokeHeader : Header
	{
		public virtual string[] CallStack { get; set; }

		protected string _token;

		public virtual string Token
		{
			get
			{
				if (string.IsNullOrEmpty(_token))
				{
					_token = string.Join("/", CallStack);
				}
				return _token;
			}
		}

		public bool Equals(InvokeHeader obj)
		{
			return CallStack.SequenceEqual(obj.CallStack);
		}
	}
}


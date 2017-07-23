using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Model.CodeExecution.DataModel.Headers.Base
{

	/// <summary>
	/// Заголовок для исполнения.
	/// </summary>
	public class InvokeHeader : Header
	{
		public virtual Token Token { get; set; }
	}
}


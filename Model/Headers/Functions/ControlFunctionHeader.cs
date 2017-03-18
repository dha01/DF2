using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model.Headers.Functions
{
	

	public class ControlFunctionHeader : FunctionHeader
	{
		public virtual string FileName { get; set; }

		public virtual string FunctionName { get; set; }

		public virtual string Version { get; set; }

	}
}


using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Core.Model.Headers.Base
{
	

	public class Header
	{
		public virtual IEnumerable<Owner> Owners { get; set; }

	}
}


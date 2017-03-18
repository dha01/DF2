using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Headers.Base;

namespace Core.Model.Headers.Data
{
	

	public class DataCellHeader : InvokeHeader
	{
		public virtual Dictionary<Owner, bool> HasValue { get; set; }

	}
}


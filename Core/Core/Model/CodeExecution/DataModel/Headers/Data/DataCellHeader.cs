using System.Collections.Generic;
using Core.Model.Headers.Base;

namespace Core.Model.Headers.Data
{
	/// <summary>
	/// Заголовок ячейки данных.
	/// </summary>
	public class DataCellHeader : InvokeHeader
	{
		public virtual Dictionary<Owner, bool> HasValue { get; set; }
	}
}


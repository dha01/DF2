using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.NetworkLogic;

namespace Core.Model.CodeExecution.DataModel.Headers.Data
{
	/// <summary>
	/// Заголовок ячейки данных.
	/// </summary>
	public class DataCellHeader : InvokeHeader
	{
		public virtual Dictionary<Owner, bool> HasValue { get; set; }
	}
}


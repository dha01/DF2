using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;

namespace Core.Model.Repository
{
	/// <summary>
	/// Репозиторий ячеек данных.
	/// </summary>
	public class DataCellRepository : ContainerRepositoryBase<DataCell, DataCellHeader>, IDataCellRepository
	{
		protected override void AddConteiner(DataCell conteiner)
		{
			var key = string.Join("/", conteiner.Header.CallStack);
			if (_items.ContainsKey(key))
			{
				var item = _items[key];
				item.Header.AddOwners(conteiner.Header.Owners);
				item.Data = conteiner.Data;
				item.HasValue = conteiner.HasValue;
			}
			else
			{
				_items[key] = conteiner;
			}
		}

		protected override bool IsItemExists(string key)
		{
			return base.IsItemExists(key) && _items[key].HasValue;
		}

		protected override void AddHeader(DataCellHeader header)
		{
			var key = string.Join("/", header.CallStack);
			if (_itemHeaders.ContainsKey(key))
			{
				_itemHeaders[key]. AddOwners(header.Owners);
			}
			else
			{
				_itemHeaders[key] = header;
			}
		}
	}
}

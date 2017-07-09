using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Headers.Data;

namespace Core.Model.CodeExecution.Repository
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
			var item = _items[key];
			return base.IsItemExists(key) && item.HasValue.HasValue && item.HasValue.Value;
		}

		protected override void AddHeader(DataCellHeader header)
		{
			var key = string.Join("/", header.CallStack);
			if (_itemHeaders.ContainsKey(key))
			{
				//_itemHeaders[key]. AddOwners(header.Owners);
			}
			else
			{
				_itemHeaders[key] = header;
			}
		}
	}
}

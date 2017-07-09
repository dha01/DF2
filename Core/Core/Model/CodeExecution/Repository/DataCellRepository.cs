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
			if (_items.ContainsKey(conteiner.Token))
			{
				var item = _items[conteiner.Token];
				item.Data = conteiner.Data;
				item.HasValue = conteiner.HasValue;
			}
			else
			{
				_items[conteiner.Token] = conteiner;
			}
		}

		protected override bool IsItemExists(string key)
		{
			var item = _items[key];
			return base.IsItemExists(key) && item.HasValue.HasValue && item.HasValue.Value;
		}

		protected override void AddHeader(DataCellHeader header)
		{
			if (_itemHeaders.ContainsKey(header.Token))
			{
				//_itemHeaders[key]. AddOwners(header.Owners);
			}
			else
			{
				_itemHeaders[header.Token] = header;
			}
		}
	}
}

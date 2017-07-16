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
			var item = _itemsTree.Get(conteiner.Token.ToEnumerable());
			if (/*_items.ContainsKey(conteiner.Token)*/ item != null)
			{
				//var item = _itemsTree.Get(conteiner.Token.Split('/')); //_items[conteiner.Token];
				item.Data = conteiner.Data;
				item.HasValue = conteiner.HasValue;
			}
			else
			{
				//_items[conteiner.Token] = conteiner;
				_itemsTree.Add(conteiner);
			}
		}

		protected override bool IsItemExists(string key)
		{
			if (!base.IsItemExists(key))
			{
				return false;
			}

			var item = _itemsTree.Get(key.Split('/'));// _items[key];
			return item.HasValue.HasValue && item.HasValue.Value;
		}

		/*protected override void AddHeader(DataCellHeader header)
		{
			if (_itemHeaders.ContainsKey(header.Token))
			{
				//_itemHeaders[key]. AddOwners(header.Owners);
			}
			else
			{
				_itemHeaders[header.Token] = header;
			}
		}*/
	}
}

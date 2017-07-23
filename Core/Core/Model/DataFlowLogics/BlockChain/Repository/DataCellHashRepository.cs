using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Repository
{
    public class DataCellHashRepository : IDataCellHashRepository
    {
	    private readonly ConcurrentDictionary<string, DataCellHash> _dataCells = new ConcurrentDictionary<string, DataCellHash>();

		public IEnumerable<DataCellHash> Get(params string[] hashs)
		{
			return hashs.Select(x => x != null && _dataCells.TryGetValue(x, out DataCellHash val) ? val : null);
		}

	    public void Set(params DataCellHash[] data_cells)
	    {
		    foreach (var data_cell in data_cells)
		    {
			    if (_dataCells.TryGetValue(data_cell.Hash, out DataCellHash val))
			    {
				    _dataCells.TryUpdate(data_cell.Hash, data_cell, val);
			    }
			    else
			    {
				    _dataCells.TryAdd(data_cell.Hash, data_cell);
			    }
		    }
	    }

	    public void Remove(params string[] data_cell_hashs)
	    {
		    foreach (var data_cell_hash in data_cell_hashs)
		    {
			    _dataCells.TryRemove(data_cell_hash, out DataCellHash val);
		    }
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		    if (data_cells.Any(
			    x => x.Hash == "3aRuH6jAgZZLJib4Hu/V8A3/DY9GZVZ9dkQMiQyNPh50PYi4hYfJShf1m5gNKnzwwDOPS4IwEANOKLsHvP91Ag=="))
		    {
			    var f = 5;
		    }
			foreach (var data_cell in data_cells)
		    {
			    if (!_dataCells.TryAdd(data_cell.Hash, data_cell))
			    {
				    if (!_dataCells.TryGetValue(data_cell.Hash, out DataCellHash val))
				    {
					    throw new NotImplementedException();
				    }/*
				    if (_dataCells.TryUpdate(data_cell.Hash, data_cell, val))
				    {
						throw new NotImplementedException();
					}*/
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

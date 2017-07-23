using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Repository
{
    public interface IDataCellHashRepository
	{
		IEnumerable<DataCellHash> Get(params string[] hashs);

		void Set(params DataCellHash[] data_cells);
		void Remove(params string[] data_cell_hashs);
	}
}

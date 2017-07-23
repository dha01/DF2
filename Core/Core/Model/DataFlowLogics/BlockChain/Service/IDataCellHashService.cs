using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
    public interface IDataCellHashService
    {
	    IEnumerable<DataCellHash> GetLocal(params string[] data_cell_hashs);
	    IEnumerable<DataCellHash> GetGlobal(params string[] data_cell_hashs);
		void Set(params DataCellHash[] data_cells);
	    void Remove(params string[] data_cell_hashs);
    }
}

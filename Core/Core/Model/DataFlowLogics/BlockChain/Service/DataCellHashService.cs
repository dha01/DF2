using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.DataFlowLogics.BlockChain.DataModel;
using Core.Model.DataFlowLogics.BlockChain.Repository;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
    public class DataCellHashService : IDataCellHashService
	{
		private readonly IDataCellHashRepository _dataCellHashRepository;

		public DataCellHashService(IDataCellHashRepository data_cell_hash_repository)
		{
			_dataCellHashRepository = data_cell_hash_repository;

		}

		public IEnumerable<DataCellHash> GetLocal(params string[] data_cell_hashs)
		{
			return _dataCellHashRepository.Get(data_cell_hashs);
		}

		public IEnumerable<DataCellHash> GetGlobal(params string[] data_cell_hashs)
		{

			throw new NotImplementedException();
		}

		public void Set(params DataCellHash[] data_cell_hashs)
		{
			_dataCellHashRepository.Set(data_cell_hashs);
		}

		public void Remove(params string[] data_cells)
		{
			_dataCellHashRepository.Remove(data_cells);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
	public interface ITransactionPoolService
	{
		long QueueLength { get; }
		long Raito { get; }
		long EnqueueSpeed { get; }

		Action OnAddToPreparation { get; set; }


		void DeleteFromPool(string transaction_hash);

		void AddToPool(Transaction transaction);

		void UpdateToPool(Transaction transaction);

		Task<string> GetResultHash(string transaction_hash, int timeout = -1);

		bool TryGetFromPool(string transaction_hash, out Transaction transaction);

		void EnqueueToPreparation(int priotiry, params Transaction[] transactions);

		bool TryDequeueToPreparation(out Transaction transaction);
	}
}

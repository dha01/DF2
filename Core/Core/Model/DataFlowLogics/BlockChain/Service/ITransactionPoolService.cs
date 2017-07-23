using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
	public interface ITransactionPoolService
	{
		void AddToPool(Transaction transaction);

		void UpdateToPool(Transaction transaction);

		bool TryGetFromPool(string transaction_hash, out Transaction transaction);

		void EnqueueToPreparation(int priotiry, params Transaction[] transactions);

		bool TryDequeueToPreparation(out Transaction transaction);
	}
}

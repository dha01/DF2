using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
	public class TransactionPoolService : ITransactionPoolService
	{
		private readonly Dictionary<int, ConcurrentQueue<Transaction>> _concurrentQueue = new Dictionary<int, ConcurrentQueue<Transaction>>()
		{
			{ 0, new ConcurrentQueue<Transaction>() },
			{ 1, new ConcurrentQueue<Transaction>() },
			{ 2, new ConcurrentQueue<Transaction>() }
		};

		private readonly ConcurrentDictionary<string, Transaction> _transactionsPool = new ConcurrentDictionary<string, Transaction>();

		public void AddToPool(Transaction transaction)
		{
			_transactionsPool.TryAdd(transaction.Hash, transaction);
		}

		public void UpdateToPool(Transaction transaction)
		{
			if (_transactionsPool.TryGetValue(transaction.Hash, out Transaction exists_transaction))
			{
				_transactionsPool.TryUpdate(transaction.Hash, transaction, exists_transaction);
			}
		}

		public bool TryGetFromPool(string transaction_hash, out Transaction transaction)
		{
			return _transactionsPool.TryGetValue(transaction_hash, out transaction);
		}

		public void EnqueueToPreparation(int priotiry, params Transaction[] transactions)
		{
			foreach (var transaction in transactions)
			{
				_concurrentQueue[priotiry].Enqueue(transaction);
			}
		}

		public bool TryDequeueToPreparation(out Transaction transaction)
		{
			Transaction transaction_in = null;
			var result = _concurrentQueue.Values.Any(x => x.TryDequeue(out transaction_in));
			transaction = transaction_in;
			return result;
		}
	}
}

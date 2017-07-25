using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
	public class TransactionPoolService : ITransactionPoolService
	{
		private SpeedTest _enqueueSpeedTest = new SpeedTest(100);
		private SpeedTest _dequeueSpeedTest = new SpeedTest(100);

		public long Raito
		{
			get { return _enqueueSpeedTest.GetSpeed() - _dequeueSpeedTest.GetSpeed(); }
		}

		public long EnqueueSpeed
		{
			get { return _enqueueSpeedTest.GetSpeed(); }
		}

		private long _queueLength = 0;

		public long QueueLength => Interlocked.Read(ref _queueLength);

		private readonly Dictionary<int, ConcurrentQueue<Transaction>> _concurrentQueue = new Dictionary<int, ConcurrentQueue<Transaction>>()
		{
			{ 0, new ConcurrentQueue<Transaction>() },
			{ 1, new ConcurrentQueue<Transaction>() },
			{ 2, new ConcurrentQueue<Transaction>() }
		};

		private readonly ConcurrentDictionary<string, Transaction> _transactionsPool = new ConcurrentDictionary<string, Transaction>();

		public Action OnAddToPreparation { get; set; } = null;

		public void DeleteFromPool(string transaction_hash)
		{
			_transactionsPool.TryRemove(transaction_hash, out Transaction transaction);
		}

		public void AddToPool(Transaction transaction)
		{
			if (_transactionsPool.TryAdd(transaction.Hash, transaction))
			{

			}
		}

		public void UpdateToPool(Transaction transaction)
		{
			if (_transactionsPool.TryGetValue(transaction.Hash, out Transaction exists_transaction))
			{
				if (_transactionsPool.TryUpdate(transaction.Hash, transaction, exists_transaction))
				{

				}
			}
		}

		private readonly ConcurrentDictionary<string, ManualResetEvent> _waitResults = new ConcurrentDictionary<string, ManualResetEvent>();

		public void CalculationComplite(string transaction_hash)
		{
			if(_waitResults.TryGetValue(transaction_hash, out ManualResetEvent manual_reset_event))
			{
				manual_reset_event.Set();
			}
		}

		public string GetResultHash(string transaction_hash, int timeout = -1)
		{
			var mre = new ManualResetEvent(false);
			_waitResults.TryAdd(transaction_hash, mre);
			mre.WaitOne(timeout);
			_transactionsPool.TryGetValue(transaction_hash, out Transaction got_transaction);

			var result = ((ExecutionTransaction) got_transaction)?.Temps?[0];

			if (result == null)
			{
				throw new NotImplementedException();
			}
			return result;
		}

		public bool TryGetFromPool(string transaction_hash, out Transaction transaction)
		{
			var result = _transactionsPool.TryGetValue(transaction_hash, out Transaction got_transaction);
			transaction = got_transaction?.Clone();
			return result;
		}
		
		public void EnqueueToPreparation(int priotiry, params Transaction[] transactions)
		{
			foreach (var transaction in transactions.OrderBy(x=>x.Hash))
			{
				_concurrentQueue[priotiry].Enqueue(transaction);
				Interlocked.Increment(ref _queueLength);
				_enqueueSpeedTest.Incremental();
			}
			OnAddToPreparation.Invoke();
		}

		public bool TryDequeueToPreparation(out Transaction transaction)
		{
			Transaction transaction_in = null;
			var result = _concurrentQueue.Values.Any(x => x.TryDequeue(out transaction_in));

			if (result)
			{
				Interlocked.Decrement(ref _queueLength);
				_dequeueSpeedTest.Incremental();
			}
			transaction = transaction_in;

			return result;
		}
	}
}

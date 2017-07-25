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
		/*
		private readonly ConcurrentQueue<Transaction> _concurrentQueueZero = new ConcurrentQueue<Transaction>();
		private readonly ConcurrentQueue<Transaction> _concurrentQueueFirst = new ConcurrentQueue<Transaction>();
		private readonly ConcurrentQueue<Transaction> _concurrentQueueSecond = new ConcurrentQueue<Transaction>();*/

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
				/*if (_waitResults.TryGetValue(transaction.Hash, out ManualResetEvent waiting))
				{
					if (((ExecutionTransaction)transaction)?.Temps?[0] != null)
					{
						waiting.Set();
					}
				}*/
			}
		}

		public void UpdateToPool(Transaction transaction)
		{
			if (_transactionsPool.TryGetValue(transaction.Hash, out Transaction exists_transaction))
			{
				if (_transactionsPool.TryUpdate(transaction.Hash, transaction, exists_transaction))
				{
					/*if (_waitResults.TryGetValue(transaction.Hash, out ManualResetEvent waiting))
					{
						if (((ExecutionTransaction)transaction)?.Temps?[0] != null)
						{
							waiting.Set();
						}
					}*/
				}
			}
		}

		private readonly ConcurrentDictionary<string, ManualResetEvent> _waitResults = new ConcurrentDictionary<string, ManualResetEvent>();

		public async Task<string> GetResultHash(string transaction_hash, int timeout = -1)
		{
			return await Task.Factory.StartNew(() =>
			{
				_transactionsPool.TryGetValue(transaction_hash, out Transaction got_transaction);

				while (((ExecutionTransaction)got_transaction)?.Temps?[0] == null)
				{
					Thread.Sleep(1000);
					
					_transactionsPool.TryGetValue(transaction_hash, out got_transaction);
				}/*

				if (((ExecutionTransaction)got_transaction)?.Temps?[0] == null)
				{
					var mre = new ManualResetEvent(false);

					if (_waitResults.TryAdd(transaction_hash, mre))
					{

					}
					else
					{
						if (!_waitResults.TryGetValue(transaction_hash, out mre))
						{
							throw new NotImplementedException("DataCellHashRepository.Get");
						}
					}
					mre.WaitOne(timeout);
				}
				_transactionsPool.TryGetValue(transaction_hash, out got_transaction);*/
				return ((ExecutionTransaction)got_transaction)?.Temps?[0];
			});
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
			/*var result = _concurrentQueueZero.TryDequeue(out transaction_in) ||
			             _concurrentQueueFirst.TryDequeue(out transaction_in) ||
			             _concurrentQueueSecond.TryDequeue(out transaction_in);*/

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

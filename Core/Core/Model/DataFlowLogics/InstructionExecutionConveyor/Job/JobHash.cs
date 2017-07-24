using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.CodeExecution.Service.Execution;
using Core.Model.DataFlowLogics.BlockChain.DataModel;
using Core.Model.DataFlowLogics.BlockChain.Service;
using Core.Model.DataFlowLogics.Logger;

namespace Core.Model.DataFlowLogics.InstructionExecutionConveyor.Job
{
	/// <summary>
	/// Исполнитель.
	/// </summary>
	public class JobHash
	{
		public int Id { get; set; }

		private long _queueLength = 0;

		public long QueueLength
		{
			get { return _queueLength; }
		}

		public Action<Command> Callback { get; set; }


		private readonly ITransactionExecutorService _transactionExecutorService;
		private readonly ITransactionPoolService _transactionPoolService;

		//private Task _currentTask;

		private readonly ManualResetEvent _eventReset = new ManualResetEvent(false);

		private readonly Object _objectLock = new Object();
		public JobHash(ITransactionExecutorService transaction_executor_service, ITransactionPoolService transaction_pool_service)
		{
			_transactionExecutorService = transaction_executor_service;
			_transactionPoolService = transaction_pool_service;

			Task.Factory.StartNew(Invoke, TaskCreationOptions.AttachedToParent);

			_speedTest = new SpeedTest();
		}


		private SpeedTest _speedTest;

		private bool _isWork = false;

		public void Strart()
		{
			/*lock (_objectLock)
			{*/
				if(!_isWork)
				{
					_eventReset.Set();
					_isWork = true;
				}
			/*}*/
		}

		public void Stop()
		{
			_eventReset.Reset();
			/*lock (_objectLock)
			{*/
				_isWork = false;
			/*}*/
			_eventReset.WaitOne();
		}

		public bool IsWork
		{
			get
			{
				/*lock (_objectLock)
				{*/
					return _isWork;
				/*}*/
			}
		}

		/*
		public void AddCommand(int priority, Transaction transaction)
		{
			_transactionPoolService.EnqueueToPreparation(priority, transaction);
			lock (_objectLock)
			{
				Interlocked.Increment(ref _queueLength);
				_eventReset.Set();
			}
		}*/

		private void Invoke()
		{
			while (true)
			{
				Stop();
				while (_transactionPoolService.TryDequeueToPreparation(out Transaction transaction))
				{
					try
					{
						_transactionExecutorService.Execute(transaction.Clone());
						_speedTest.Incremental();
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
			}
		}
	}
}


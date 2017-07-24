using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Core.Model.DataFlowLogics.BlockChain.Service;

namespace Core.Model.DataFlowLogics.InstructionExecutionConveyor.Job
{
	public class JobHashManager
	{
		private const int MAX_JOB_COUNT = 8;

		private List<JobHash> _jobs = new List<JobHash>();

		private ITransactionPoolService _transactionPoolService;
		private ITransactionExecutorService _transactionExecutorService;
		public JobHashManager(ITransactionPoolService transaction_pool_service, ITransactionExecutorService transaction_executor_service)
		{
			_transactionPoolService = transaction_pool_service;
			_transactionExecutorService = transaction_executor_service;

			for (int i = 0; i < MAX_JOB_COUNT; i++)
			{
				var new_job = new JobHash(_transactionExecutorService, _transactionPoolService)
				{
					Id = i
				};

				_jobs.Add(new_job);
			}

			_transactionPoolService.OnAddToPreparation = OnNewTransaction;
		}

		private long max = 0;
		private long min = 9999999;
		public void OnNewTransaction()
		{
			  var work_count = _jobs.Count(x => x.IsWork);
			var ql = _transactionPoolService.Raito;

			if (max < ql)
			{
				max = ql;
			}

			if (min > ql)
			{
				min = ql;
			}

			if (work_count < MAX_JOB_COUNT && _transactionPoolService.QueueLength > work_count)
			{
				_jobs.FirstOrDefault(x => !x.IsWork)?.Strart();
			}
		}
	}
}

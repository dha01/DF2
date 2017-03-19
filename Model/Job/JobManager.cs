using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Execution;
using Core.Model.Headers.Commands;
using Core.Model.Repository;

namespace Core.Model.Job
{
	public class JobManager : IJobManager
	{
		private const int MAX_JOB_COUNT = 1;

		private ICommandRepository _commandRepository;

		private IEnumerable<Job> _jobs;

		private Action<Command> _onReliseJob;

		private IExecutionManager _executionManager;

		public Action<Command> OnReliseJob
		{
			get { return _onReliseJob; }
			set
			{
				_onReliseJob = value;
				foreach (var job in _jobs)
				{
					job.Callback = value;
				}
			}
		}

		public virtual object ExecutionServiceFactory { get; set; }

		public JobManager(IExecutionManager execution_manager)
		{
			_jobs = new List<Job>() { new Job(execution_manager) };
		}

		public bool HasFreeJob()
		{
			return true;
		}

		/// <summary>
		/// Добавляет команду в очередь на исполнение.
		/// </summary>
		/// <param name="command"></param>
		public virtual void AddCommand(Command command)
		{
			_jobs.OrderBy(x => x.QueueLength).First().AddCommand(command);
		}

		/// <summary>
		/// Возвращает количество свободных исполнителей.
		/// </summary>
		/// <returns></returns>
		public int GetFreeJobCount()
		{
			return _jobs.Count(x => x.QueueLength == 0);
		}
	}
}


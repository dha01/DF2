using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Execution;
using Core.Model.Headers.Commands;
using Core.Model.Repository;

namespace Core.Model.Job
{
	

	public class Job
	{
		public int QueueLength
		{
			get { return CommandQueue.Count(); }
		}

		public Action<Command> Callback;
		
		private ICommandRepository _commandRepository;
		private Queue<Command> CommandQueue { get; set; }


		private IExecutionManager _executionManager;

		private Task _currentTask;

		private ManualResetEvent _eventReset = new ManualResetEvent(false);

		public Job(IExecutionManager execution_manager)
		{
			CommandQueue = new Queue<Command>();
			_executionManager = execution_manager;
		}

		public void Start()
		{
			if (_currentTask == null)
			{
				new Task(() =>
				{
					while (true)
					{
						_eventReset.WaitOne();
						Invoke();
						_eventReset.Reset();
						Invoke();
					}
				}).Start();
			}
		}

		private void Invoke()
		{
			while (CommandQueue.Count > 0)
			{
			/*	try
				{*/
					var command = CommandQueue.Dequeue();
					_executionManager.Execute(command.Function, command.InputData, command.OutputData);
					/*command.OutputData.Data
					command.OutputData.HasValue = true;*/
					//invoke
					Callback(command);
				/*}
				catch (Exception)
				{
					
					throw;
				}*/
			}
		}

		public void AddCommand(Command command)
		{
			CommandQueue.Enqueue(command);
			_eventReset.Set();
			Start();
		}
	}
}


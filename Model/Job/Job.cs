using System;
using System.Collections.Concurrent;
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
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.Repository;

namespace Core.Model.Job
{
	
	/// <summary>
	/// Исполнитель.
	/// </summary>
	public class Job
	{
		public int Id { get; set; }

		private int _queueLength = 0;

		public int QueueLength
		{
			get { return _queueLength; }
		}

		public Action<Command> Callback;
		
		private ICommandRepository _commandRepository;
		private ConcurrentQueue<Command> CommandQueue { get; set; }


		private IExecutionManager _executionManager;

		private Task _currentTask;

		private ManualResetEvent _eventReset = new ManualResetEvent(false);

		public Job(IExecutionManager execution_manager)
		{
			CommandQueue = new ConcurrentQueue<Command>();
			_executionManager = execution_manager;
		}

		public void Start()
		{
			if (_currentTask == null)
			{
				_currentTask = new Task(() =>
				{
					while (true)
					{
					//	_eventReset.WaitOne();
						Invoke();
					/*_eventReset.Reset();
						Invoke();*/

						
					}
				});

				_currentTask.Start();
			}
		}

		private void Invoke()
		{
			//Console.WriteLine("Job.Invoke1");

			while (CommandQueue.Count > 0)
			{
				//Console.WriteLine("Job.Invoke2 CommandQueue.Count={0}", CommandQueue.Count);
			/*	try
				{*/
					Command command;
					CommandQueue.TryDequeue(out command);
					Console.WriteLine(string.Format("{2} Job.Invoke {0} начал выполнять функцию {1}", Id, string.Join("/", ((DataCellHeader)command.OutputData.Header).CallStack), DateTime.Now));
				
					_executionManager.Execute(command.Function, command.InputData, command.OutputData);
					Interlocked.Decrement(ref _queueLength);
					/*command.OutputData.Data
					command.OutputData.HasValue = true;*/
					//invoke
					Console.WriteLine(string.Format("{2} Job.Invoke {0} выполнил функцию {1}", Id, string.Join("/", ((DataCellHeader)command.OutputData.Header).CallStack), DateTime.Now));

					Parallel.Invoke(()=> { Callback(command); });
				/*}
				catch (Exception)
				{
					
					throw;
				}*/
				//	Console.WriteLine("Job.Invoke");
			}
		}

		public void AddCommand(Command command)
		{
			Interlocked.Increment(ref _queueLength);
			CommandQueue.Enqueue(command);
			_eventReset.Set();
			Start();
		}
	}
}


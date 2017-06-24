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
using Core.Model.Commands.Logger;
using Core.Model.Execution;
using Core.Model.Headers.Base;
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

		private long _queueLength = 0;

		public long QueueLength
		{
			get { return _queueLength; }
		}

		public Action<Command> Callback;
		
		//private ICommandRepository _commandRepository;
		private ConcurrentQueue<Command> CommandQueue { get; set; }


		private IExecutionManager _executionManager;

		//private Task _currentTask;

		private ManualResetEvent _eventReset = new ManualResetEvent(false);

		Object object_lock = new Object();
		public Job(IExecutionManager execution_manager)
		{
			CommandQueue = new ConcurrentQueue<Command>();
			_executionManager = execution_manager;

			Task.Factory.StartNew(Invoke, TaskCreationOptions.AttachedToParent);
		}

		public void AddCommand(Command command)
		{
			CommandQueue.Enqueue(command);
			lock (object_lock)
			{
				Interlocked.Increment(ref _queueLength);
				_eventReset.Set();
			}
		}

		private void Invoke()
		{
			while (true)
			{
				_eventReset.WaitOne();
				lock (object_lock)
				{
					if (Interlocked.Read(ref _queueLength) > 0)
					{
						Interlocked.Decrement(ref _queueLength);
					}
					else
					{
						_eventReset.Reset();
						continue;
					}
				}
				
				try
				{
					CommandQueue.TryDequeue(out Command command);
					//Console.WriteLine(string.Format("{2} Job.Invoke {0} начал выполнять функцию {1}", Id, string.Join("/", ((DataCellHeader)command.OutputData.Header).CallStack), DateTime.Now));
					//Parallel.Invoke(() => { Parallel.Invoke(() => { Parallel.Invoke(() => { throw new Exception("!"); }); }); });


					_executionManager.Execute(command.Function, command.InputData, command.OutputData, new CommandContext() { Header = new InvokeHeader() { CallStack = command.Header.CallStack} });

				//	Interlocked.Decrement(ref _queueLength);
					/*command.OutputData.Data
					command.OutputData.HasValue = true;*/
					//invoke

					StackTraceLogger.Write(command);

					string str = "";

					str += command.Header.CallstackToString() + " : ";
					str += command.Function.GetHeader<FunctionHeader>().CallstackToString(".") + "(";

					str += string.Join(", ", command.InputData.Select(x => x.GetHeader<DataCellHeader>().CallstackToString()));

					str += ")";

					str += " -> " + command.OutputData.Header.CallstackToString();
					Console.WriteLine(str);

					//Console.WriteLine(string.Format("{2} Job.Invoke {0} выполнил функцию {1}",  Id, string.Join("/", ((DataCellHeader)command.OutputData.Header).CallStack), DateTime.Now));
					
					//Console.WriteLine(string.Format("{2} Job.Invoke {0} выполнил функцию {1}", Id, string.Join("/", ((DataCellHeader)command.OutputData.Header).CallStack), DateTime.Now));

					Parallel.Invoke(()=> { Callback(command); });
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
				//	Console.WriteLine("Job.Invoke");
			}
		}
	}
}


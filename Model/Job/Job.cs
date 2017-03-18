using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Functions;
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


		private Task _currentTask;

		private ManualResetEvent _eventReset = new ManualResetEvent(false);

		public Job()
		{
			CommandQueue = new Queue<Command>();
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
				var command = CommandQueue.Dequeue();

				switch (((BasicFunction) command.Function).Id)
				{
					case 1:
						command.OutputData.Data = (int)command.InputData.ToArray()[0].Data + (int)command.InputData.ToArray()[1].Data;
						break;
					case 2:
						command.OutputData.Data = (int)command.InputData.ToArray()[0].Data * (int)command.InputData.ToArray()[1].Data;
						break;
				}
				command.OutputData.HasValue = true;
				//invoke
				Callback(command);
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


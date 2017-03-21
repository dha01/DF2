using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.Bodies.Commands;
using Core.Model.Headers.Base;
using Core.Model.Headers.Functions;

namespace Core.Model.Commands.Logger
{
	public static class StackTraceLogger
	{
		private static ConcurrentQueue<NewCommand> _writeQueue = new ConcurrentQueue<NewCommand>();
		
		private static FunctionCall zeroLvl = new FunctionCall()
		{
			Lvl = 0,
			CallStack = new List<string>(),
			LvlName = "",
			Childs = new List<FunctionCall>(),
			Parent = null,
			FunctionName = ""
		};

		public static FunctionCall GetLog()
		{
			return zeroLvl;
		}

		private static ManualResetEvent _eventReset = new ManualResetEvent(false);

		public static void Write(Command command)
		{
			_writeQueue.Enqueue(new NewCommand()
			{
				DateTime = DateTime.Now,
				Command = command
			});
			_eventReset.Set();
		}

		private static readonly Task _currentTask;

		static StackTraceLogger()
		{
			_currentTask = new Task(() =>
			{
				while (true)
				{
					_eventReset.WaitOne();
					Invoke();
					_eventReset.Reset();
					Invoke();
				}
			});
			_currentTask.Start();
		}

		private static void Invoke()
		{
			NewCommand new_command;
			if (_writeQueue.TryDequeue(out new_command))
			{
				Write(new_command);
			}
		}

		private static int index = 0;

		public static void Wait()
		{
			
		}

		private static void Write(NewCommand new_command, FunctionCall current = null)
		{
			var command = new_command.Command;
			current = current ?? zeroLvl;

			if (current.Lvl + 1 == command.Header.CallStack.Count())
			{
				/*var call_stack = command.OutputData.Header.CallStack.Take(current.Lvl).ToList();
				call_stack.Add(command.Function.GetHeader<FunctionHeader>().Name);*/
				/*current.InputDataNames = command.InputData.Select(x => x.Header.CallStack.Last()).ToList();
				current.OutputDataName = command.OutputData.Header.CallStack.Last();
				current.FunctionName = command.Function.GetHeader<FunctionHeader>().CallstackToString(".");*/

				var new_child = new FunctionCall()
				{
					Lvl = current.Lvl + 1,
					CallStack = command.GetHeader<InvokeHeader>().CallStack.ToList(),
					InputDataNames = command.InputData.Select(x => x.Header.CallStack.Last()).ToList(),
					OutputDataName = command.OutputData.Header.CallStack.Last(),
					FunctionName = command.Function.GetHeader<FunctionHeader>().CallstackToString("."),
					LvlName = command.GetHeader<InvokeHeader>().CallStack.Last(),
					Childs = new List<FunctionCall>()
				};

				current.Childs.Add(new_child);
				/*
				if (current.LvlName.Equals("Process1"))
				{
					var x = 2;
				}*/
				var i = index;
				Interlocked.Increment(ref index);
				Console.WriteLine("!!!!! White {0}", i);
				Console.WriteLine("!!!!! LvlName={0}, FunctionName={1}", new_child.LvlName, new_child.FunctionName);
				Console.WriteLine("!!!!! Callstack={0}", command.Header.CallstackToString("/"));
				//Console.WriteLine("!!!!! Callstack={0}", string.Join("/", call_stack));
				return;
			}

			
			var lvl_name = command.Header.CallStack.Skip(current.Lvl).Take(1).FirstOrDefault();
			var exists = current.Childs.FirstOrDefault(x => x.LvlName.Equals(lvl_name));

			if (exists == null)
			{
				exists = new FunctionCall()
				{
					CallDateTime = new_command.DateTime,
					Lvl = current.Lvl + 1,
					CallStack = new List<string>(),
					LvlName = lvl_name,
					Childs = new List<FunctionCall>(),
					Parent = null,
					FunctionName = ""
				};
				current.Childs.Add(exists);

				
			}

			Write(new_command, exists);
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.DataFlowLogics.Logger
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


		private static string PresentFuncName(string name)
		{
			return name.Split('.').Last().Replace("<", "_").Replace(">", "");
		}

		public static string GetLogScheme(FunctionCall function_call = null)
		{
			var result = function_call == null ? "digraph Diagram { \n" : "";
						
			var function = function_call ?? zeroLvl;
			if(function.Childs != null && function.Childs.Any())
			{
				result += string.Format("subgraph cluster_{0} {{ label = \"{0}\"; \n", PresentFuncName(function.LvlName));
			}
			

			if (function.Childs == null || !function.Childs.Any())
			{
				if (function.InputDataNames != null)
				{
					foreach (var name in function.InputDataNames)
					{
						result += string.Format("{1} -> {0};\n", PresentFuncName(function.LvlName), name);
					}
				}

				result += string.Format("{0} -> {1};\n", PresentFuncName(function.LvlName), function.OutputDataName);
			}
			else
			{
				foreach (var child in function.Childs )
				{
					result += GetLogScheme(child) + "\n";
				}
			}
			if (function.Childs != null && function.Childs.Any())
			{
				result += "}\n";
			}
			if (function_call == null)
			{
				result += "}\n";
			}
			return result;
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



		static StackTraceLogger()
		{
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					//_eventReset.WaitOne();
					Invoke(); /*
					_eventReset.Reset();
					Invoke();*/
				}
			}, TaskCreationOptions.AttachedToParent);
		}

		private static void Invoke()
		{
			if (_writeQueue.TryDequeue(out NewCommand new_command))
			{
				Write(new_command);
			}
		}

		private static int index = 0;

		public static void Wait()
		{
			while (_writeQueue.Count > 0)
			{
				Thread.Sleep(100);
			}
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
				if (command.GetHeader<InvokeHeader>().CallStack.Last().StartsWith("User1.BasicFunctions.ControlCallFunction2"))
				{
					//var c = 2;
				}
				var exist = current.Childs.FirstOrDefault(x => x.LvlName.Equals(command.GetHeader<InvokeHeader>().CallStack.Last()));
				FunctionCall new_child;
				if(exist == null)
				{
					new_child = new FunctionCall()
					{
						CallDateTime = new_command.DateTime,
						Lvl = current.Lvl + 1,
						CallStack = command.GetHeader<InvokeHeader>().CallStack.ToList(),
						InputDataNames = command.InputData, //command.InputData.Select(x => x.Header.CallStack.Last()).ToList(),
						OutputDataName = command.OutputData,//command.OutputData.Header.CallStack.Last(),
						FunctionName = command.Function.GetHeader<FunctionHeader>().CallstackToString("."),
						LvlName = command.GetHeader<InvokeHeader>().CallStack.Last(),
						Childs = new List<FunctionCall>(),
						Command = new_command.Command
					};
					current.Childs.Add(new_child);
				}
				else
				{
					new_child = exist;
					new_child.FunctionName = command.Function.GetHeader<FunctionHeader>().CallstackToString(".");
					new_child.InputDataNames = command.InputData; //command.InputData.Select(x => x.Header.CallStack.Last()).ToList();
					new_child.OutputDataName = command.OutputData;//command.OutputData.Header.CallStack.Last();
					new_child.CallStack = command.GetHeader<InvokeHeader>().CallStack.ToList();
				}
				
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

			if (lvl_name.StartsWith("User1.BasicFunctions.ControlCallFunction2"))
			{
				//var c = 2;
			}

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

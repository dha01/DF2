using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.CodeExecution.Repository;
using Core.Model.CodeExecution.Service.Computing;
using Core.Model.DataFlowLogics.InstructionExecutionConveyor.Job;
using Core.Model.DataFlowLogics.Logger;
using Core.Model.DataFlowLogics.Logics.Service;
using Core.Model.NetworkLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//using SimpleMethods.Simple;

namespace Core.Tests.Model.Computing
{
	[TestClass]
	public class ComputingCoreTests
	{
		private IFunctionRepository _functionRepository;

		private IDataCellRepository _dataCellRepository;

		private ICommandRepository _commandRepository;

		private IJobManager _jobManager;

		private ICommandManager _commandManager;

		private ComputingCore _computingCore;

		private IDataFlowLogicsService _dataFlowLogicsService;

		[TestInitialize()]
		public void Initialize()
		{
			//Math2.Sum;
			
			/*_functionRepository = Mock.Of<IFunctionRepository>();
			_dataCellRepository = Mock.Of<IDataCellRepository>();
			_commandRepository = Mock.Of<ICommandRepository>();
			_jobManager = Mock.Of<IJobManager>();
			_commandManager = Mock.Of<ICommandManager>();
			_dataFlowLogicsService = Mock.Of<IDataFlowLogicsService>();
			*///_computingCore = new ComputingCore(_functionRepository, _dataCellRepository, _commandRepository/*, _commandManager*/, _dataFlowLogicsService);
		}
		/*
		[Test]
		public void AddCommandHeaders_CommandAdded_Success()
		{
			var command_headers = new List<CommandHeader>()
			{
				new CommandHeader()
				{
					
				}
			};

			_computingCore.AddCommandHeaders(command_headers);

			Mock.Get(_commandManager).Verify(x => x.AddHeaders(command_headers));
		}*/

		[TestMethod]
		public void ThredTest()
		{
			
			var x = 0;
			var warning_count = 0;
			var warning2_count = 0;
			var try_count = 0;
			long added = 0;

			var in_process = 0;

			var object_lock = new Object();
			var object_lock_complited = new Object();

			long length = 0;

			long complited = 0;

			var random = new Random(135);
			/*
			Action critical_action = () =>
			{
				while()
					x++;
					if (x > 1)
					{
						warning_count++;
						Console.WriteLine("Warning {0} / {1}", warning_count, try_count);
					}
					x--;
			};*/


			ManualResetEvent event_reset = new ManualResetEvent(false);

			var time_limit = DateTime.Now.AddSeconds(10);
			Action action_p = () =>
			{
				while (time_limit > DateTime.Now)
				{
					var count = random.Next(0, 10000);
					added += count;
					try_count++;
					
					lock (object_lock)
					{
						Interlocked.Add(ref length, count);
						event_reset.Set();
					}
					
				//	Console.WriteLine("Set {0}", count);
					
					
					//Thread.SpinWait(10000);
					//Thread.Sleep(1);
				}
			};

			Action invoke = () =>
			{
				while (true)
				{
					lock (object_lock)
					{
						if (length > 0)
						{
							Interlocked.Decrement(ref length);
							Interlocked.Increment(ref in_process);

							Console.WriteLine(in_process);
						}
						else
						{
							Console.WriteLine("break; {0}", Interlocked.Read(ref length));
							break;
						}
					}
					/*
					var sl = length;
					lock (object_lock)
					{
						length--;
					}
					if (sl - length < 1)
					{
						warning2_count++;
						Console.WriteLine("Warning 2 {0} / {1}", warning2_count, try_count);
					}*/

					var sdt = DateTime.Now.AddMilliseconds(1000);

					while (DateTime.Now < sdt)
					{
						var xx = 5 + 5;
					}

					Interlocked.Increment(ref complited);
					Interlocked.Decrement(ref in_process);
				}
			};
			

			Action action_m = () =>
			{
				while (time_limit > DateTime.Now)
				{
					if (length > 0 && !event_reset.WaitOne(0))
					{
						warning_count++;
						Console.WriteLine("Warning ! {0} / {1} {2}", warning_count, try_count, length);
					}
					
					event_reset.WaitOne();
					invoke();
					lock (object_lock)
					{
						if (Interlocked.Read(ref length) == 0)
						{
							event_reset.Reset();
						}
					}
				}
			};

			//Task.Factory.StartNew(action_m)

			var task_list = new List<Task>();
			var task_count = 16;

			task_list.Add(new Task(action_p));

			for (int i = 0; i < task_count; i++)
			{
				task_list.Add(new Task(action_m));
			}

			var process = Process.GetCurrentProcess();
			Console.WriteLine("ProcessorCount {0} ", Environment.ProcessorCount);
			Console.WriteLine("CurrentManagedThreadId {0} ", Environment.CurrentManagedThreadId);
			task_list.ForEach(t => t.Start());

			//PerformanceCounter theCPUCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
			
			Task.WaitAll(task_list.ToArray(), 10000);
			Console.WriteLine("Complited {0} / {1}", complited, added);

			Console.WriteLine("theCPUCounter {0} ", process.TotalProcessorTime);
		}
		/*
		private static BasicFunction Sum = new BasicFunction()
			{
				Id = 1,
				Header = new BasicFunctionHeader()
				{
					Name = "CallFunction1",
					Owners = new List<Owner>(),
					CallStack = new List<string>() { "User1", "BasicFunctions", "CallFunction1" },
					Id = 1
				}
			};

		private static BasicFunction Mul = new BasicFunction()
		{
			Id = 2,
			Header = new BasicFunctionHeader()
			{
				Name = "CallFunction2",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "BasicFunctions", "CallFunction2" },
				Id = 2
			}
		};
		
		private static ControlFunction BuildedControlFunction2
		{
			get
			{
				return CommandBuilder.Build(
					"ControlCallFunction",
					new List<string>() {"User1", "BasicFunctions", "ControlCallFunction2"},
					() =>
					{
						var cmd = new CommandBuilder();

						var a = cmd.InputData();
						var b = cmd.InputData();

						var tmp_1 = cmd.NewCommand(Sum, new[] {a, b});
						var tmp_2 = cmd.NewCommand(Mul, new[] {a, b});
						var tmp_3 = cmd.NewCommand(Mul, new[] {tmp_1, tmp_2});
						cmd.Return(tmp_3);

						return cmd;
					});
			}
		}*/



		private int ControlCallFunction(int a, int b)
		{
			return a + b;
		}

		private static ControlFunction BuildedControlFunction
		{
			get
			{
				return CommandBuilder.Build(
					"ControlCallFunction",
					new List<string>() { "User1", "BasicFunctions", "ControlCallFunction" },
					() =>
					{
						var cmd = new CommandBuilder();

						var a = cmd.InputData();
						var b = cmd.InputData();
						var c = cmd.InputData();
						var d = cmd.InputData();
						var e = cmd.InputData();
						var f = cmd.InputData();
						var g = cmd.InputData();
						var h = cmd.InputData();

						var x = cmd.NewCommand(new Function()
						{
							Header = CommandBuilder.BuildHeader("Sum", $"SimpleMethods.Simple.Math".Split('.').ToList())
						}, new[] { a, b });
						var y = cmd.NewCommand(new Function()
						{
							Header = CommandBuilder.BuildHeader("Sum", $"SimpleMethods.Simple.Math".Split('.').ToList())
						}, new[] { c, d });
						var z = cmd.NewCommand(new Function()
						{
							Header = CommandBuilder.BuildHeader("Sum", $"SimpleMethods.Simple.Math".Split('.').ToList())
						}, new[] { x, y });
						cmd.Return(z);
						/*
						var tmp_1 = cmd.NewCommand(Sum, new[] { a, b });
						var tmp_2 = cmd.NewCommand(Sum, new[] { c, d });
						var tmp_3 = cmd.NewCommand(Sum, new[] { e, f });
						var tmp_4 = cmd.NewCommand(Sum, new[] { g, h });

						var tmp_5 = cmd.NewCommand(Sum, new[] { tmp_1, tmp_2 });
						var tmp_6 = cmd.NewCommand(Sum, new[] { tmp_3, tmp_4 });

						var tmp_7 = cmd.NewCommand(BuildedControlFunction2, new[] { tmp_5, tmp_6 });
						var tmp_8 = cmd.NewCommand(Sum, new[] { tmp_7, cmd.Constant(5) });
						*/
						/*
						var tmp_1 = cmd.NewCommand(Sum, new[] {a, b});
						var tmp_2 = cmd.NewCommand(Mul, new[] {a, b});
						var tmp_3 = cmd.NewCommand(Sum, new[] {tmp_1, tmp_2});
						var tmp_4 = cmd.NewCommand(Sum, new[] { tmp_3, b });
						var tmp_5 = cmd.NewCommand(Sum, new[] { tmp_4, b });*/

						/*var tmp_1 = cmd.NewCommand(Sum, new[] { a, b });
						var tmp_2 = cmd.NewCommand(Mul, new[] { a, b });
						var tmp_3 = cmd.NewCommand(Sum, new[] { tmp_1, tmp_2 });
						var tmp_4 = cmd.NewCommand(Mul, new[] { tmp_1, tmp_2 });
						var tmp_5 = cmd.NewCommand(Sum, new[] { tmp_3, tmp_4 });
						var tmp_6 = cmd.NewCommand(Sum, new[] { tmp_5, b });
						var tmp_7 = cmd.NewCommand(Sum, new[] { tmp_6, b });*/

						//cmd.Return(tmp_8);

						return cmd;
					});
			}
		}


		[TestMethod]
		public void IntegrationTest()
		{
			/*var cs_assembly = CSharpFunctionExtractor.ExtractAssembly(
				@"F:\Main Folder\Аспирантура\Диссертация\Program\DF2\SimpleMethods.dll"
				//@"F:\Main Folder\Аспирантура\Диссертация\Program\DF2\Core\Core\bin\Debug\netcoreapp1.1\Core.dll"
				);

			var e1 = cs_assembly.CSharpClass.First();
			var e2 = e1.CSharpFunction.First(x => x.FuncName.Equals("Sum"));


			Sum = e2;*/


			//var xx = SimpleMethods.Simple.Math.Sum;

			var computing_core = ComputingCore.InitComputingCore();

			computing_core.AddAssembly(@"F:\Main Folder\Аспирантура\Диссертация\Program\DF2\SimpleMethods\bin\Debug\netcoreapp1.1\SimpleMethods.dll");
			//computing_core.AddFuction(cs_assembly.CSharpClass.First().CSharpFunction);
			//computing_core.AddFuction(new Function[] { Sum, Mul, BuildedControlFunction, BuildedControlFunction2 });
			/*

			var output_data_header = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "result" },
				HasValue = new Dictionary<Owner, bool>()
			};


			//var control_function = Simple.MainHeader;
			var input_data = CommandBuilder.BuildInputData(new object[] {1, 2, 3, 4, 5, 6, 7, 8}, new List<string>() {"User1", "Process1" });
			//computing_core.AddFuction(new List<Function>(){BuildedControlFunction});
			var command_headers = new List<CommandHeader>()
			{
				new CommandHeader()
				{
					//CallStack = new List<string>() { "User1", "Process1", "User1.BasicFunctions.ControlCallFunction" },
					CallStack = new List<string>() { "User1", "Process1" },
					FunctionHeader = CommandBuilder.BuildHeader("Main", $"SimpleMethods.Control.Simple".Split('.').ToList()),//(FunctionHeader)BuildedControlFunction.Header,//CommandBuilder.BuildHeader("Main", $"SimpleMethods.Control.Simple".Split('.').ToList()), //SimpleMethods.Control.Simple.MainHeader,
					InputDataHeaders = input_data.Select(x=>(DataCellHeader)x.Header).ToList(),
					OutputDataHeader = output_data_header,
					TriggeredCommands = new List<InvokeHeader>()
				}
			};

			
			computing_core.AddDataCell(input_data);
			computing_core.AddCommandHeaders(command_headers);


			var r = computing_core.GetDataCell(new []{ output_data_header }).FirstOrDefault();

			*/

			var r = computing_core.Exec("SimpleMethods.Control.Simple.Main", 1, 2, 3, 4, 5, 6, 7, 8).Result;

			//r.Wait();
			/*while (r == null || r.Data == null)
			{
				//r = computing_core.GetDataCell(new[] { output_data_header }).FirstOrDefault();
			}*/

			//Thread.Sleep(1000);
			StackTraceLogger.Wait();
			var log = StackTraceLogger.GetLog();

			var scheme = StackTraceLogger.GetLogScheme();

			if (r == null)
			{
				Console.WriteLine("null");
			}
			else
			{
				Console.WriteLine(r.Data.ToString());
				Assert.Fail(r.Data.ToString());
			}
		}


		[TestMethod]
		public void IntegrationSimpleTest()
		{
			var computing_core = ComputingCore.InitComputingCore();

			computing_core.AddAssembly(@"F:\Main Folder\Аспирантура\Диссертация\Program\DF2\SimpleMethods\bin\Debug\netcoreapp1.1\SimpleMethods.dll");
			var result = computing_core.Exec("SimpleMethods.Control.Simple.Main", 1, 2, 3, 4, 5, 6, 7, 8).Result;

			Assert.Fail(result.Data.ToString());
		}

		private static ComputingCore computing_core = ComputingCore.InitComputingCore();

		private static Assembly assembly = CommandBuilder.CreateFunctionFromSourceCode(@"
using Core.Model.CodeCompiler.Build.Attributes;
using Core.Model.CodeCompiler.Build.DataModel;
using Core.Model.CodeCompiler.Code;

namespace CustomNamespace
{
	public class CustomClass : ControlFunctionBase
	{
		[ControlFunction]
		public void Fib(Var<int> a)
		{

			//Return(Iif(a == 1 | a == 2, Const(1), Exec<int>(""Fib"", a - 1) + Exec<int>(""Fib"", a - 2)));

			var one = Const(1);
			var two = Const(2);
			Return(
				Iif(
					Exec<bool>(""Fib_labda_1"", a, one, two), 
					Exec<int>(""Fib_labda_2"", one), 
					Exec<int>(""Fib_labda_3"", a, one, two)
				));
		}

		[ControlFunction]
		public void Fib_labda_1(Var<int> a, Var<int> b, Var<int> c)
		{
			Return(a == b | a == c);
		}

		[ControlFunction]
		public void Fib_labda_2(Var<int> a)
		{
			Return(a);
		}

		[ControlFunction]
		public void Fib_labda_3(Var<int> a, Var<int> b, Var<int> c)
		{
			Return(Exec<int>(""Fib"", a - b) + Exec<int>(""Fib"", a - c));
		}
	}
}");
		[ClassInitialize]
		public static void ComputingCoreTestsInit(TestContext tc)
		{
			computing_core.AddAssembly(assembly);
		}

		[TestMethod]
		public void IntegrationSCustomCodeTest2()
		{

			//	computing_core.AddAssembly(@"F:\Main Folder\Аспирантура\Диссертация\Program\DF2\SimpleMethods\bin\Debug\netcoreapp1.1\SimpleMethods.dll");


			var text = GetText(assembly, "CustomNamespace.CustomClass.Fib");
			var text1 = GetText(assembly, "CustomNamespace.CustomClass.Fib_labda_1");
			var text2 = GetText(assembly, "CustomNamespace.CustomClass.Fib_labda_2");
			var text3 = GetText(assembly, "CustomNamespace.CustomClass.Fib_labda_3");
			//var new_text = GetNewText(assembly, "CustomNamespace.CustomClass.MyFunction");

			var count = 10;

			Task<DataCell>[] tasks = new Task<DataCell>[count + 1];
			int[] results = new int[count + 1];

			for (int i = 0; i < count; i++)
			{
				var result = computing_core.Exec("CustomNamespace.CustomClass.Fib", 15);
				//	result.Wait(15000);
				tasks[i] = result;
				//GC.Collect(2);
			}

			for (int i = 0; i < count; i++)
			{
				tasks[i].Wait(15000);
				results[i] = (int)tasks[i].Result.Data;
			}

			//Assert.Fail(result.Result.Data.ToString());
		}

		private int fib(int a)
		{
			return a == 1 || a == 2 ? 1 : fib(a-1) + fib(a-2);
		}

		private readonly ConcurrentDictionary<string, DataCell> _testDataCells = new ConcurrentDictionary<string, DataCell>();

		private void Meth()
		{
			for (int i = 0; i < 5000000; i++)
			{
				_testDataCells.TryAdd(Guid.NewGuid().ToString(), new DataCell
				{
					HasValue = true,
					Data = 25,
					Header = new DataCellHeader
					{
						//HasValue = new Dictionary<Owner, bool>(),
						CallStack = new [] { Guid.NewGuid().ToString() },
						//Owners = new List<Owner>()
					}
				});
			}

			foreach (var key in _testDataCells.Keys)
			{
				_testDataCells.TryRemove(key, out DataCell data_cell);
			}
		}

		[TestMethod]
		public void IntegrationSCustomCodeTest()
		{
			//Meth();

			//GC.Collect(2);

			var text = GetText(assembly, "CustomNamespace.CustomClass.MyFunction");
			//var new_text = GetNewText(assembly, "CustomNamespace.CustomClass.MyFunction");
			var f = fib(21);

			//var result1 = computing_core.Exec("CustomNamespace.CustomClass.MyFunction", 20);
			//var result2 = computing_core.Exec("CustomNamespace.CustomClass.MyFunction", 21);
			var count = 1000;

			Task<DataCell>[] tasks = new Task<DataCell>[count + 1];
			int[] results = new int[count + 1];

			for (int i = 0; i < count; i++)
			{
				var result = computing_core.Exec("CustomNamespace.CustomClass.MyFunction", 5);
			//	result.Wait(15000);
				tasks[i] = result;
				//GC.Collect(2);
			}

			for (int i = 0; i < count; i++)
			{
				tasks[i].Wait(15000);
				results[i] = (int)tasks[i].Result.Data;
			}

			//StackTraceLogger.Wait();
				//var log = StackTraceLogger.GetLog();

				//var scheme = StackTraceLogger.GetLogScheme(log);
				//Console.WriteLine(result.Result.Data);
				//Assert.Fail(result.Result.Data.ToString());
		}

		public string GetText(ControlFunction code)
		{
			//var max = Math.Max(code.Commands.Max(x => x.OutputDataId), code.Commands.Max(y => y.InputDataIds.Max(x=>x)))  + code.Constants.Count;

			var max = code.InputDataCount + code.Constants.Count + code.Commands.Count();

			string[] arr = new string[max];

			arr[0] = "[0] out OutputData";

			var index = 0;
			foreach (var row in code.Commands)
			{
				arr[row.OutputDataId] = $@"[{row.OutputDataId}]	{(row.OutputDataId == 0 ? "out" : "tmp")}	= {row.FunctionHeader.Token}<{index}>({string.Join(",", row.InputDataIds.Select(y => $"[{y}]"))}) {string.Join("|", row.TriggeredCommandIds.Select(y => $"<{y}>"))} {string.Join("|", row.ConditionId.Select(y => $"Cond([{y}])"))}";

				foreach (var val in row.InputDataIds)
				{
					if (string.IsNullOrEmpty(arr[val]))
					{
						arr[val] = $"[{val}]	in	InputData{val}";
					}
				}
				index++;
			}

			for (int i = 0; i < code.Constants.Count; i++)
			{
				var ind = arr.Length - i - 1;
				arr[ind] = $"[{ind}]	tmp	Const{code.Constants.Count - i}";
			}

			for (int i = 0; i < arr.Length; i++)
			{
				if (string.IsNullOrEmpty(arr[i]))
				{
					arr[i] = $"[{i}]	not use";
				}
			}

			var in_index = 1;
			var text = $@"{code.Token}({string.Join(", ", arr.Where(x => x.Contains("InputData")).Select(y => $"InputData{in_index++}"))})
{{
	{string.Join("\n	", arr)}
}}";

			return text;
		}

		public string GetText(Assembly myAssembly, string full_name)
		{
			var code = CommandBuilder.CompileMethodFromAssembly(myAssembly, full_name);

			return GetText(code);
		}

		public string GetNewText(Assembly myAssembly, string full_name)
		{
			var new_code = CommandBuilder.CompileMethodFromAssembly(myAssembly, full_name);

			return GetText(new_code);
		}

		[TestMethod]
		public void Command()
		{
			//CommandBuilder.BuildHeader("Main", $"SimpleMethods.Control.Simple".Split('.').ToList())
			var fs = new FileStream(@"F:\Main Folder\Аспирантура\Диссертация\Program\DF2\SimpleMethods\bin\Debug\netcoreapp1.1\SimpleMethods.dll", FileMode.Open);
			var myAssembly = AssemblyLoadContext.Default.LoadFromStream(fs);

			var t = GetText(myAssembly, "SimpleMethods.Control.Simple.Main");

		}
	}
}

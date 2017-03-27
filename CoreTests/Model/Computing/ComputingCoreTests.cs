using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Commands.Build;
using Core.Model.Commands.Logger;
using Core.Model.Computing;
using Core.Model.DataFlowLogics.Logics.Service;
using Core.Model.Execution;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.Job;
using Core.Model.Repository;
using Moq;
using NUnit.Framework;

namespace Core.Tests.Model.Computing
{
	[TestFixture]
	public class ComputingCoreTests
	{
		private IFunctionRepository _functionRepository;

		private IDataCellRepository _dataCellRepository;

		private ICommandRepository _commandRepository;

		private IJobManager _jobManager;

		private ICommandManager _commandManager;

		private ComputingCore _computingCore;

		private IDataFlowLogicsService _dataFlowLogicsService;

		[SetUp]
		public void SetUp()
		{
			_functionRepository = Mock.Of<IFunctionRepository>();
			_dataCellRepository = Mock.Of<IDataCellRepository>();
			_commandRepository = Mock.Of<ICommandRepository>();
			_jobManager = Mock.Of<IJobManager>();
			_commandManager = Mock.Of<ICommandManager>();
			_dataFlowLogicsService = Mock.Of<IDataFlowLogicsService>();
			_computingCore = new ComputingCore(_functionRepository, _dataCellRepository, _commandRepository/*, _commandManager*/, _dataFlowLogicsService);
		}
		
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
		}

		[Test]
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

			PerformanceCounter theCPUCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
			
			Task.WaitAll(task_list.ToArray(), 10000);
			Console.WriteLine("Complited {0} / {1}", complited, added);

			Console.WriteLine("theCPUCounter {0} ", process.TotalProcessorTime);
		}

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

		private static ControlFunction BuildedControlFunction2 = CommandBuilder.Build(
			"ControlCallFunction",
			new List<string>() { "User1", "BasicFunctions", "ControlCallFunction2" },
			() =>
			{
				var cmd = new CommandBuilder();

				var a = cmd.InputData();
				var b = cmd.InputData();

				var tmp_1 = cmd.NewCommand(Sum, new[] { a, b });
				var tmp_2 = cmd.NewCommand(Mul, new[] { a, b });
				var tmp_3 = cmd.NewCommand(Mul, new[] { tmp_1, tmp_2 });
				cmd.Return(tmp_3);

				return cmd;
			});


		private static ControlFunction BuildedControlFunction = CommandBuilder.Build(
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

				var tmp_1 = cmd.NewCommand(Sum, new[] { a, b });
				var tmp_2 = cmd.NewCommand(Sum, new[] { c, d });
				var tmp_3 = cmd.NewCommand(Sum, new[] { e, f });
				var tmp_4 = cmd.NewCommand(Sum, new[] { g, h });

				var tmp_5 = cmd.NewCommand(Sum, new[] { tmp_1, tmp_2 });
				var tmp_6 = cmd.NewCommand(Sum, new[] { tmp_3, tmp_4 });

				var tmp_7 = cmd.NewCommand(BuildedControlFunction2, new[] { tmp_5, tmp_6 });
				var tmp_8 = cmd.NewCommand(Sum, new[] { tmp_7, cmd.Constant(5) });
				
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

				cmd.Return(tmp_8);

				return cmd;
			});
		/// <summary>
		/// function(int a, int b)
		/// {
		///		var x1 = CallFunction1(a, b);
		///		var x2 = CallFunction2(a, b);
		///		var x3 = CallFunction1(x1, x2);
		///		return CallFunction1(x3, x2);
		/// }
		/// 
		/// function(int a, int b)
		/// {
		///		var x2 = CallFunction2(a, b);
		///		return CallFunction1(CallFunction1(CallFunction1(a, b), x2), x2);
		/// }
		/// 
		/// function(int a, int b)
		/// {
		///		var x2 = (a * b);
		///		return ((a + b) + x2) + x2);
		/// }
		/// </summary>
		/*private static ControlFunction ControlCallFunction = new ControlFunction()
		{
			Commands = new List<CommandTemplate>()
				{
					new CommandTemplate()
					{
						InputDataIds = new List<int> { 1, 2 },
						TriggeredCommandIds = new List<int> { 2 },
						OutputDataId = 3,
						FunctionHeader = (FunctionHeader)Sum.Header
					},
					new CommandTemplate()
					{
						InputDataIds = new List<int> { 1, 2 },
						TriggeredCommandIds = new List<int>{ 2 },
						OutputDataId = 4,
						FunctionHeader = (FunctionHeader)Mul.Header
					},
					new CommandTemplate()
					{
						InputDataIds = new List<int> { 3, 4 },
						TriggeredCommandIds = new List<int>{ 3 },
						OutputDataId = 5,
						FunctionHeader = (FunctionHeader)Sum.Header
					},
					new CommandTemplate()
					{
						InputDataIds = new List<int> { 5, 2 },
						TriggeredCommandIds = new List<int>{},
						OutputDataId = 0,
						FunctionHeader = (FunctionHeader)Sum.Header
					}
				},
			Header = new ControlFunctionHeader()
			{
				Name = "ControlCallFunction",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "ControlCallFunction" },
			}
		};
		*/
		/*private static DataCell a = new DataCell()
		{
			Data = 5,
			HasValue = true,
			Header = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "Data1" },
				HasValue = new Dictionary<Owner, bool>()
			}
		};

		private static DataCell b = new DataCell()
		{
			Data = 6,
			HasValue = true,
			Header = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "Data2" },
				HasValue = new Dictionary<Owner, bool>()
			}
		};*/

		[Test]
		public void IntegrationTest()
		{
			var output_data_header = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "result" },
				HasValue = new Dictionary<Owner, bool>()
			};
			var control_function = BuildedControlFunction;
			var input_data = CommandBuilder.BuildInputData(new object[] {1, 2, 3, 4, 5, 6, 7, 8}, new List<string>() {"User1", "Process1" });

			var command_headers = new List<CommandHeader>()
			{
				new CommandHeader()
				{
					CallStack = new List<string>() { "User1", "Process1", "User1.BasicFunctions.ControlCallFunction" },
					//Owners = new List<Owner>(),
					FunctionHeader = (FunctionHeader)control_function.Header,
					InputDataHeaders = input_data.Select(x=>(DataCellHeader)x.Header).ToList(),
					OutputDataHeader = output_data_header,
					TriggeredCommands = new List<InvokeHeader>()
				}
			};

			var data_cell_repository = new DataCellRepository();
			var function_repository = new FunctionRepository(data_cell_repository);
			var command_repository = new CommandRepository();

			

			var control_execution_service = new ControlExecutionService();

			var execution_manager = new ExecutionManager(
				new List<IExecutionService>()
				{
					new BasicExecutionService(),
					control_execution_service,
					new CSharpExecutionService()
				}
			);

			var job_manager = new JobManager(execution_manager);
			var preparation_command_service = new PreparationCommandService(data_cell_repository, function_repository);
			var data_flow_logics_service = new DataFlowLogicsService(job_manager, preparation_command_service);

			control_execution_service.SetDataFlowLogicsService(data_flow_logics_service);

			var command_service = new CommandService(
				function_repository,
				data_cell_repository,
				command_repository
			);
			/*
			var command_manager = new CommandManager(
				function_repository,
				data_cell_repository,
				job_manager,
				command_repository,
				command_service
			);*/

			var computing_core = new ComputingCore(
				function_repository,
				data_cell_repository,
				command_repository, 
				//command_manager
				data_flow_logics_service
			);

			function_repository.Add(new Function[] { Sum, Mul, BuildedControlFunction, BuildedControlFunction2 });
			computing_core.AddDataCell(input_data);

			computing_core.AddCommandHeaders(command_headers);


			var r = computing_core.GetDataCell(new []{ output_data_header }).FirstOrDefault();
			while (r == null || r.Data == null)
			{
				r = computing_core.GetDataCell(new[] { output_data_header }).FirstOrDefault();
			}

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
				Console.WriteLine(r.Data);
			}
		}
	}
}

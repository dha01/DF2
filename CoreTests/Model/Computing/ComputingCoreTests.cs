using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Computing;
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

		[SetUp]
		public void SetUp()
		{
			_functionRepository = Mock.Of<IFunctionRepository>();
			_dataCellRepository = Mock.Of<IDataCellRepository>();
			_commandRepository = Mock.Of<ICommandRepository>();
			_jobManager = Mock.Of<IJobManager>();
			_commandManager = Mock.Of<ICommandManager>();

			_computingCore = new ComputingCore(_functionRepository, _dataCellRepository, _jobManager, _commandRepository, _commandManager);
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

		private static BasicFunction CallFunction1 = new BasicFunction()
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

		private static BasicFunction CallFunction2 = new BasicFunction()
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

		/// <summary>
		/// function(int a, int b)
		/// {
		///		var x1 = CallFunction1(a, b);
		///		var x2 = CallFunction2(a, b);
		///		var x3 = CallFunction1(x1, x2);
		///		return CallFunction1(x3, x2);
		/// }
		/// </summary>
		private static ControlFunction ControlCallFunction = new ControlFunction()
		{
			Commands = new List<CommandTemplate>()
				{
					new CommandTemplate()
					{
						InputDataIds = new [] { 1, 2 },
						TriggeredCommandIds = new int[] { 1 },
						OutputDataId = 3,
						FunctionHeader = (FunctionHeader)CallFunction1.Header
					},
					new CommandTemplate()
					{
						InputDataIds = new [] { 1, 2 },
						TriggeredCommandIds = new int[]{},
						OutputDataId = 4,
						FunctionHeader = (FunctionHeader)CallFunction2.Header
					},
					new CommandTemplate()
					{
						InputDataIds = new [] { 3, 4 },
						TriggeredCommandIds = new int[]{},
						OutputDataId = 5,
						FunctionHeader = (FunctionHeader)CallFunction1.Header
					},
					new CommandTemplate()
					{
						InputDataIds = new [] { 5, 2 },
						TriggeredCommandIds = new int[]{},
						OutputDataId = 0,
						FunctionHeader = (FunctionHeader)CallFunction1.Header
					}
				},
			Header = new ControlFunctionHeader()
			{
				Name = "ControlCallFunction",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "ControlCallFunction" },
			}
		};

		private static DataCell a = new DataCell()
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
		};

		[Test]
		public void IntegrationTest()
		{
			var output_data_header = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "result" },
				HasValue = new Dictionary<Owner, bool>()
			};

			var command_headers = new List<CommandHeader>()
			{
				new CommandHeader()
				{
					CallStack = new List<string>() { "User1", "Process1" },
					//Owners = new List<Owner>(),
					FunctionHeader = (FunctionHeader)ControlCallFunction.Header,
					InputDataHeaders = new List<DataCellHeader>()
					{
						(DataCellHeader)a.Header,
						(DataCellHeader)b.Header
					},
					OutputDataHeader = output_data_header,
					TriggeredCommands = new List<InvokeHeader>()
				}
			};
			
			var function_repository = new FunctionRepository();
			var data_cell_repository = new DataCellRepository();
			var command_repository = new CommandRepository();
			var execution_manager = new ExecutionManager(
				new List<IExecutionService>()
				{
					new BasicExecutionService(),
					new ControlExecutionService(command_repository),
					new CSharpExecutionService()
				}
			);

			var job_manager = new JobManager(execution_manager);

			var command_service = new CommandService(
				function_repository,
				data_cell_repository,
				command_repository
			);

			var command_manager = new CommandManager(
				function_repository,
				data_cell_repository,
				job_manager,
				command_repository,
				command_service
			);

			var computing_core = new ComputingCore(
				function_repository,
				data_cell_repository,
				job_manager,
				command_repository, 
				command_manager
			);

			function_repository.Add(new Function[] { CallFunction1, CallFunction2, ControlCallFunction });
			computing_core.AddDataCell(new[] { a, b });

			computing_core.AddCommandHeaders(command_headers);


			var r = computing_core.GetDataCell(new []{ output_data_header }).FirstOrDefault();
			while (r == null || r.Data == null)
			{
				r = computing_core.GetDataCell(new[] { output_data_header }).FirstOrDefault();
			}
			

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

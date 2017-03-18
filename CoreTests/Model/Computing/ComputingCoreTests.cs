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



		[Test]
		public void IntegrationTest()
		{
			var function_header = new BasicFunctionHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "CallFunction1" },
				Id = 1
			};

			var function = new BasicFunction()
			{
				Id = 2,
				Name = "CallFunction1",
				Header = function_header
			};

			var input_data_header_one = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "Data1" },
				HasValue = new Dictionary<Owner, bool>()
			};

			var input_data_one = new DataCell()
			{
				Header = input_data_header_one,
				HasValue = true,
				Data = 5
			};

			var input_data_header_two = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "Data2" },
				HasValue = new Dictionary<Owner, bool>()
			};

			var input_data_two = new DataCell()
			{
				Header = input_data_header_two,
				HasValue = true,
				Data = 6
			};

			var output_data_header = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "result" },
				HasValue = new Dictionary<Owner, bool>()
			};

			var triggered_command_one = new InvokeHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "CallFunction2" },
			};

			var triggered_command_two = new InvokeHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "CallFunction3" },
			};

			var command_headers = new List<CommandHeader>()
			{
				new CommandHeader()
				{
					FunctionHeader = function_header,
					InputDataHeaders = new List<DataCellHeader>()
					{
						input_data_header_one,
						input_data_header_two
					},
					OutputDataHeader = output_data_header,
					TriggeredCommands = new List<InvokeHeader>()
					{
						triggered_command_one,
						triggered_command_two
					}
				}
			};
			
			var function_repository = new FunctionRepository();
			var data_cell_repository = new DataCellRepository();
			var command_repository = new CommandRepository();
			var job_manager = new JobManager();

			var computing_core = new ComputingCore(
				function_repository,
				data_cell_repository,
				job_manager,
				command_repository, 
				new CommandManager(
					function_repository,
					data_cell_repository,
					job_manager,
					command_repository,
					new CommandService(
						function_repository,
						data_cell_repository,
						command_repository
					)
				)
			);

			function_repository.Add(new[] { function });
			computing_core.AddDataCell(new[] { input_data_one, input_data_two });

			computing_core.AddCommandHeaders(command_headers);

			//Thread.Sleep(1000);

			var r = computing_core.GetDataCell(new []{ output_data_header }).FirstOrDefault();

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

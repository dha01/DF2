using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Bodies.Commands;
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
	public class CommandManagerTests
	{
		private IFunctionRepository _functionRepository;

		private IDataCellRepository _dataCellRepository;

		private ICommandRepository _commandRepository;

		private IJobManager _jobManager;

		private ICommandManager _commandManager;
		private ICommandService _commandService;

		private ComputingCore _computingCore;

		[SetUp]
		public void SetUp()
		{
			_functionRepository = Mock.Of<IFunctionRepository>();
			_dataCellRepository = Mock.Of<IDataCellRepository>();
			_commandRepository = Mock.Of<ICommandRepository>();
			_commandService = Mock.Of<ICommandService>();
			_jobManager = Mock.Of<IJobManager>();
			_commandManager = new CommandManager(_functionRepository, _dataCellRepository, _jobManager, _commandRepository, _commandService);
		}
		/// <summary>
		/// Проверяет, что все заголовки данных и функций добаляются в репозитории.
		/// </summary>
		[Test]
		public void AddHeaders_PrepareCommandWithExistsDataAndFunction_()
		{
			var function_header = new BasicFunctionHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "User1", "Process1", "CallFunction1" },
				Id = 1
			};

			var function = new BasicFunction()
			{
				Id = 1,
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
				Data = 1
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
				Data = 2
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

			var added_function_headers = new List<FunctionHeader>();
			var added_data_cell_headers = new List<DataCellHeader>();

			Mock.Get(_jobManager).Setup(x => x.GetFreeJobCount()).Returns(1);

			Mock.Get(_dataCellRepository).Setup(x => x.AddHeaders(It.IsAny<IEnumerable<DataCellHeader>>())).Callback((IEnumerable<DataCellHeader> new_data_cell_headers) =>
			{
				added_data_cell_headers.AddRange(new_data_cell_headers);
			});

			Mock.Get(_functionRepository).Setup(x => x.AddHeaders(It.IsAny<IEnumerable<FunctionHeader>>())).Callback((IEnumerable<FunctionHeader> new_function_headers) =>
			{
				added_function_headers.AddRange(new_function_headers);
			});
/*
			Mock.Get(_functionRepository).Setup(x => x.Get(It.IsAny<IEnumerable<FunctionHeader>>())).Returns(
				(IEnumerable<FunctionHeader> headers) =>
				{
					var result = new List<Function>();
					if (headers.Contains(function_header)) { result.Add(function); }
					return result;
				});
			Mock.Get(_dataCellRepository).Setup(x => x.Get(It.IsAny<IEnumerable<DataCellHeader>>())).Returns(
				(IEnumerable<DataCellHeader> headers) =>
				{
					var result = new List<DataCell>();
					if (headers.Contains(input_data_header_one)) { result.Add(input_data_one); }
					if (headers.Contains(input_data_header_two)) { result.Add(input_data_two); }
					return result;
				});*/



			_commandManager.AddHeaders(command_headers);

			Thread.Sleep(100);

			Mock.Get(_commandService).Verify(x => x.PrepareAndInvokeCommands(It.Is((IEnumerable<CommandHeader> command_heades) => command_heades.Count() == 1 ), It.IsAny<Action<Command>>()));

			Assert.Contains(input_data_header_one, added_data_cell_headers);
			Assert.Contains(input_data_header_two, added_data_cell_headers);
			Assert.Contains(output_data_header, added_data_cell_headers);

			Assert.Contains(function_header, added_function_headers);
		}


		/// <summary>
		/// Проверяет, что все заголовки данных и функций добаляются в репозитории.
		/// </summary>
		[Test]
		public void AddHeaders_Void_HeadersAdded()
		{
			var function_header = new FunctionHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>(){ "User1", "Process1", "CallFunction1"}
			};

			var input_data_header_one = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>(){ "User1", "Process1", "Data1"},
				HasValue = new Dictionary<Owner, bool>()
			};

			var input_data_header_two = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>(){ "User1", "Process1", "Data2"},
				HasValue = new Dictionary<Owner, bool>()
			};

			var output_data_header = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>(){ "User1", "Process1", "result"},
				HasValue = new Dictionary<Owner, bool>()
			};

			var triggered_command_one = new InvokeHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>(){ "User1", "Process1", "CallFunction2"},
			};

			var triggered_command_two = new InvokeHeader()
			{
				Owners = new List<Owner>(),
				CallStack = new List<string>(){ "User1", "Process1", "CallFunction3"},
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

			var added_function_headers = new List<FunctionHeader>();
			var added_data_cell_headers = new List<DataCellHeader>();

			Mock.Get(_jobManager).Setup(x => x.GetFreeJobCount()).Returns(0);

			Mock.Get(_dataCellRepository).Setup(x => x.AddHeaders(It.IsAny<IEnumerable<DataCellHeader>>())).Callback((IEnumerable<DataCellHeader> new_data_cell_headers) =>
			{
				added_data_cell_headers.AddRange(new_data_cell_headers);
			});

			Mock.Get(_functionRepository).Setup(x => x.AddHeaders(It.IsAny<IEnumerable<FunctionHeader>>())).Callback((IEnumerable<FunctionHeader> new_function_headers) =>
			{
				added_function_headers.AddRange(new_function_headers);
			});

			_commandManager.AddHeaders(command_headers);

			Thread.Sleep(100);

			Assert.Contains(input_data_header_one, added_data_cell_headers);
			Assert.Contains(input_data_header_two, added_data_cell_headers);
			Assert.Contains(output_data_header, added_data_cell_headers);

			Assert.Contains(function_header, added_function_headers);
		}
	}
}

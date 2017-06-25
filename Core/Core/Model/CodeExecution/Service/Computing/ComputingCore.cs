using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Commands.Build;
using Core.Model.Compiler.Build.DataModel;
using Core.Model.Compiler.Code;
using Core.Model.DataFlowLogics.Logics.Service;
using Core.Model.Execution;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.InstructionExecutionConveyor.Extractors;
using Core.Model.Job;
using Core.Model.Repository;

namespace Core.Model.Computing
{
	/// <summary>
	/// Вычислительное ядро.
	/// </summary>
	public class ComputingCore : IComputingCore
	{
		public static ComputingCore InitComputingCore()
		{
			var data_cell_repository = new DataCellRepository();
			var function_repository = new FunctionRepository(data_cell_repository);
			var command_repository = new CommandRepository();
			var control_execution_service = new ControlExecutionService();

			var execution_manager = new ExecutionManager(
				new List<IExecutionService>()
				{
					new BasicExecutionService(),
					control_execution_service,
					new CSharpExecutionService(function_repository)
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
			var computing_core = new ComputingCore(
				function_repository,
				data_cell_repository,
				command_repository,
				data_flow_logics_service
			);

			function_repository.Add(new List<Function>()
			{
				BasicFunctions.Sum,
				BasicFunctions.Sub,
				BasicFunctions.Mul,
				BasicFunctions.Div,
				BasicFunctions.Set,
				BasicFunctions.Any
			});

			return computing_core;
		}

		//public DataCell Invoke() { }

		public void AddFuction(IEnumerable<Function> conteiner, bool send_subscribes = true)
		{
			_functionRepository.Add(conteiner, send_subscribes);
		}

		public void AddAssembly(Assembly assembly)
		{
			var cs_assembly = CSharpFunctionExtractor.ExtractAssembly(assembly);

			var funcs = new List<Function>();

			foreach (var cl in cs_assembly.CSharpClass)
			{
				funcs.AddRange(cl.CSharpFunction);
			}

			_functionRepository.Add(funcs);
			_functionRepository.Add(cs_assembly.ControlFunctions);
		}

		public void AddAssembly(string path)
		{
			var cs_assembly = CSharpFunctionExtractor.ExtractAssembly(path);

			var funcs = new List<Function>();

			foreach (var cl in cs_assembly.CSharpClass)
			{
				funcs.AddRange(cl.CSharpFunction);
			}

			_functionRepository.Add(funcs);
			_functionRepository.Add(cs_assembly.ControlFunctions);
		}

		private readonly IFunctionRepository _functionRepository;

		private readonly IDataCellRepository _dataCellRepository;

		private readonly ICommandRepository _commandRepository;

		//private readonly ICommandManager _commandManager;

		private readonly IDataFlowLogicsService _dataFlowLogicsService;

		public ComputingCore(IFunctionRepository function_repository, IDataCellRepository data_cell_repository, ICommandRepository command_repository/*, ICommandManager command_manager*/, IDataFlowLogicsService data_flow_logics_service)
		{
			_functionRepository = function_repository;
			_dataCellRepository = data_cell_repository;
			_commandRepository = command_repository;
			//_commandManager = command_manager;
			_dataFlowLogicsService = data_flow_logics_service;
		}

		public virtual void Invoke(object first_command)
		{
			throw new System.NotImplementedException();
		}

		private static int index = 0;
		public Task<DataCell> Exec(FunctionHeader function_header, params object[] param)
		{
			string root = "User1.Process" + index++;

			var output_data_header = new DataCellHeader()
			{
				Owners = new List<Owner>(),
				CallStack = $"{root}.result".Split('.').ToList(),
				HasValue = new Dictionary<Owner, bool>()
			};

			var input_data = CommandBuilder.BuildInputData(param, $"{root}".Split('.').ToList());

			var command_headers = new List<CommandHeader>()
			{
				new CommandHeader()
				{
					CallStack = $"{root}".Split('.').ToList(),
					FunctionHeader = function_header,//CommandBuilder.BuildHeader("Main", $"SimpleMethods.Control.Simple".Split('.').ToList()),//(FunctionHeader)BuildedControlFunction.Header,//CommandBuilder.BuildHeader("Main", $"SimpleMethods.Control.Simple".Split('.').ToList()), //SimpleMethods.Control.Simple.MainHeader,
					InputDataHeaders = input_data.Select(x=>(DataCellHeader)x.Header).ToList(),
					OutputDataHeader = output_data_header,
					TriggeredCommands = new List<InvokeHeader>()
				}
			};


			AddDataCell(input_data);
			AddCommandHeaders(command_headers);

			var result = GetDataCell(new[] {output_data_header}).First();
			return Task.Factory.StartNew(() =>
			{
				while (!result.HasValue)
				{

				}
				return result;
			});
		}

		#region Команды

		public virtual void AddCommandHeaders(IEnumerable<CommandHeader> command_headers)
		{
			//_commandManager.AddHeaders(command_headers);
			_dataFlowLogicsService.AddNewCommandHeader(command_headers);
		}

		public IEnumerable<CommandHeader> GetCommandHeaders(IEnumerable<InvokeHeader> invoke_header)
		{
			return _commandRepository.Get(invoke_header);
		}

		#endregion

		#region Данные

		public virtual void AddDataCell(IEnumerable<DataCell> data_cells)
		{
			_dataCellRepository.Add(data_cells);
		}

		public virtual IEnumerable<DataCell> GetDataCell(IEnumerable<DataCellHeader> data_cell_headers)
		{
			return _dataCellRepository.Get(data_cell_headers);
		}

		#endregion

		#region Функции

		public virtual void AddFunctions(IEnumerable<Function> function)
		{
			_functionRepository.Add(function);
		}

		public virtual IEnumerable<Function> GetFunctions(IEnumerable<FunctionHeader> functions)
		{
			return _functionRepository.Get(functions);
		}

		#endregion
	}
}


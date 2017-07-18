using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeCompiler.Code;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.CodeExecution.Repository;
using Core.Model.CodeExecution.Service.DataModel;
using Core.Model.CodeExecution.Service.Execution;
using Core.Model.DataFlowLogics.InstructionExecutionConveyor.Extractors;
using Core.Model.DataFlowLogics.InstructionExecutionConveyor.Job;
using Core.Model.DataFlowLogics.Logics.DataModel;
using Core.Model.DataFlowLogics.Logics.Service;
using Core.Model.NetworkLogic;

namespace Core.Model.CodeExecution.Service.Computing
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
			var control_execution_service = new ControlExecutionService(data_cell_repository);

			var execution_manager = new ExecutionManager(
				new List<IExecutionService>()
				{
					new BasicExecutionService(),
					control_execution_service,
					new CSharpExecutionService(function_repository)
				},
				data_cell_repository
			);

			var job_manager = new JobManager(execution_manager);
			var preparation_command_service = new PreparationCommandService(data_cell_repository, function_repository);
			var data_flow_logics_service = new DataFlowLogicsService(job_manager, preparation_command_service, data_cell_repository);
			control_execution_service.SetDataFlowLogicsService(data_flow_logics_service);

			var computing_core = new ComputingCore(
				function_repository,
				data_cell_repository,
				command_repository,
				data_flow_logics_service
			);
			
			function_repository.Add(BasicFunctionModel.AllMethods.Select(x => (Function)x.Value.BasicFunction).ToList());

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
			Token root = new Token($"User1/Process{index++}").Next(function_header.Token);

			var output_data_header = new DataCellHeader()
			{
				Token = root.Next("result")
			};

			var in_index = 0;
			var input_data = param.Select(x => new DataCell()
				{
					Data = x,
					HasValue = true,
					Header = new DataCellHeader()
					{
						Token = root.Next($"InputData{in_index}")
					}
				})
				.ToList(); //CommandBuilder.BuildInputData(param, root.ToList());

			var command_headers = new List<CommandHeader>()
			{
				new CommandHeader()
				{
					Token = root,
					FunctionHeader = function_header,//CommandBuilder.BuildHeader("Main", $"SimpleMethods.Control.Simple".Split('.').ToList()),//(FunctionHeader)BuildedControlFunction.Header,//CommandBuilder.BuildHeader("Main", $"SimpleMethods.Control.Simple".Split('.').ToList()), //SimpleMethods.Control.Simple.MainHeader,
					InputDataHeaders = input_data.Select(x=>(DataCellHeader)x.Header).ToList(),
					OutputDataHeader = output_data_header,
					TriggeredCommands = new List<InvokeHeader>(),
					ConditionDataHeaders = new List<DataCellHeader>()
				}
			};

			AddDataCell(input_data);
			AddCommandHeaders(command_headers);


			ManualResetEvent mer = new ManualResetEvent(false);
			_dataCellRepository.Subscribe(new[] { output_data_header }, (dch) =>
			{
				mer.Set();
			} );

			return Task.Factory.StartNew(() =>
			{
				mer.WaitOne();
				/*while (!result.HasValue.HasValue)
				{

				}*/
				return GetDataCell(new[] { output_data_header }).First();
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

		/// <summary>
		/// Возвращает количество команд в каждой из очередей.
		/// </summary>
		/// <returns></returns>
		public ComputingCoreInfo GetComputingCoreInfo()
		{
			return new ComputingCoreInfo
			{
				StateQueuesInfo = _dataFlowLogicsService.GetStateQueuesInfo(),
				DataCellsInfo = _dataCellRepository.GetConteinerRepositoryInfo()
			};
		}
	}
}


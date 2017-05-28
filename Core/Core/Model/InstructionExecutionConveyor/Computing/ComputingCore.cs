using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.DataFlowLogics.Logics.Service;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.Job;
using Core.Model.Repository;

namespace Core.Model.Computing
{
	/// <summary>
	/// Вычислительное ядро.
	/// </summary>
	public class ComputingCore : IComputingCore
	{
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


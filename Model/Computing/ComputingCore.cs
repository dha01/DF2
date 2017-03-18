using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.Job;
using Core.Model.Repository;

namespace Core.Model.Computing
{

	public class ComputingCore : IComputingCore
	{
		private IFunctionRepository _functionRepository;

		private IDataCellRepository _dataCellRepository;

		private ICommandRepository _commandRepository;

		private IJobManager _jobManager;

		private ICommandManager _commandManager;

		public ComputingCore(IFunctionRepository function_repository, IDataCellRepository data_cell_repository, IJobManager job_manager, ICommandRepository command_repository, ICommandManager command_manager)
		{
			_functionRepository = function_repository;
			_dataCellRepository = data_cell_repository;
			_commandRepository = command_repository;
			_jobManager = job_manager;
			_commandManager = command_manager;
		}

		public virtual void Invoke(object first_command)
		{
			throw new System.NotImplementedException();
		}

		#region Команды

		public virtual void AddCommandHeaders(IEnumerable<CommandHeader> command_headers)
		{
			_commandManager.AddHeaders(command_headers);
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


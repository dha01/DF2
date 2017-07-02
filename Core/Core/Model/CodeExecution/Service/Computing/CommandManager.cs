using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.CodeExecution.Repository;
using Core.Model.DataFlowLogics.InstructionExecutionConveyor.Job;

namespace Core.Model.CodeExecution.Service.Computing
{
	/// <summary>
	/// Управляет потоком команд и решает нужно ли их исполнять на этом узле или ищет другой подходящий узел и отправляет на исполнение на него.
	/// </summary>
	public class CommandManager : ICommandManager
	{
		private readonly IDataCellRepository _dataCellRepository;

		private readonly IFunctionRepository _functionRepository;

		private readonly IJobManager _jobManager;

		private readonly ICommandRepository _commandRepository;

		private readonly ConcurrentQueue<CommandHeader> _commandHeaders;

		private readonly ConcurrentQueue<Command> _readyCommands;

		private readonly ICommandService _commandService;

		public CommandManager(IFunctionRepository function_repository, IDataCellRepository data_cell_repository, IJobManager job_manager, ICommandRepository command_repository, ICommandService command_service)
		{
			_dataCellRepository = data_cell_repository;
			_functionRepository = function_repository;
			_jobManager = job_manager;
			_jobManager.OnReliseJob = OnReliseJob;
			_commandRepository = command_repository;
			_commandService = command_service;

			_commandHeaders = new ConcurrentQueue<CommandHeader>();
			_readyCommands = new ConcurrentQueue<Command>();

			_commandRepository.Subscribe(null, OnNewCommand);
		}

		/// <summary>
		/// Событие при завершении вычисления исполнителя.
		/// </summary>
		/// <param name="result"></param>
		private void OnReliseJob(Command result)
		{
			if (result.Function.GetType() != typeof(ControlFunction))
			{
				_dataCellRepository.Add(new[] { result.OutputData });
				//Console.WriteLine(string.Format("CommandManager.OnReliseJob _readyCommands.Count={0}, _commandHeaders.Count={1}", _readyCommands.Count, _commandHeaders.Count));
			}

			if (_readyCommands.Count > 0)
			{
				Command command;
				if (_readyCommands.TryDequeue(out command))
				{
					_jobManager.AddCommand(command);
				}
			}
			else
			if (_commandHeaders.Count > 0)
			{
				CommandHeader command_header;
				if (_commandHeaders.TryDequeue(out command_header))
				{
					PrepareOrSendToWait(new[] { command_header });
				}
			}
		}

		public void OnNewCommand(InvokeHeader invoke_header)
		{
			var command_header = _commandRepository.Get(new[] { invoke_header }).First();
			
			_functionRepository.AddHeaders(new [] { command_header.FunctionHeader });
			_dataCellRepository.AddHeaders(new [] { command_header.OutputDataHeader });
			_dataCellRepository.AddHeaders(command_header.InputDataHeaders);

			//Console.WriteLine(string.Format("CommandManager.OnNewCommand Callstack={0}", string.Join("/", invoke_header.CallStack)));
			PrepareOrSendToWait(new[] { command_header });
		}

		public virtual void AddHeaders(IEnumerable<CommandHeader> command_headers)
		{
			_commandRepository.Add(command_headers);
		}

		/// <summary>
		/// Отправляет команды на подготовку или оставляет в очереди ожидания, если нет свободных исполнителей.
		/// </summary>
		/// <param name="command_headers"></param>
		private void PrepareOrSendToWait(IEnumerable<CommandHeader> command_headers)
		{
			var list = command_headers as IList<CommandHeader> ?? command_headers.ToList();
			int free_job_count = _jobManager.GetFreeJobCount();
			
			// Отправляет на исполнение количество команд равное числу свододных исполнителей.
			_commandService.PrepareAndInvokeCommands(list.Take(free_job_count), SendToInvokeCommand);

			// Отправляет оставшиеся заголовки команд в очередь ожидания.
			foreach (var command_header in list.Skip(free_job_count))
			{
				_commandHeaders.Enqueue(command_header);
			}
		}

		private void SendToInvokeCommand(Command command)
		{
			if (_jobManager.GetFreeJobCount() > 0)
			{
				_jobManager.AddCommand(command);
			}
			else
			{
				_readyCommands.Enqueue(command);
			}
		}
	}
}


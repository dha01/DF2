
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.Job;
using Core.Model.Repository;

namespace Core.Model.Computing
{
	/// <summary>
	/// Управляет потоком команд и решает нужно лих их исполнять на этом узле или ищет другой подходящий узел и отправляет на исполнение на него.
	/// </summary>
	public class CommandManager : ICommandManager
	{
		private IDataCellRepository _dataCellRepository;

		private IFunctionRepository _functionRepository;

		private IJobManager _jobManager;

		private ICommandRepository _commandRepository;

		private Queue<CommandHeader> _commandHeaders;

		private Queue<CommandHeader> _preparingCommandHeaders;

		private Queue<Command> _readyCommands;

		private ICommandService _commandService;

		public CommandManager(IFunctionRepository function_repository, IDataCellRepository data_cell_repository, IJobManager job_manager, ICommandRepository command_repository, ICommandService command_service)
		{
			_dataCellRepository = data_cell_repository;
			_functionRepository = function_repository;
			_jobManager = job_manager;
			_jobManager.OnReliseJob = OnReliseJob;
			_commandRepository = command_repository;
			_commandService = command_service;

			_commandHeaders = new Queue<CommandHeader>();
			_preparingCommandHeaders = new Queue<CommandHeader>();
			_readyCommands = new Queue<Command>();
		}

		/// <summary>
		/// Событие при завершении вычисления исполнителя.
		/// </summary>
		/// <param name="result"></param>
		private void OnReliseJob(Command result)
		{
			_dataCellRepository.Add(new[] { result.OutputData });

			if (_readyCommands.Count > 0)
			{
				_jobManager.AddCommand(_readyCommands.Dequeue());
			}
			else
			if (_commandHeaders.Count > 0)
			{
				PrepareOrSendToWait(new[] {_commandHeaders.Dequeue()});
			}
		}

		public virtual void AddHeaders(IEnumerable<CommandHeader> command_headers)
		{
			_functionRepository.AddHeaders(command_headers.Select(x => x.FunctionHeader));
			_dataCellRepository.AddHeaders(command_headers.Select(x => x.OutputDataHeader));
			foreach (var command_header in command_headers)
			{
				_dataCellRepository.AddHeaders(command_header.InputDataHeaders);				
			}

			_commandRepository.AddHeaders(command_headers);

			PrepareOrSendToWait(command_headers);
		}

		/// <summary>
		/// Отправляет команды на подготовку или оставляет в очереди ожидания, если нет свободных исполнителей.
		/// </summary>
		/// <param name="command_headers"></param>
		private void PrepareOrSendToWait(IEnumerable<CommandHeader> command_headers)
		{
			var list = command_headers as IList<CommandHeader> ?? command_headers.ToList();
			int free_job_count = _jobManager.GetFreeJobCount();
			_commandService.PrepareAndInvokeCommands(list.Take(free_job_count), SendToInvokeCommand);

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


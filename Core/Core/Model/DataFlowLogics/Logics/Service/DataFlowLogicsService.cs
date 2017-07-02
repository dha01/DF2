using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.DataFlowLogics.InstructionExecutionConveyor.Job;

namespace Core.Model.DataFlowLogics.Logics.Service
{
	/// <summary>
	/// Сервис логики потока данных.
	/// </summary>
	public class DataFlowLogicsService : IDataFlowLogicsService
	{
		/// <summary>
		/// Все команды.
		/// </summary>
		private readonly ConcurrentDictionary<string, CommandHeader> _allCommandHeaders = new ConcurrentDictionary<string, CommandHeader>();

		/// <summary>
		/// Команды находящиеся в очереди на подготовку.
		/// </summary>
		private readonly ConcurrentDictionary<string, CommandHeader> _awaitingPreparationCommandHeaders = new ConcurrentDictionary<string, CommandHeader>();

		/// <summary>
		/// Команды находящиеся в подготовке.
		/// </summary>
		private readonly ConcurrentDictionary<string, CommandHeader> _preparationCommandHeaders = new ConcurrentDictionary<string, CommandHeader>();

		/// <summary>
		/// Готовые к исполнению команды ожидающие своей очереди.
		/// </summary>
		private readonly ConcurrentDictionary<string, Command> _awaitingExecutionCommands = new ConcurrentDictionary<string, Command>();

		/// <summary>
		/// Команды находящиеся в процессе исполнения.
		/// </summary>
		private readonly ConcurrentDictionary<string, Command> _executingCommands = new ConcurrentDictionary<string, Command>();
		
		/// <summary>
		/// Исполненные команды.
		/// </summary>
		private readonly ConcurrentDictionary<string, Command> _executedCommands = new ConcurrentDictionary<string, Command>();

		/// <summary>
		/// Управлеяющий пулом исполнителей.
		/// </summary>
		private readonly IJobManager _jobManager;

		/// <summary>
		/// Сервис подготовки команд к исполнению.
		/// </summary>
		private readonly IPreparationCommandService _preparationCommandService;

		/// <summary>
		/// Конструктор.
		/// </summary>
		/// <param name="job_manager">Управлеяющий пулом исполнителей.</param>
		/// <param name="preparation_command_service">Сервис подготовки команд к исполнению.</param>
		public DataFlowLogicsService(IJobManager job_manager, IPreparationCommandService preparation_command_service)
		{
			_jobManager = job_manager;
			_preparationCommandService = preparation_command_service;

			_preparationCommandService.OnPreparedCommand = OnPreparedCommand;

			_jobManager.OnReliseJob = (command) =>
			{
				OnExecutedCommand(command);
				OnFreeJob();
			};
		}

		public void AddNewCommandHeader(IEnumerable<CommandHeader> command_headers)
		{
			foreach (var command_header in command_headers)
			{
				AddNewCommandHeader(command_header);
			}
		}

		/// <summary>
		/// Для уже существующих заголовков команд добаляет в список владельцев новых владельцев команды.
		/// Новые команды отправляет на подготовку к исполнению.
		/// </summary>
		/// <param name="command_header"></param>
		public void AddNewCommandHeader(CommandHeader command_header)
		{
			//Console.WriteLine("! AddNewCommandHeader {0}", command_header.CallstackToString());

			if (_allCommandHeaders.ContainsKey(command_header.Token))
			{
				CommandHeader header;
				if (_allCommandHeaders.TryGetValue(command_header.Token, out header))
				{
					header.AddOwners(command_header.Owners);
				}
				else
				{
					throw new NotImplementedException("DataFlowLogicsService.AddNewCommandHeader Не удалось получить.");
				}
			}
			else
			{
				if (!_allCommandHeaders.TryAdd(command_header.Token, command_header))
				{
					throw new NotImplementedException("DataFlowLogicsService.AddNewCommandHeader _allCommandHeaders Не удалось добавить.");
				}

				if (_jobManager.HasFreeJob())
				{
					if (!_preparationCommandHeaders.TryAdd(command_header.Token, command_header))
					{
						throw new NotImplementedException("DataFlowLogicsService.AddNewCommandHeader _rawCommandHeaders Не удалось добавить.");
					}
					_preparationCommandService.PrepareCommand(command_header);
					
					//Console.WriteLine("! AddNewCommandHeader _preparationCommandHeaders {0}", command_header.CallstackToString());
				}
				else
				{
					if (!_awaitingPreparationCommandHeaders.TryAdd(command_header.Token, command_header))
					{
						throw new NotImplementedException("DataFlowLogicsService.AddNewCommandHeader _rawCommandHeaders Не удалось добавить.");
					}
				}
				
			}
		}

		/// <summary>
		/// Событие, которое выполняется при появлении свободного исполнителя.
		/// </summary>
		public void OnFreeJob()
		{
			var key = _awaitingExecutionCommands.Keys.FirstOrDefault();
			if (!string.IsNullOrEmpty(key))
			{
				if (_awaitingExecutionCommands.TryRemove(key, out Command command))
				{
					_jobManager.AddCommand(command);
					if (!_executingCommands.TryAdd(key, command))
					{
						throw new NotImplementedException("DataFlowLogicsService.OnFreeJob _executingCommands Не удалось добавить.");
					}
					//Console.WriteLine("! AddNewCommandHeader _awaitingExecutionCommands->_executingCommands {0}", command.Header.CallstackToString());
				}
				else
				{
					throw new NotImplementedException("DataFlowLogicsService.OnFreeJob Не удалось извлечь.");
				}
			}
			else
			{
				key = _awaitingPreparationCommandHeaders.Keys.FirstOrDefault();
				if (!string.IsNullOrEmpty(key))
				{
					if (_awaitingPreparationCommandHeaders.TryRemove(key, out CommandHeader command_header))
					{
						_preparationCommandService.PrepareCommand(command_header);
						if (!_preparationCommandHeaders.TryAdd(key, command_header))
						{
							throw new NotImplementedException("DataFlowLogicsService.OnFreeJob _preparationCommandHeaders Не удалось добавить.");
						}
						//Console.WriteLine("! AddNewCommandHeader _awaitingPreparationCommandHeaders->_preparationCommandHeaders {0}", command_header.CallstackToString());
					}
					else
					{
						throw new NotImplementedException("DataFlowLogicsService.OnFreeJob Не удалось извлечь.");
					}
				}
			}
		}

		/// <summary>
		/// Событие, которое выполняется при завершения исполнения команды на исполнителе.
		/// </summary>
		/// <param name="command">Команда, выполнение которой было завершено.</param>
		public void OnExecutedCommand(Command command)
		{
			//Console.WriteLine("! OnExecutedCommand {0}", command.Header.CallstackToString());
			
			var key = command.Header.CallstackToString();
			if (!(command.Function is ControlFunction))
			{
				_preparationCommandService.OnDataReady(command.OutputData.Header.Token);
			}

			if (_executingCommands.TryRemove(key, out Command removed_command))
			{
				if (!_executedCommands.TryAdd(key, removed_command))
				{
					throw new NotImplementedException("DataFlowLogicsService.OnExecutedCommand Не удалось добавить.");
				}
				//Console.WriteLine("! AddNewCommandHeader _executingCommands->_executedCommands {0}", removed_command.Header.CallstackToString());
			}
			else
			{
				throw new NotImplementedException("DataFlowLogicsService.OnExecutedCommand Не удалось извлечь.");
			}
		}

		/// <summary>
		/// Событие, которое выполняется при получении готовой к выполнению команды.
		/// </summary>
		/// <param name="command">Команда готовая к выполнению.</param>
		public void OnPreparedCommand(Command command)
		{
			//Console.WriteLine("! OnPreparedCommand {0}", command.Header.CallstackToString());

			if (command.ConditionData.Any(x => x.HasValue == null || !x.HasValue.Value || !(bool)x.Data))
			{
				throw new Exception("Невозможное событие. Проверка для отладки.");
			}

			if (_preparationCommandHeaders.TryRemove(command.Header.Token, out CommandHeader removed_command_header))
			{
				if (_jobManager.HasFreeJob())
				{
					_jobManager.AddCommand(command);
					if (!_executingCommands.TryAdd(command.Header.Token, command))
					{
						throw new NotImplementedException("DataFlowLogicsService.OnPreparedCommand true Не удалось добавить.");
					}
					//Console.WriteLine("! AddNewCommandHeader _preparationCommandHeaders->_executingCommands {0}", command.Header.CallstackToString());
				}
				else
				{
					if (!_awaitingExecutionCommands.TryAdd(command.Header.Token, command))
					{
						throw new NotImplementedException("DataFlowLogicsService.OnPreparedCommand false Не удалось добавить.");
					}
					//Console.WriteLine("! AddNewCommandHeader _preparationCommandHeaders->_awaitingExecutionCommands {0}", command.Header.CallstackToString());
				}
			}
			else
			{
				throw new NotImplementedException("DataFlowLogicsService.OnPreparedCommand Не удалось извлечь.");
			}
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Bodies.Commands;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Job;

namespace Core.Model.DataFlowLogics.Logics.Service
{
	public class DataFlowLogicsService : IDataFlowLogicsService
	{
		private readonly ConcurrentDictionary<string, CommandHeader> _allCommandHeaders;

		private readonly ConcurrentDictionary<string, CommandHeader> _awaitingPreparationCommandHeaders;
		private readonly ConcurrentDictionary<string, CommandHeader> _preparationCommandHeaders;

		private readonly ConcurrentDictionary<string, Command> _awaitingExecutionCommands;
		private readonly ConcurrentDictionary<string, Command> _executingCommands;

		private ConcurrentDictionary<string, CommandHeader> _searchAnotherExecutorCommands;
		private ConcurrentDictionary<string, Command> _searchAnotherExecutorCommandHeaders;

		private readonly ConcurrentDictionary<string, Command> _executedCommands;


		private readonly IJobManager _jobManager;

		private readonly IPreparationCommandService _preparationCommandService;

		public DataFlowLogicsService(IJobManager job_manager, IPreparationCommandService preparation_command_service)
		{
			_allCommandHeaders = new ConcurrentDictionary<string, CommandHeader>();

			_awaitingPreparationCommandHeaders = new ConcurrentDictionary<string, CommandHeader>();
			_preparationCommandHeaders = new ConcurrentDictionary<string, CommandHeader>();

			_awaitingExecutionCommands = new ConcurrentDictionary<string, Command>();
			_executingCommands = new ConcurrentDictionary<string, Command>();

			_searchAnotherExecutorCommands = new ConcurrentDictionary<string, CommandHeader>();
			_searchAnotherExecutorCommandHeaders = new ConcurrentDictionary<string, Command>();

			_executedCommands = new ConcurrentDictionary<string, Command>();

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
			Console.WriteLine("! AddNewCommandHeader {0}", command_header.CallstackToString());
			
			var key = command_header.CallstackToString();
			if (_allCommandHeaders.ContainsKey(key))
			{
				CommandHeader header;
				if (_allCommandHeaders.TryGetValue(key, out header))
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
				if (!_allCommandHeaders.TryAdd(key, command_header))
				{
					throw new NotImplementedException("DataFlowLogicsService.AddNewCommandHeader _allCommandHeaders Не удалось добавить.");
				}

				if (_jobManager.HasFreeJob())
				{
					if (!_preparationCommandHeaders.TryAdd(key, command_header))
					{
						throw new NotImplementedException("DataFlowLogicsService.AddNewCommandHeader _rawCommandHeaders Не удалось добавить.");
					}
					_preparationCommandService.PrepareCommand(command_header);
					
					Console.WriteLine("! AddNewCommandHeader _preparationCommandHeaders {0}", command_header.CallstackToString());
				}
				else
				{
					if (!_awaitingPreparationCommandHeaders.TryAdd(key, command_header))
					{
						throw new NotImplementedException("DataFlowLogicsService.AddNewCommandHeader _rawCommandHeaders Не удалось добавить.");
					}
				}
				
			}
		}


		public void OnFreeJob()
		{
			var key = _awaitingExecutionCommands.Keys.FirstOrDefault();
			if (!string.IsNullOrEmpty(key))
			{
				Command command;
				if (_awaitingExecutionCommands.TryRemove(key, out command))
				{
					_jobManager.AddCommand(command);
					if (!_executingCommands.TryAdd(key, command))
					{
						throw new NotImplementedException("DataFlowLogicsService.OnFreeJob _executingCommands Не удалось добавить.");
					}
					Console.WriteLine("! AddNewCommandHeader _awaitingExecutionCommands->_executingCommands {0}", command.Header.CallstackToString());
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
					CommandHeader command_header;
					if (_awaitingPreparationCommandHeaders.TryRemove(key, out command_header))
					{
						_preparationCommandService.PrepareCommand(command_header);
						if (!_preparationCommandHeaders.TryAdd(key, command_header))
						{
							throw new NotImplementedException("DataFlowLogicsService.OnFreeJob _preparationCommandHeaders Не удалось добавить.");
						}
						Console.WriteLine("! AddNewCommandHeader _awaitingPreparationCommandHeaders->_preparationCommandHeaders {0}", command_header.CallstackToString());
					}
					else
					{
						throw new NotImplementedException("DataFlowLogicsService.OnFreeJob Не удалось извлечь.");
					}
				}
			}
		}

		public void OnExecutedCommand(Command command)
		{
			Console.WriteLine("! OnExecutedCommand {0}", command.Header.CallstackToString());
			
			var key = command.Header.CallstackToString();
			_preparationCommandService.OnDataReady((DataCellHeader)command.OutputData.Header);
			Command removed_command;
			if (_executingCommands.TryRemove(key, out removed_command))
			{
				if (!_executedCommands.TryAdd(key, removed_command))
				{
					throw new NotImplementedException("DataFlowLogicsService.OnExecutedCommand Не удалось добавить.");
				}
				Console.WriteLine("! AddNewCommandHeader _executingCommands->_executedCommands {0}", removed_command.Header.CallstackToString());
			}
			else
			{
				throw new NotImplementedException("DataFlowLogicsService.OnExecutedCommand Не удалось извлечь.");
			}
		}

		public void OnPreparedCommand(Command command)
		{
			Console.WriteLine("! OnPreparedCommand {0}", command.Header.CallstackToString());
			
			var key = command.Header.CallstackToString();
			CommandHeader removed_command_header;
			if (_preparationCommandHeaders.TryRemove(key, out removed_command_header))
			{
				if (_jobManager.HasFreeJob())
				{
					_jobManager.AddCommand(command);
					if (!_executingCommands.TryAdd(key, command))
					{
						throw new NotImplementedException("DataFlowLogicsService.OnPreparedCommand true Не удалось добавить.");
					}
					Console.WriteLine("! AddNewCommandHeader _preparationCommandHeaders->_executingCommands {0}", command.Header.CallstackToString());
				}
				else
				{
					if (!_awaitingExecutionCommands.TryAdd(key, command))
					{
						throw new NotImplementedException("DataFlowLogicsService.OnPreparedCommand false Не удалось добавить.");
					}
					Console.WriteLine("! AddNewCommandHeader _preparationCommandHeaders->_awaitingExecutionCommands {0}", command.Header.CallstackToString());
				}
			}
			else
			{
				throw new NotImplementedException("DataFlowLogicsService.OnPreparedCommand Не удалось извлечь.");
			}
		}
	}
}

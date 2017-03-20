using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;
using Core.Model.Repository;

namespace Core.Model.Computing
{
	/// <summary>
	/// Сервис команд. Выполняет операции с командами.
	/// </summary>
	public class CommandService : ICommandService
	{
		private IDataCellRepository _dataCellRepository;

		private IFunctionRepository _functionRepository;

		private ICommandRepository _commandRepository;

		public CommandService(IFunctionRepository function_repository, IDataCellRepository data_cell_repository, ICommandRepository command_repository)
		{
			_dataCellRepository = data_cell_repository;
			_functionRepository = function_repository;
			_commandRepository = command_repository;
		}



		/// <summary>
		/// Создает команду из заголовка. Если данных недостаточно, то возвращает <code>null</code>.
		/// </summary>
		/// <param name="command_header"></param>
		/// <param name="not_ready_data"></param>
		/// <returns></returns>
		public Command CreateCommand(CommandHeader command_header, out Tuple<FunctionHeader, List<DataCellHeader>> not_ready_data)
		{
			not_ready_data = null;
			var input_data = new List<DataCell>();
			var empty_input_data = new List<DataCellHeader>();

			foreach (var data_cell_header in command_header.InputDataHeaders)
			{
				var data = _dataCellRepository.Get(new[] { data_cell_header }).FirstOrDefault();

				if (data != null)
				{
					input_data.Add(data);
				}
				else
				{
					empty_input_data.Add(data_cell_header);
				}
			}

			var output_data = _dataCellRepository.Get(new[] { command_header.OutputDataHeader }).FirstOrDefault();

			if (output_data == null)
			{
				output_data = new DataCell()
				{
					Header = command_header.OutputDataHeader,
					HasValue = false,
					Data = null
				};
			}

			var function = _functionRepository.Get(new[] { command_header.FunctionHeader }).FirstOrDefault();

			if (empty_input_data.Any() || function == null)
			{
				not_ready_data = new Tuple<FunctionHeader, List<DataCellHeader>>(function == null ? command_header.FunctionHeader : null, empty_input_data);
				return null;
			}

			return new Command()
			{
				Function = function,
				InputData = input_data,
				OutputData = output_data
			};
		}



		private Tuple<FunctionHeader, List<DataCellHeader>> InvokeOrSendToWait(CommandHeader command_headers, Action<Command> invoke_method)
		{
			Tuple<FunctionHeader, List<DataCellHeader>> not_ready_data;
			var command = CreateCommand(command_headers, out not_ready_data);

			if (command == null)
			{
				return not_ready_data;
			}
			
			invoke_method(command);

			return null;
		}

		/// <summary>
		/// Проверяет наличие всех данных для выполнения команды.
		/// Если все данные доступны, то отправляет на исполнение.
		/// Если какие либо данные отсутствуют, то запрашивает их у других узлов.
		/// </summary>
		/// <param name="command_headers"></param>
		/// <param name="invoke_method"></param>
		public void PrepareAndInvokeCommands(IEnumerable<CommandHeader> command_headers, Action<Command> invoke_method)
		{
			var requested_functions = new List<FunctionHeader>();
			var requested_data_cells = new List<DataCellHeader>();

			foreach (var command_header in command_headers)
			{
				var not_ready_data = InvokeOrSendToWait(command_header, invoke_method);
				if (not_ready_data != null)
				{
					if (not_ready_data.Item1 != null)
					{
						requested_functions.Add(not_ready_data.Item1);
						var header = command_header;
						_functionRepository.Subscribe(new[] { not_ready_data.Item1 }, (data_header) => { InvokeOrSendToWait(header, invoke_method); });
					}

					if (not_ready_data.Item2.Any())
					{
						requested_data_cells.AddRange(not_ready_data.Item2);
						var header = command_header;
						_dataCellRepository.Subscribe(not_ready_data.Item2, (function_header) => { InvokeOrSendToWait(header, invoke_method); });

						foreach (var h in not_ready_data.Item2)
						{
							Console.WriteLine(string.Format("CommandService.PrepareAndInvokeCommands отсутствуют данные для вычисления {0}. {1}", command_header.FunctionHeader.Name, string.Join("/", h.CallStack)));
						}
					}
				}
			}

			if (requested_data_cells.Any())
			{
				RequestDataCells(requested_data_cells);
			}

			if (requested_functions.Any())
			{
				RequestFunctions(requested_functions);
			}
		}

		/// <summary>
		/// Запрашивает данные у других узлов.
		/// </summary>
		/// <param name="data_cell_headers"></param>
		private void RequestDataCells(IEnumerable<DataCellHeader> data_cell_headers)
		{
			foreach (var data_cell_header in data_cell_headers)
			{
				DataCell data_cell = null;

				foreach (var owner in data_cell_header.Owners)
				{
					data_cell = GetDataCellRequest(data_cell_header, owner.IpAddress);
					if (data_cell != null)
					{
						break;
					}
				}
				/*
				if (data_cell != null)
				{
					_dataCellRepository.Add(new[] { data_cell });
					break;
				}
				else
				{
					// TODO: Нужно что то делать если ни у кого из владельцев нет нужных данных.
					throw new NotImplementedException("Нужно что то делать если ни у кого из владельцев нет нужных данных.");
				}*/
			}
		}

		/// <summary>
		/// Запрос данных у другого узла.
		/// </summary>
		/// <param name="data_cell_header"></param>
		/// <param name="ip_address"></param>
		/// <returns></returns>
		private DataCell GetDataCellRequest(DataCellHeader data_cell_header, IPAddress ip_address)
		{
			throw new NotImplementedException("Запрос данных у других узлов ещё не реализован.");
		}

		/// <summary>
		/// Запрашивает функции у других узлов.
		/// </summary>
		/// <param name="function_headers"></param>
		private void RequestFunctions(IEnumerable<FunctionHeader> function_headers)
		{
			foreach (var function_header in function_headers)
			{
				DataCell data_cell = null;

				foreach (var owner in function_header.Owners)
				{
					data_cell = GetFunctionRequest(function_header, owner.IpAddress);
					if (data_cell != null)
					{
						break;
					}
				}

				if (data_cell != null)
				{
					_dataCellRepository.Add(new[] { data_cell });
					break;
				}
				else
				{
					// TODO: Нужно что то делать если ни у кого из владельцев нет нужных данных.
					throw new NotImplementedException("Нужно что то делать если ни у кого из владельцев нет нужных данных.");
				}
			}
		}

		/// <summary>
		/// Запрос функции у другого узла.
		/// </summary>
		/// <param name="function_header"></param>
		/// <param name="ip_address"></param>
		/// <returns></returns>
		private DataCell GetFunctionRequest(FunctionHeader function_header, IPAddress ip_address)
		{
			throw new NotImplementedException("Запрос функций у других узлов ещё не реализован.");
		}
	}
}

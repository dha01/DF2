﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.CodeExecution.Repository;

namespace Core.Model.DataFlowLogics.Logics.Service
{
	public class PreparationCommandService : IPreparationCommandService
	{

		public Action<Command> OnPreparedCommand { get; set; }

		private IDataCellRepository _dataCellRepository;

		private IFunctionRepository _functionRepository;

		private ConcurrentDictionary<string, Command> _preparingCommands { get; set; }
		private ConcurrentDictionary<string, CommandHeader> _waitConditionCommands { get; set; } = new ConcurrentDictionary<string, CommandHeader>();

		private ConcurrentDictionary<string, List<string>> _dataLinksWithCommands = new ConcurrentDictionary<string, List<string>>();

		public PreparationCommandService(IDataCellRepository data_cell_repository, IFunctionRepository function_repository)
		{
			_preparingCommands = new ConcurrentDictionary<string, Command>();
			_dataCellRepository = data_cell_repository;
			_functionRepository = function_repository;
		}

		private void AddCommandToPreparing(Command new_command)
		{
			if (_preparingCommands.TryAdd(new_command.Header.Token, new_command))
			{
				//throw new NotImplementedException("PreparationCommandService.PrepareCommand не удалось добавить.");
			}

			foreach (var input_data in new_command.InputData)
			{
				if (_dataLinksWithCommands.ContainsKey(input_data.Header.Token))
				{
					_dataLinksWithCommands[input_data.Header.Token].Add(new_command.Header.Token);
				}
				else
				{
					_dataLinksWithCommands[input_data.Header.Token] = new List<string>() { new_command.Header.Token };
				}
			}
		}

		private DataCell GetOutputData(DataCellHeader data_cell_header)
		{
			var output_data = _dataCellRepository.Get(new[] { data_cell_header }).FirstOrDefault();
			if (output_data == null)
			{
				output_data = new DataCell()
				{
					Header = data_cell_header,
					HasValue = null,
					Data = null
				};
				_dataCellRepository.Add(new[] { output_data });
			}

			return output_data;
		}

		private DataCell GetDataCell(DataCellHeader data_cell_header)
		{
			var data_cell = _dataCellRepository.Get(new[] { data_cell_header }).FirstOrDefault();
			if (data_cell == null)
			{
				return new DataCell()
				{
					Header = data_cell_header,
					HasValue = null,
					Data = null
				};

				//_dataCellRepository.Subscribe(data_cell_header, OnDataReady);
				// TODO: нужно отправлять запросы за другие узлы для получения данных.
			}
			else
			{
				return data_cell;
			}
		}

		private bool CheckCommandCodition(CommandHeader command_header)
		{
			if (command_header.ConditionDataHeaders.Any() && !command_header.ConditionDataHeaders.Any(x =>
			{
				var data = GetDataCell(x);
				return data.HasValue != null && data.HasValue.Value;
			}))
			{
				/*var new_commandd = new Command()
				{
					Header = new InvokeHeader()
					{
						CallStack = command_header.CallStack
					}
				};*/
				if (_waitConditionCommands.TryAdd(command_header.Token, command_header))
				{
					//throw new NotImplementedException("PreparationCommandService.CheckCommandCodition не удалось добавить.");
				}

				foreach (var input_dataa in command_header.ConditionDataHeaders)
				{
					if (_dataLinksWithCommands.ContainsKey(input_dataa.Token))
					{
						_dataLinksWithCommands[input_dataa.Token].Add(command_header.Token);
					}
					else
					{
						_dataLinksWithCommands[input_dataa.Token] = new List<string>() { command_header.Token };
					}
				}

				return false;
			}

			return true;
		}

		private Command CreateNewCommand(CommandHeader command_header)
		{
			if (!CheckCommandCodition(command_header))
			{
				return null;
			}

			// Получаем выходную ячейку данных.
			var output_data = GetOutputData(command_header.OutputDataHeader);

			// Подготавливае места для ячеек с выходными данными.
			var count = command_header.InputDataHeaders.Count;
			var input_data = new List<DataCell>(count);
			for (int i = 0; i < count; i++)
			{
				input_data.Add(null);
			}

			var new_command = new Command()
			{
				Header = new InvokeHeader()
				{
					CallStack = command_header.CallStack
				},
				InputData = input_data,
				OutputData = output_data,
				ConditionData = command_header.ConditionDataHeaders.Select(GetDataCell).ToList()
			};

			if (new_command.ConditionData.Any(x => !(bool)x.Data))
			{
				return null;
			}

			var all_ready = true;
			var any_ready = false;

			switch (command_header.FunctionHeader.Condition)
			{
				case InputParamCondition.All:
					// Получаем или подписываемся на получение входных параметров.
					for (int i = 0; i < command_header.InputDataHeaders.Count; i++)
					{
						var result = GetDataCell(command_header.InputDataHeaders[i]);
						new_command.InputData[i] = result;

						if (result.HasValue == null)
						{
							all_ready = false;
						}
					}
					break;
				case InputParamCondition.Any:
					var any = false;
					// Получаем или подписываемся на получение входных параметров.
					for (int i = 0; i < command_header.InputDataHeaders.Count; i++)
					{
						var result = GetDataCell(command_header.InputDataHeaders[i]);
						new_command.InputData[i] = result;

						if (result.HasValue != null && result.HasValue.Value)
						{
							any = true;
						}
					}

					if (!any)
					{
						all_ready = false;
					}
					break;
				case InputParamCondition.Iif:
					var condition = GetDataCell(command_header.InputDataHeaders[0]);
					var if_true = GetDataCell(command_header.InputDataHeaders[1]);
					var if_false = GetDataCell(command_header.InputDataHeaders[2]);

					new_command.InputData[0] = condition;
					new_command.InputData[1] = if_true;
					new_command.InputData[2] = if_false;

					if (condition.HasValue == null ||
						(bool)condition.Data && condition.HasValue == null ||
						!(bool)condition.Data && condition.HasValue == null)
					{
						all_ready = false;
					}
					break;
				default:
					throw new Exception($"CreateNewCommand Неизвестный тип: {command_header.FunctionHeader.Condition}");
			}

			if (!all_ready)
			{
				AddCommandToPreparing(new_command);
			}
			_dataCellRepository.Add(new_command.InputData.Where(x => x.HasValue == null), false);

			// Получаем или подписываемся на получение функций.
			var function_header = new[] { command_header.FunctionHeader };
			var function = _functionRepository.Get(function_header).FirstOrDefault();
			if (function == null)
			{
				if (all_ready)
				{
					all_ready = false;
					AddCommandToPreparing(new_command);
				}
				//_functionRepository.Subscribe(function_header, OnFunctionReady);
				// TODO: нужно отправлять запросы на другие узлы для получения функции.
			}
			else
			{
				new_command.Function = function;
			}

			// Если команда готова, то возвращаем её.
			if (all_ready)
			{
				return new_command;
			}
			else
			{
				return null;
			}
		}

		public void PrepareCommand(CommandHeader command_header)
		{
			if (_preparingCommands.ContainsKey(command_header.Token))
			{
				throw new NotImplementedException("PreparationCommandService.PrepareCommand Такая команда уже находится в подготовке");
			}
			
			// Добавляем новую команду, находящуюся в процессе подготовки.
			var new_command = CreateNewCommand(command_header);
			if (new_command != null)
			{
				OnPreparedCommand(new_command);
			}
		}

		public void OnDataReady(DataCellHeader data_cell_header)
		{
			var data_cell_headers = new[] { data_cell_header };
			var data_cell = _dataCellRepository.Get(data_cell_headers).FirstOrDefault();

			if (data_cell != null && data_cell.HasValue != null)
			{
				//if (data_cell.HasValue != null)
				//{
					if (_dataLinksWithCommands.ContainsKey(data_cell_header.Token))
					{
						var command_tokens = _dataLinksWithCommands[data_cell_header.Token].ToList();
						foreach (var command_token in command_tokens)
						{
							if (_preparingCommands.ContainsKey(command_token))
							{
								var command = _preparingCommands[command_token];
								command.InputData = _dataCellRepository.Get(command.InputData.Select(x => (DataCellHeader) x.Header)).ToList();

								if (command.Function != null)
								{
									switch (((FunctionHeader) command.Function.Header).Condition)
									{
										case InputParamCondition.All:
											if (command.InputData.All(x => x.HasValue != null && x.HasValue.Value))
											{
												break;
											}
											return;
										case InputParamCondition.Any:
											if (command.InputData.Any(x => x.HasValue != null && x.HasValue.Value))
											{
												break;
											}
											return;
										case InputParamCondition.Iif:
											if (command.InputData[0].HasValue != null &&
											    ((bool) command.InputData[0].Data && command.InputData[1].HasValue != null
												 || !(bool) command.InputData[0].Data && command.InputData[2].HasValue != null))
											{
												break;
											}
											return;
										default:
											throw new Exception($"OnDataReady Неизвестный тип: {((FunctionHeader) command.Function.Header).Condition}");
									}

									if (_preparingCommands.TryRemove(command_token, out command))
									{
										_dataLinksWithCommands[data_cell_header.Token].Remove(command_token);
										if (_dataLinksWithCommands[data_cell_header.Token].Count == 0)
										{
											_dataLinksWithCommands.TryRemove(data_cell_header.Token, out List<string> str);
										}
										OnPreparedCommand(command);
									}
								}
							}

							if (_waitConditionCommands.ContainsKey(command_token))
							{
								if ((bool) data_cell.Data)
								{
									var new_command = CreateNewCommand(_waitConditionCommands[command_token]);
									if (new_command != null)
									{
										OnPreparedCommand(new_command);
									}

									_waitConditionCommands.TryRemove(command_token, out CommandHeader token);
								}
								else
								{
									//_waitConditionCommands.TryRemove(command_token, out CommandHeader token);
								}
							}
						}
					}
				/*}
				else
				{
					throw new NotImplementedException("Нужно что то делать если данные не придут.");
				}*/
			}
		}

		public void OnFunctionReady(FunctionHeader function_header)
		{
			var function = _functionRepository.Get(new[] { function_header }).FirstOrDefault();
			if (function == null)
			{
				_functionRepository.Subscribe(new[] { function_header }, OnFunctionReady);
			}
			else
			{
				if (_preparingCommands.ContainsKey(function_header.Token))
				{
					var command = _preparingCommands[function_header.Token];
					command.Function = function;
					if (command.InputData.All(x => x.HasValue != null) && command.Function != null)
					{
						if (_preparingCommands.TryRemove(function_header.Token, out command))
						{
							OnPreparedCommand(command);
						}
					}
				}
			}
		}
	}
}

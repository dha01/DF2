﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.Model.CodeCompiler.Code;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.CodeExecution.Repository;
using Core.Model.CodeExecution.Service.Execution;
using Core.Model.DataFlowLogics.BlockChain.DataModel;
using Core.Model.DataFlowLogics.BlockChain.Repository;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
	public class TransactionExecutorService : ITransactionExecutorService
	{
		private readonly IDataCellHashService _dataCellHashService;
		private readonly IFunctionHashService _functionHashService;

		private readonly IExecutionService _executionService;
		private readonly ITransactionPoolService _transactionPoolService;

		public TransactionExecutorService(IDataCellHashService data_cell_hash_service, IFunctionHashService function_hash_service, IExecutionService execution_service, ITransactionPoolService transaction_pool_service)
		{
			_dataCellHashService = data_cell_hash_service;
			_functionHashService = function_hash_service;
			_executionService = execution_service;
			_transactionPoolService = transaction_pool_service;
		}


		Object obj = new Object();
		public void Execute(Transaction transaction)
		{
			var actions = new Dictionary<Type, Action<Transaction>> {
				{ typeof(ExecutionTransaction), et => ExecuteExecutionTransaction((ExecutionTransaction)et) },
				{ typeof(ExecutionCompliteTransaction), ect => ExecuteExecutionCompliteTransaction((ExecutionCompliteTransaction)ect) },
				{ typeof(GCTransaction), gct => ExecuteGCTransaction((GCTransaction)gct) },
			};

			actions[transaction.GetType()].Invoke(transaction);
		}

		private void ExecuteExecutionTransaction(ExecutionTransaction execution_transaction)
		{
			
				var function = _functionHashService.GetLocal(execution_transaction.Function);

				if (function == null)
				{
					// TODO: поиск на других узлах пока не реализован.
					_functionHashService.GetGlobal(execution_transaction.Function);
					return;
				}
			
				if (function.Function is ControlFunction)
				{
					
					ExecuteControlExecutionTransaction(function, execution_transaction);
					
				}
				else
				{
					ExecuteCalculateExecutionTransaction(function.Function, execution_transaction);
				}
		}

		//private static SHA512 _mySha = SHA512.Create();

		private void ExecuteControlExecutionTransaction(FunctionHash control_function_hash, ExecutionTransaction execution_transaction)
		{

			
			var control_function = (ControlFunction) control_function_hash.Function;

			if (execution_transaction.Temps == null)
			{
				execution_transaction.Temps = new string[control_function.Commands.Count()];
			}
			var _mySha = SHA512.Create();

			var const_hashs = new List<string>();
			for (int i = 0; i < control_function.Constants.Count; i++)
			{
				const_hashs.Add(Convert.ToBase64String(_mySha.ComputeHash(Encoding.ASCII.GetBytes($"{control_function.Token.Hash}/Const_{i}"))));
			}/*

			if (((FunctionHeader)control_function_hash.Function.Header).Name == "Fib")
			{
				if (!const_hashs.Contains(
					    "5HEgQyCy9C6ss9D78MFjAm6NmWarWW5Saw84hRfd7bi/49NMPBL6NpMSuMuc4MumL6lvAs1oB7gQ2kJMwgDJNg==") ||
				    !const_hashs.Contains(
					    "ND3DNjWFz3uZb1mLseuAp4R+MI2aJj4bvbRl9S9PVWxlljLFflCnuNCUvEiEQx0aMBA34kxVt2r4KbAVusfPrw=="))
				{
					var g = 5;

					var const_hashs2 = new List<string>();
					for (int i = 0; i < control_function.Constants.Count; i++)
					{
						var str = $"{control_function.Token.Hash}/Const_{i}";
						const_hashs2.Add(Convert.ToBase64String(_mySha.ComputeHash(Encoding.ASCII.GetBytes(str))));
					}
				}
			}*/


			var temps = execution_transaction.Inputs.Concat(execution_transaction.Temps).Concat(const_hashs).ToArray();

			
			var new_transactions = new List<Transaction>();
			
			int j = -1;
			foreach (var command in control_function.Commands)
			{
				j++;
				// Если результат выполнения команды уже получен, то пропускаем.
				if (execution_transaction.Temps[j] != null)
				{
					continue;
				}

				// Если не выполняется ни одного условия, то пропускаем команду.
				if (command.ConditionId.Any() && command.ConditionId.Any(x =>
							temps[x] != "True" // Условие уже получено и истинно
					))
				{
					continue;
				}

				// Если недостаточно входных параметров, то пропускаем команду.
				if (command.FunctionHeader.Condition == InputParamCondition.All && command.InputDataIds.Any(x => temps[x] == null))
				{
					continue;
				}

				// Если недостаточно входных параметров, то пропускаем команду.
				if (command.FunctionHeader.Condition == InputParamCondition.Any && command.InputDataIds.All(x => temps[x] == null))
				{
					continue;
				}
				/*if (temps.Any(x => x == "3aRuH6jAgZZLJib4Hu/V8A3/DY9GZVZ9dkQMiQyNPh50PYi4hYfJShf1m5gNKnzwwDOPS4IwEANOKLsHvP91Ag==" ||
					x == "roZvVkCSbMOgpSN2QunWLHG0vK9kmoumJMhDVazHdJUHp7VfD3dri61EH80A+IXMGVvX8cUeLR2j+BmeMDPMFw==" ||
				    x == "IY2X7o9K83b7pifWQDnlgVA9PRiS8mD+AKapMRu6qvIkxvIuHgoZ/UdEQF0sqiYmMfRHjX5aGKk/BPsu3OIvqw==" ||
				    x == "SRcp7p3rMPZwu55qVVPHUA0i5Zc5uOAK2Kk3+FSiTjsjbv1DVXg4yUe8fS3qKTbrgGaX6gRE7CSQ4+BBkhfHYg=="))
				{
					var g = 5;
				}*/


				var new_transaction =
					new ExecutionTransaction
					{
						Function = command.FunctionHash,
						Index = j,
						Temps = null,
						TaskHash = execution_transaction.TaskHash,
						Inputs = command.InputDataIds.Select(x => temps[x]).ToArray(),
						IsInitial = true,
						ParentTransaction = execution_transaction.Hash,
						ParentFunction = execution_transaction.Function
					};
				/*
				if (((FunctionHeader)control_function_hash.Function.Header).Name == "Fib")
				{
					if (!temps.Contains(
						    "5HEgQyCy9C6ss9D78MFjAm6NmWarWW5Saw84hRfd7bi/49NMPBL6NpMSuMuc4MumL6lvAs1oB7gQ2kJMwgDJNg==") ||
					    !temps.Contains(
						    "ND3DNjWFz3uZb1mLseuAp4R+MI2aJj4bvbRl9S9PVWxlljLFflCnuNCUvEiEQx0aMBA34kxVt2r4KbAVusfPrw=="))
					{
						var g = 5;

						var const_hashs2 = new List<string>();
						for (int i = 0; i < control_function.Constants.Count; i++)
						{
							var str = $"{control_function.Token.Hash}/Const_{i}";
							const_hashs2.Add(Convert.ToBase64String(_mySha.ComputeHash(Encoding.ASCII.GetBytes(str))));
						}
					}
				}*/
				new_transactions.Add(new_transaction);
			}
			lock (obj)
			{
				if (_transactionPoolService.TryGetFromPool(execution_transaction.Hash, out Transaction exists_transaction))
				{
					_transactionPoolService.UpdateToPool((ExecutionTransaction) exists_transaction + execution_transaction);
				}
				else
				{
					execution_transaction.IsInitial = false;
					_transactionPoolService.AddToPool(execution_transaction);
				}

				_transactionPoolService.EnqueueToPreparation(1, new_transactions.ToArray());
			}
		}

		private void ExecuteCalculateExecutionTransaction(Function calculate_function, ExecutionTransaction execution_transaction)
		{
			lock (obj)
			{

				if (_transactionPoolService.TryGetFromPool(execution_transaction.Hash, out Transaction exists_transaction))
				{
					_transactionPoolService.AddToPool((ExecutionTransaction) exists_transaction + execution_transaction);
					return;
				}

				if (!execution_transaction.IsInitial)
				{
					_transactionPoolService.AddToPool(execution_transaction);
					return;
				}

				var inputs = _dataCellHashService.GetLocal(execution_transaction.Inputs).ToList();

				if (((FunctionHeader) calculate_function.Header).Condition == InputParamCondition.All &&
				    inputs.Any(x => x == null) ||
				    ((FunctionHeader) calculate_function.Header).Condition == InputParamCondition.Any && inputs.All(x => x == null))
				{
					// TODO: поиск на других узлах пока не реализован.
					_dataCellHashService.GetGlobal(execution_transaction.Inputs.Where(x => x != null).ToArray());
					return;
				}

				var output = new DataCell();
				_executionService.Execute(calculate_function, inputs.Select(x => x != null
					? new DataCell
					{
						Data = x.Value,
						HasValue = true
					}
					: null), output);


				string output_hash;
				if (calculate_function is BasicFunction &&
				    ((BasicFunctionHeader) ((BasicFunction) calculate_function).Header).Name == "IsTrue" ||
				    ((BasicFunctionHeader) ((BasicFunction) calculate_function).Header).Name == "IsFalse")
				{
					output_hash = output.Data.ToString();
				}
				else
				{
					output_hash = execution_transaction.GetOutputHash();
					_dataCellHashService.Set(new DataCellHash
					{
						Hash = output_hash,
						Value = output.Data,
						Type = output.Data.GetType().ToString()
					});
				}

				execution_transaction.IsInitial = false;
				_transactionPoolService.AddToPool(execution_transaction);
				_transactionPoolService.EnqueueToPreparation(1,
					new ExecutionCompliteTransaction(execution_transaction.ParentTransaction)
					{
						Index = execution_transaction.Index,
						TaskHash = execution_transaction.TaskHash,
						Temp = output_hash
					});
			}
		}


		private void ExecuteExecutionCompliteTransaction(ExecutionCompliteTransaction execution_complite_transaction)
		{
			lock (obj)
			{
				if (_transactionPoolService.TryGetFromPool(execution_complite_transaction.Hash, out Transaction transaction))
				{
					var execution_transaction = (ExecutionTransaction) transaction;
					_transactionPoolService.UpdateToPool((ExecutionTransaction) transaction + execution_complite_transaction);

					if (execution_complite_transaction.Index == 0)
					{
						_transactionPoolService.EnqueueToPreparation(1,
							new ExecutionCompliteTransaction(execution_transaction.ParentTransaction)
							{
								Index = execution_transaction.Index,
								TaskHash = execution_transaction.TaskHash,
								Temp = execution_transaction.Temps[0]
							});
					}
					else
					{
						var function = _functionHashService.GetLocal(execution_transaction.Function);
						ExecuteControlExecutionTransaction(function, execution_transaction);
					}
				}
			}
		}

		private void ExecuteGCTransaction(GCTransaction gc_transaction)
		{
			_dataCellHashService.Remove(gc_transaction.DataCells.ToArray());
		}
	}
}

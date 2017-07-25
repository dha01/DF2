using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeCompiler.Code;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.Repository;
using Core.Model.CodeExecution.Service.Execution;
using Core.Model.DataFlowLogics.BlockChain.DataModel;
using Core.Model.DataFlowLogics.BlockChain.Repository;
using Core.Model.DataFlowLogics.BlockChain.Service;
using Core.Model.DataFlowLogics.InstructionExecutionConveyor.Job;
using NUnit.Framework;

namespace Core.Tests.Model.DataFlowLogics.BlockChain.Service
{
	[TestFixture]
	public class TransactionExecutorServiceTests
	{
		private static Assembly assembly = CommandBuilder.CreateFunctionFromSourceCode(@"
using Core.Model.CodeCompiler.Build.Attributes;
using Core.Model.CodeCompiler.Build.DataModel;
using Core.Model.CodeCompiler.Code;

namespace CustomNamespace
{
	public class CustomClass : ControlFunctionBase
	{
		[ControlFunction]
		public void Fib(Var<int> a)
		{

			//Return(Iif(a == 1 | a == 2, Const(1), Exec<int>(""Fib"", a - 1) + Exec<int>(""Fib"", a - 2)));

			var one = Const(1);
			var two = Const(2);
			Return(
				Iif(
					Exec<bool>(""Fib_labda_1"", a, one, two), 
					Exec<int>(""Fib_labda_2"", one), 
					Exec<int>(""Fib_labda_3"", a, one, two)
				));
		}

		[ControlFunction]
		public void Fib_labda_1(Var<int> a, Var<int> b, Var<int> c)
		{
			Return(a == b | a == c);
		}

		[ControlFunction]
		public void Fib_labda_2(Var<int> a)
		{
			Return(a);
		}

		[ControlFunction]
		public void Fib_labda_3(Var<int> a, Var<int> b, Var<int> c)
		{
			Return(Exec<int>(""Fib"", a - b) + Exec<int>(""Fib"", a - c));
		}
	}
}");

		[Test]
		public void BlockChainTest()
		{
			var one = Transaction.GetHash($"o07+HMx9CckGOC4XFxy1BGCzteX9XxNeyAuxIX/uEKJVDieH+06qHR9ENTsdnjMYVG6sksH0UsMIE2uuzo9kiA==/Const_0");
			
			var two = Transaction.GetHash($"o07+HMx9CckGOC4XFxy1BGCzteX9XxNeyAuxIX/uEKJVDieH+06qHR9ENTsdnjMYVG6sksH0UsMIE2uuzo9kiA==/Const_1");
			var three = Transaction.GetHash($"o07+HMx9CckGOC4XFxy1BGCzteX9XxNeyAuxIX/uEKJVDieH+06qHR9ENTsdnjMYVG6sksH0UsMIE2uuzo9kiA==/Const_0");


			var data_cell_repository = new DataCellHashRepository();
			var data_cell_service = new DataCellHashService(data_cell_repository);
			var function_service = new FunctionHashService(new FunctionHashRepository(), data_cell_repository);
			var transaction_pool_service = new TransactionPoolService();

			var ts = new TransactionExecutorService(
				data_cell_service,
				function_service,
				new ExecutionManager(new List<IExecutionService>
				{
					new BasicExecutionService(),
					new CSharpExecutionService(new FunctionRepository(new DataCellRepository()))
				}, new DataCellRepository()),
				transaction_pool_service);

			var input_value = new DataCellHash
			{
				Hash = Transaction.GetHash("User0/Process0/Input0"),
				Type = "int",
				Value = 20
			};

			var text = CommandBuilder.CompileMethodFromAssembly(assembly, "CustomNamespace.CustomClass.Fib");
			var text1 = CommandBuilder.CompileMethodFromAssembly(assembly, "CustomNamespace.CustomClass.Fib_labda_1");
			var text2 = CommandBuilder.CompileMethodFromAssembly(assembly, "CustomNamespace.CustomClass.Fib_labda_2");
			var text3 = CommandBuilder.CompileMethodFromAssembly(assembly, "CustomNamespace.CustomClass.Fib_labda_3");

			function_service.Set(BasicFunctionModel.AllMethods.Select(x => new FunctionHash
			{
				Function = x.Value.BasicFunction,
				Hash = x.Value.BasicFunction.Token.Hash
			}).ToArray());



			data_cell_service.Set(input_value);
			function_service.Set(new[]
			{
				new FunctionHash
				{
					Function = text,
					Hash = text.Token.Hash
				},
				new FunctionHash
				{
					Function = text1,
					Hash = text1.Token.Hash
				},
				new FunctionHash
				{
					Function = text2,
					Hash = text2.Token.Hash
				},
				new FunctionHash
				{
					Function = text3,
					Hash = text3.Token.Hash
				},
			});

			var task_hash = Transaction.GetHash("User0/Process0/");

			var first_transaction = new ExecutionTransaction
			{
				ParentFunction = null,
				Index = 0,
				Inputs = new[] {input_value.Hash},
				Temps = null,
				Function = text.Token.Hash,
				TaskHash = task_hash,
				IsInitial = true,
				ParentTransaction = null
			};
			
			var job_hash_manager = new JobHashManager(transaction_pool_service, ts);
			
			transaction_pool_service.EnqueueToPreparation(2, first_transaction);

/*
			bool a = false;
			bool b = false;

			Task.Run(() =>
			{
				while (!a || !b)
				{
					a = false;
					while (transaction_pool_service.TryDequeueToPreparation(out Transaction transaction))
					{
						
						ts.Execute(transaction.Clone());
					}
					a = true;
				}
			});
			Task.Run(() =>
			{
				while (!a || !b)
				{
					b = false;
					while (transaction_pool_service.TryDequeueToPreparation(out Transaction transaction))
					{
						ts.Execute(transaction.Clone());
					}
					b = true;
				}
			});*/


			var hash_result = transaction_pool_service.GetResultHash(first_transaction.Hash).Result;
			var result = data_cell_service.GetLocal(hash_result).FirstOrDefault()?.Value;
			/*if (first_transaction.Temps != null && first_transaction.Temps[0] != null)
			{
				var result = data_cell_service.GetLocal(first_transaction.Temps[0]).FirstOrDefault()?.Value;
			}*/
			Assert.Fail(result?.ToString());
		}
	}
}

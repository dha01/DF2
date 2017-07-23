using System;
using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.Repository;

namespace Core.Model.CodeExecution.Service.Execution
{
	/// <summary>
	/// Класс для управления исполняющими сервисами.
	/// </summary>
	public class ExecutionManager : IExecutionManager
	{

		private Dictionary<Type, IExecutionService> _availableExecutionServices;

		private IDataCellRepository _dataCellRepository;

		public ExecutionManager(IEnumerable<IExecutionService> execution_services, IDataCellRepository data_cell_repository)
		{
			_dataCellRepository = data_cell_repository;
			_availableExecutionServices = new Dictionary<Type, IExecutionService>();
			foreach (var execution_service in execution_services)
			{
				_availableExecutionServices.Add(execution_service.GetType(), execution_service);
			}
		}

		private void ThrowReturnedValue(DataCell output, Token callstack)
		{
			var par = Token.Parse(callstack.Last());
			if (par.Index == 0)
			{
				ThrowReturnedValue(output, callstack.Prev());
			}
			else
			{
				var result = callstack.Next("result");
				if (output.Token != result)
				{
					output.Header.Token = result;
				}
				_dataCellRepository.Add(new []{ output });
			}
		}

		public void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, Token? callstack = null)
		{
			if (function.GetType() == typeof(BasicFunction))
			{
				Execute((BasicFunction)function, input_data, output, callstack);
				//ThrowReturnedValue(output, callstack.Value);
			}
			else
			if (function.GetType() == typeof(ControlFunction))
			{
				Execute((ControlFunction)function, input_data, output, callstack);
			}
			else
			if (function.GetType() == typeof(CSharpFunction))
			{
				Execute((CSharpFunction)function, input_data, output, callstack);
			}
			else
			{
				throw new Exception(String.Format("Исполнение функций с типом {0} не реализовано.", function.GetType().Name));
			}
		}

		public void Execute(BasicFunction function, IEnumerable<DataCell> input_data, DataCell output, Token? callstack = null)
		{
			if (_availableExecutionServices.ContainsKey(typeof (BasicExecutionService)))
			{
				_availableExecutionServices[typeof(BasicExecutionService)].Execute(function, input_data, output, callstack);
			}
			else
			{
				throw new Exception(String.Format("Исполнитель базовых функций не доступен."));
			}
		}

		public void Execute(ControlFunction function, IEnumerable<DataCell> input_data, DataCell output, Token? callstack = null)
		{
			if (_availableExecutionServices.ContainsKey(typeof(BasicExecutionService)))
			{
				_availableExecutionServices[typeof(ControlExecutionService)].Execute(function, input_data, output, callstack);
			}
			else
			{
				throw new Exception(String.Format("Исполнитель управляющих функций не доступен."));
			}
		}

		public void Execute(CSharpFunction function, IEnumerable<DataCell> input_data, DataCell output, Token? callstack = null)
		{
			if (_availableExecutionServices.ContainsKey(typeof(BasicExecutionService)))
			{
				_availableExecutionServices[typeof(CSharpExecutionService)].Execute(function, input_data, output, callstack);
			}
			else
			{
				throw new Exception(String.Format("Исполнитель функций C# не доступен."));
			}
		}
	}
}

using System;
using System.Collections.Generic;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;

namespace Core.Model.Execution
{
	/// <summary>
	/// Класс для управления исполняющими сервисами.
	/// </summary>
	public class ExecutionManager : IExecutionManager
	{

		private Dictionary<Type, IExecutionService> _availableExecutionServices;

		public ExecutionManager(IEnumerable<IExecutionService> execution_services)
		{
			_availableExecutionServices = new Dictionary<Type, IExecutionService>();
			foreach (var execution_service in execution_services)
			{
				_availableExecutionServices.Add(execution_service.GetType(), execution_service);
			}
		}

		public void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null)
		{
			if (function.GetType() == typeof(BasicFunction))
			{
				Execute((BasicFunction)function, input_data, output, command_context);
			}
			else
			if (function.GetType() == typeof(ControlFunction))
			{
				Execute((ControlFunction)function, input_data, output, command_context);
			}
			else
			if (function.GetType() == typeof(CSharpFunction))
			{
				Execute((CSharpFunction)function, input_data, output, command_context);
			}
			else
			{
				throw new Exception(String.Format("Исполнение функций с типом {0} не реализовано.", function.GetType().Name));
			}
		}

		public void Execute(BasicFunction function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null)
		{
			if (_availableExecutionServices.ContainsKey(typeof (BasicExecutionService)))
			{
				_availableExecutionServices[typeof(BasicExecutionService)].Execute(function, input_data, output, command_context);
			}
			else
			{
				throw new Exception(String.Format("Исполнитель базовых функций не доступен."));
			}
		}

		public void Execute(ControlFunction function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null)
		{
			if (_availableExecutionServices.ContainsKey(typeof(BasicExecutionService)))
			{
				_availableExecutionServices[typeof(ControlExecutionService)].Execute(function, input_data, output, command_context);
			}
			else
			{
				throw new Exception(String.Format("Исполнитель управляющих функций не доступен."));
			}
		}

		public void Execute(CSharpFunction function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null)
		{
			if (_availableExecutionServices.ContainsKey(typeof(BasicExecutionService)))
			{
				_availableExecutionServices[typeof(CSharpExecutionService)].Execute(function, input_data, output, command_context);
			}
			else
			{
				throw new Exception(String.Format("Исполнитель функций C# не доступен."));
			}
		}
	}
}

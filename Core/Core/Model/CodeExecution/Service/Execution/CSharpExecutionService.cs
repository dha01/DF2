using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.CodeExecution.Repository;

namespace Core.Model.CodeExecution.Service.Execution
{
	/// <summary>
	/// Исполняющий сервис функций C#.
	/// </summary>
	public class CSharpExecutionService : IExecutionService
	{
		private IFunctionRepository _functionRepository;

		public CSharpExecutionService(IFunctionRepository function_repository)
		{
			_functionRepository = function_repository;
		}

		public virtual void Execute(Function function, IEnumerable<DataCell> input_data, DataCell output, CommandContext command_context = null)
		{
			var func = _functionRepository.Get<CSharpFunction>(new List<FunctionHeader>(){ (FunctionHeader)function.Header } ).FirstOrDefault();

			if (func == null)
			{
				throw new System.NotImplementedException("Нужно попытаться получить функцию.");
			}

			var full_name = ((CSharpFunctionHeader)function.Header).Namespace;
		//	var types = func.Assembly.GetTypes();
			var type = func.Assembly.GetType($"{full_name}.{func.ClassName}");
			var method = type.GetMethod(func.FuncName);

			output.Data = method.Invoke(null, input_data.Select(x => x.Data).ToArray());
			output.HasValue = true;
		}

	}
}


using System;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeCompiler.Code
{
	public static class BasicFunctions
	{
		/// <summary>
		/// Возвращает модель исполнения базовой функции.
		/// </summary>
		/// <param name="basic_function"></param>
		/// <returns></returns>
		public static BasicFunctionModel GetModel(this BasicFunction basic_function)
		{
			var name = ((BasicFunctionHeader) basic_function.Header).Name;

			if (BasicFunctionModel.AllMethods.ContainsKey(name))
			{
				return BasicFunctionModel.AllMethods[name];
			}

			throw new Exception($"Отсутствует реализация метода с названием '{name}'");
		}
	}
}

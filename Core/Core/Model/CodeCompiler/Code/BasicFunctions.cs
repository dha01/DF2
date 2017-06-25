using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.NetworkLogic;

namespace Core.Model.CodeCompiler.Code
{
	public static class BasicFunctions
	{
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

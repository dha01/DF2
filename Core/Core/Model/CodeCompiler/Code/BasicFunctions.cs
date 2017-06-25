using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.NetworkLogic;

namespace Core.Model.CodeCompiler.Code
{
	public static class BasicFunctions
	{
		public static BasicFunction Sum = new BasicFunction()
		{
			Id = 1,
			Header = new BasicFunctionHeader()
			{
				Name = "+",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "+" },
				Id = 1
			}
		};

		public static BasicFunction Sub = new BasicFunction()
		{
			Id = 2,
			Header = new BasicFunctionHeader()
			{
				Name = "-",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "-" },
				Id = 2
			}
		};

		public static BasicFunction Mul = new BasicFunction()
		{
			Id = 3,
			Header = new BasicFunctionHeader()
			{
				Name = "*",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "*" },
				Id = 3
			}
		};

		public static BasicFunction Div = new BasicFunction()
		{
			Id = 4,
			Header = new BasicFunctionHeader()
			{
				Name = "/",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "/" },
				Id = 4
			}
		};

		public static BasicFunction Equal = new BasicFunction()
		{
			Id = 5,
			Header = new BasicFunctionHeader()
			{
				Name = "==",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "==" },
				Id = 5
			}
		};

		public static BasicFunction NotEqual = new BasicFunction()
		{
			Id = 6,
			Header = new BasicFunctionHeader()
			{
				Name = "!=",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "!==" },
				Id = 6
			}
		};

		public static BasicFunction Not = new BasicFunction()
		{
			Id = 7,
			Header = new BasicFunctionHeader()
			{
				Name = "!",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "!" },
				Id = 7
			}
		};

		public static BasicFunction And = new BasicFunction()
		{
			Id = 8,
			Header = new BasicFunctionHeader()
			{
				Name = "&",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "&" },
				Id = 8
			}
		};

		public static BasicFunction Set = new BasicFunction()
		{
			Id = 9,
			Header = new BasicFunctionHeader()
			{
				Name = "=",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "=" },
				Id = 9
			}
		};

		public static BasicFunction Any = new BasicFunction()
		{
			Id = 10,
			Header = new BasicFunctionHeader()
			{
				Name = "Any",
				Owners = new List<Owner>(),
				CallStack = new List<string>() { "BasicFunctions", "Any" },
				Id = 10
			},
			Condition = InputParamCondition.Any
		};
	}
}

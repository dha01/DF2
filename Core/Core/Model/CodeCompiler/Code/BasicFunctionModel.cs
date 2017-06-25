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
	public class BasicFunctionModel
	{
		#region Fields

		public Func<DataCell[], object> Invoke { get; set; }

		public BasicFunction BasicFunction { get; set; }

		#endregion

		#region Methods / Static

		public static Dictionary<string, BasicFunctionModel> AllMethods = GetAllMethods();

		public static BasicFunctionModel Sum = new BasicFunctionModel
		{
			BasicFunction = GetBase("Sum", 1),
			Invoke = obj => (int)obj[0].Data + (int)obj[1].Data
		};

		public static BasicFunctionModel Sub = new BasicFunctionModel
		{
			BasicFunction = GetBase("Sub", 2),
			Invoke = obj => (int)obj[0].Data - (int)obj[1].Data
		};

		public static BasicFunctionModel Mul = new BasicFunctionModel
		{
			BasicFunction = GetBase("Mul", 3),
			Invoke = obj => (int)obj[0].Data * (int)obj[1].Data
		};

		public static BasicFunctionModel Div = new BasicFunctionModel
		{
			BasicFunction = GetBase("Div", 4),
			Invoke = obj => (int)obj[0].Data / (int)obj[1].Data
		};

		public static BasicFunctionModel Equal = new BasicFunctionModel
		{
			BasicFunction = GetBase("Equal", 5),
			Invoke = obj => obj[0].Equals(obj[1].Data)
		};

		public static BasicFunctionModel NotEqual = new BasicFunctionModel
		{
			BasicFunction = GetBase("NotEqual", 6),
			Invoke = obj => !obj[0].Equals(obj[1].Data)
		};

		public static BasicFunctionModel Any = new BasicFunctionModel
		{
			BasicFunction = GetBase("Any", 7, InputParamCondition.Any),
			Invoke = obj => obj.First(x => x.HasValue).Data
		};

		public static BasicFunctionModel Iif = new BasicFunctionModel
		{
			BasicFunction = GetBase("Iif", 8, InputParamCondition.Iif),
			Invoke = obj => (bool)obj[0].Data ? obj[1].Data : obj[2].Data
		};

		public static BasicFunctionModel Set = new BasicFunctionModel
		{
			BasicFunction = GetBase("Set", 9),
			Invoke = obj => obj[0].Data
		};

		public static BasicFunctionModel Not = new BasicFunctionModel
		{
			BasicFunction = GetBase("Not", 10),
			Invoke = obj => !(bool)obj[0].Data
		};

		#endregion

		#region Methods / Private

		private static BasicFunction GetBase(string name, int id, InputParamCondition condition = InputParamCondition.All)
		{
			return new BasicFunction()
			{
				Id = id,
				Header = new BasicFunctionHeader()
				{
					Name = name,
					Owners = new List<Owner>(),
					CallStack = new List<string>() { "BasicFunctions", name },
					Id = id,
					Condition = condition
				}
			};
		}

		private static Dictionary<string, BasicFunctionModel> GetAllMethods()
		{
			var result = new Dictionary<string, BasicFunctionModel>();

			foreach (var field in typeof(BasicFunctionModel).GetFields())
			{
				if (field.FieldType == typeof(BasicFunctionModel))
				{
					result.Add(field.Name, (BasicFunctionModel)field.GetValue(null));
				}
			}
			return result;
		}

		#endregion
	}
}

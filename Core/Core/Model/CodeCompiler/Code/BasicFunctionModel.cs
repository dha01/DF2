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
	/// <summary>
	/// Модель исполнения базовых методов.
	/// </summary>
	public class BasicFunctionModel
	{
		#region Fields

		public Func<DataCell[], DataCell> Invoke { get; set; }

		public BasicFunction BasicFunction { get; set; }

		#endregion

		#region Methods / Static

		private static Dictionary<string, BasicFunctionModel> _allMethods;

		/// <summary>
		/// Все базовые методы.
		/// </summary>
		public static Dictionary<string, BasicFunctionModel> AllMethods
		{
			get
			{
				if (_allMethods == null)
				{
					_allMethods = GetAllMethods();
				}
				return _allMethods;
			}
		}

		public static BasicFunctionModel Sum = new BasicFunctionModel
		{
			BasicFunction = GetBase("Sum", 1),
			Invoke = obj => new DataCell
			{
				Data = (int)obj[0].Data + (int)obj[1].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel Sub = new BasicFunctionModel
		{
			BasicFunction = GetBase("Sub", 2),
			Invoke = obj => new DataCell
			{
				Data = (int)obj[0].Data - (int)obj[1].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel Mul = new BasicFunctionModel
		{
			BasicFunction = GetBase("Mul", 3),
			Invoke = obj =>new DataCell
			{
				Data = (int) obj[0].Data * (int) obj[1].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel Div = new BasicFunctionModel
		{
			BasicFunction = GetBase("Div", 4),
			Invoke = obj => new DataCell
			{
				Data = (int)obj[0].Data / (int)obj[1].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel Equal = new BasicFunctionModel
		{
			BasicFunction = GetBase("Equal", 5),
			Invoke = obj => new DataCell
			{
				Data = obj[0].Data.Equals(obj[1].Data),
				HasValue = true
			}
		};

		public static BasicFunctionModel NotEqual = new BasicFunctionModel
		{
			BasicFunction = GetBase("NotEqual", 6),
			Invoke = obj => new DataCell
			{
				Data = !obj[0].Equals(obj[1].Data),
				HasValue = true
			}
		};

		public static BasicFunctionModel Any = new BasicFunctionModel
		{
			BasicFunction = GetBase("Any", 7, InputParamCondition.Any),
			Invoke = obj => new DataCell
			{
				Data = obj.First(x => x != null && x.HasValue.HasValue && x.HasValue.Value).Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel Iif = new BasicFunctionModel
		{
			BasicFunction = GetBase("Iif", 8, InputParamCondition.Iif),
			Invoke = obj => {
				if (obj[0].HasValue == false)
				{
					return new DataCell
					{
						Data = null,
						HasValue = false
					};
				}

				return (bool) obj[0].Data ? obj[1] : obj[2];
			}
		};

		public static BasicFunctionModel Set = new BasicFunctionModel
		{
			BasicFunction = GetBase("Set", 9),
			Invoke = obj => new DataCell
			{
				Data = obj[0].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel Not = new BasicFunctionModel
		{
			BasicFunction = GetBase("Not", 10),
			Invoke = obj => new DataCell
			{
				Data = !(bool)obj[0].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel Or = new BasicFunctionModel
		{
			BasicFunction = GetBase("Or", 11),
			Invoke = obj => new DataCell
			{
				Data = (bool)obj[0].Data || (bool)obj[1].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel And = new BasicFunctionModel
		{
			BasicFunction = GetBase("And", 12),
			Invoke = obj => new DataCell
			{
				Data = (bool)obj[0].Data && (bool)obj[1].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel Wait = new BasicFunctionModel
		{
			BasicFunction = GetBase("Wait", 13),
			Invoke = obj => new DataCell
			{
				Data = obj[0].Data,
				HasValue = true
			}
		};

		public static BasicFunctionModel IsTrue = new BasicFunctionModel
		{
			BasicFunction = GetBase("IsTrue", 14),
			Invoke = obj => new DataCell
			{
				Data = (bool)obj[0].Data == true,
				HasValue = true
			}
		};

		public static BasicFunctionModel IsFalse = new BasicFunctionModel
		{
			BasicFunction = GetBase("IsFalse", 15),
			Invoke = obj => new DataCell
			{
				Data = (bool)obj[0].Data == false,
				HasValue = true
			}
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
					Token = $"BasicFunctions.{name}",
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

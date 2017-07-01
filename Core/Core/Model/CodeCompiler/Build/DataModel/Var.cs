using System.Collections.Generic;
using System.Linq;
using Core.Model.CodeCompiler.Code;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeCompiler.Build.DataModel
{
	/// <summary>
	/// Переменная.
	/// </summary>
	/// <typeparam name="T">Тип.</typeparam>
	public class Var<T> : IVarInterface
	{
		private CommandBuilder _commandBuilder;

		/// <summary>
		/// Строка шаблона функции.
		/// </summary>
		public TemplateFunctionRow Id { get; set; }

		public Var<T> Constant(T value)
		{
			Id = _commandBuilder.Constant(value);
			return this;
		}

		public Var<T> NewCommand(FunctionHeader fucntion_header, params IVarInterface[] input_data)
		{
			Id = _commandBuilder.NewCommand(fucntion_header, input_data.Select(x=>x.Id));
			return this;
		}

		public Var<T> NewCommand(BasicFunctionModel fucntion, params IVarInterface[] input_data)
		{
			Id = _commandBuilder.NewCommand((FunctionHeader)fucntion.BasicFunction.Header, input_data.Select(x => x.Id));
			return this;
		}

		public Var(CommandBuilder command_builder)
		{
			_commandBuilder = command_builder;
		}

		public void Set(Var<T> var)
		{
			
		}

		public void Set(T var)
		{

		}

		public static implicit operator T(Var<T> var)
		{
			return default(T);
		}

		public static implicit operator TemplateFunctionRow(Var<T> var)
		{
			return var.Id;
		}

		#region Sum

		public static Var<T> operator +(Var<T> a, Var<T> b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Sum, new List<TemplateFunctionRow> {a , b})
			};
		}

		public static Var<T> operator +(Var<T> a, T b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Sum, new List<TemplateFunctionRow> { a, a._commandBuilder.Constant(b) })
			};
		}

		public static Var<T> operator +(T a, Var<T> b)
		{
			return new Var<T>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.Sum, new List<TemplateFunctionRow> { b._commandBuilder.Constant(a), b })
			};
		}

		#endregion

		#region Sub

		public static Var<T> operator -(Var<T> a, Var<T> b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Sub, new List<TemplateFunctionRow> { a, b })
			};
		}

		public static Var<T> operator -(Var<T> a, T b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Sub, new List<TemplateFunctionRow> { a, a._commandBuilder.Constant(b) })
			};
		}

		public static Var<T> operator -(T a, Var<T> b)
		{
			return new Var<T>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.Sub, new List<TemplateFunctionRow> { b._commandBuilder.Constant(a), b })
			};
		}

		#endregion

		public static Var<T> operator -(Var<T> a)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Sub, new List<TemplateFunctionRow> { a._commandBuilder.Constant(0), a })
			};
		}

		#region Mul

		public static Var<T> operator *(Var<T> a, Var<T> b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Mul, new List<TemplateFunctionRow> { a, b })
			};
		}

		public static Var<T> operator *(Var<T> a, T b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Mul, new List<TemplateFunctionRow> { a, a._commandBuilder.Constant(b) })
			};
		}

		public static Var<T> operator *(T a, Var<T> b)
		{
			return new Var<T>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.Mul, new List<TemplateFunctionRow> { b._commandBuilder.Constant(a), b })
			};
		}

		#endregion

		#region Div

		public static Var<T> operator /(Var<T> a, Var<T> b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Div, new List<TemplateFunctionRow> { a, b })
			};
		}

		public static Var<T> operator /(Var<T> a, T b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Div, new List<TemplateFunctionRow> { a, a._commandBuilder.Constant(b) })
			};
		}

		public static Var<T> operator /(T a, Var<T> b)
		{
			return new Var<T>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.Div, new List<TemplateFunctionRow> { b._commandBuilder.Constant(a), b })
			};
		}

		#endregion

		#region Equal

		public static Var<bool> operator ==(Var<T> a, Var<T> b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Equal, new List<TemplateFunctionRow> { a, b })
			};
		}

		public static Var<bool> operator ==(Var<T> a, T b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Equal, new List<TemplateFunctionRow> { a, a._commandBuilder.Constant(b) })
			};
		}

		public static Var<bool> operator ==(T a, Var<T> b)
		{
			return new Var<bool>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.Equal, new List<TemplateFunctionRow> { b._commandBuilder.Constant(a), b })
			};
		}

		#endregion

		#region NotEqual

		public static Var<bool> operator !=(Var<T> a, Var<T> b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.NotEqual, new List<TemplateFunctionRow> { a, b })
			};
		}


		public static Var<bool> operator !=(Var<T> a, T b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.NotEqual, new List<TemplateFunctionRow> { a, a._commandBuilder.Constant(b) })
			};
		}

		public static Var<bool> operator !=(T a, Var<T> b)
		{
			return new Var<bool>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.NotEqual, new List<TemplateFunctionRow> { b._commandBuilder.Constant(a), b })
			};
		}

		#endregion

		#region Or

		public static Var<bool> operator |(Var<T> a, Var<T> b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Or, new List<TemplateFunctionRow> { a, b })
			};
		}


		public static Var<bool> operator |(Var<T> a, T b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Or, new List<TemplateFunctionRow> { a, a._commandBuilder.Constant(b) })
			};
		}

		public static Var<bool> operator |(T a, Var<T> b)
		{
			return new Var<bool>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.Or, new List<TemplateFunctionRow> { b._commandBuilder.Constant(a), b })
			};
		}

		#endregion

		#region And

		public static Var<bool> operator &(Var<T> a, Var<T> b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.And, new List<TemplateFunctionRow> { a, b })
			};
		}


		public static Var<bool> operator &(Var<T> a, T b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.And, new List<TemplateFunctionRow> { a, a._commandBuilder.Constant(b) })
			};
		}

		public static Var<bool> operator &(T a, Var<T> b)
		{
			return new Var<bool>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.And, new List<TemplateFunctionRow> { b._commandBuilder.Constant(a), b })
			};
		}

		#endregion
		/*
		public static Var<bool> operator !(Var<T> b)
		{
			return new Var<bool>(b._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.Not, new List<TemplateFunctionRow> { b })
			};
		}

		public static Var<bool> operator &(Var<T> a, Var<T> b)
		{
			return new Var<bool>(a._commandBuilder)
			{
				Id = b._commandBuilder.NewCommand(BasicFunctionModel.And, new List<TemplateFunctionRow> { a._commandBuilder.Constant(a), a._commandBuilder.Constant(b) })
			};
		}*/
		/*
		public static Var<T> operator +(Var<T> x, Var<T> y)
		{
			return var.Id;
		}*/
	}
}

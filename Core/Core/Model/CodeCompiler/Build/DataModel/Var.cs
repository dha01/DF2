using System.Collections.Generic;
using Core.Model.CodeCompiler.Code;

namespace Core.Model.CodeCompiler.Build.DataModel
{
	public interface VarInterface
	{
		TemplateFunctionRow Id { get; set; }
	}

	public class Var<T> : VarInterface
	{
		private CommandBuilder _commandBuilder;

		public TemplateFunctionRow Id { get; set; }

		/*public Var()
		{
			//_commandBuilder = command_builder;
		}*/

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
		/*
		public static explicit operator Var<T>(int var)
		{
			return new Var<T>() { Id = var };
		}*/

		public static implicit operator TemplateFunctionRow(Var<T> var)
		{
			return var.Id;
		}

		#region

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

		public static Var<T> operator *(Var<T> a, Var<T> b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Mul, new List<TemplateFunctionRow> { a, b })
			};
		}

		public static Var<T> operator /(Var<T> a, Var<T> b)
		{
			return new Var<T>(a._commandBuilder)
			{
				Id = a._commandBuilder.NewCommand(BasicFunctionModel.Div, new List<TemplateFunctionRow> { a, b })
			};
		}

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

		#region Or

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

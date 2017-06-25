using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeCompiler.Build.DataModel;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeCompiler.Code
{
	public class ControlFunctionBase
	{
		protected CommandBuilder cmd = new CommandBuilder();

		protected Var<T> Null<T>()
		{
			return new Var<T>(cmd);
		}

		protected void If(Var<bool> a)
		{
		}

		protected Var<T> Iif<T>(Var<bool> condition, Var<T> val_falce, Var<T> val_true)
		{
			return new Var<T>(cmd)
			{
				Id = cmd.NewCommand(BasicFunctionModel.Iif, new TemplateFunctionRow[] { condition, val_falce, val_true })
			};
		}

		protected Var<T> Iif<T>(Var<bool> a, T val)
		{
			return Iif(a, new Var<T>(cmd) { Id = cmd.Constant(val) });
		}

		protected void Else()
		{
		}

		protected void End()
		{
		}

		protected void Return<T>(Var<T> id)
		{
			cmd.Return(id);
		}

		protected Var<T> Any<T>(params Var<T>[] ids)
		{
			return new Var<T>(cmd)
			{
				Id = cmd.NewCommand(BasicFunctionModel.Any, ids.Select(id => (TemplateFunctionRow)id))
			};
		}

		/*
		protected InvokedControlFunction<T, T_2, TResult> Get<T, T_2, TResult>(Func<T, T_2, TResult> func)
		{
			var method_info = SymbolExtensions.GetMethodInfo((T a, T_2 b) => func(a, b));

			return new InvokedControlFunction<T, T_2, TResult>(cmd, method_info);
		}*/

		protected Var<TResult> Exec<T, T_2, TResult>(Func<T, T_2, TResult> func, Var<T> input1, Var<T_2> input2)
		{
			var method_info = func.GetMethodInfo();// SymbolExtensions.GetMethodInfo((T a, T_2 b) => func(a, b));
		//	var method_info = SymbolExtensions.GetMethodInfo(func);
			//return new InvokedControlFunction<T, T_2, TResult>(cmd, method_info).Invoke(input1, input2);

			var header = CommandBuilder.BuildHeader(method_info.Name, $"{method_info.DeclaringType.Namespace}.{method_info.DeclaringType.Name}".Split('.'));
			return new Var<TResult>(cmd)
			{
				Id = cmd.NewCommand(header, new TemplateFunctionRow[] { input1, input2 })
			};
		}

		protected Var<TResult> Exec<TResult>(string func, params object[] param)
		{
			var type = GetType();
			//var method_info = func.GetMethodInfo();// SymbolExtensions.GetMethodInfo((T a, T_2 b) => func(a, b));
			//	var method_info = SymbolExtensions.GetMethodInfo(func);
			//return new InvokedControlFunction<T, T_2, TResult>(cmd, method_info).Invoke(input1, input2);

			var header = CommandBuilder.BuildHeader(func, $"{type.Namespace}.{type.Name}".Split('.'));
			var par = param.Select(x => ((VarInterface) x).Id).ToList();
			return new Var<TResult>(cmd)
			{
				Id = cmd.NewCommand(header, par)
			};
		}

		protected Var<TResult> Exec<T, T_2, TResult>(FunctionHeader func, Var<T> input1, Var<T_2> input2)
		{
			return new Var<TResult>(cmd)
			{
				Id = cmd.NewCommand(func, new TemplateFunctionRow[] { input1, input2 })
			};
		}

		protected Var<T> Const<T>(T value)
		{
			return new Var<T>(cmd)
			{
				Id = cmd.Constant(value)
			}; 
		}

		public CommandBuilder GetFunc()
		{
			return cmd;
		}
	}
}

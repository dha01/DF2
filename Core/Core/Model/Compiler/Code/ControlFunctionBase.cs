using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Core.Model.Commands.Build;
using Core.Model.Compiler.Build.DataModel;
using Core.Model.Headers.Functions;

namespace Core.Model.Compiler.Code
{
	public class ControlFunctionBase
	{
		protected CommandBuilder cmd = new CommandBuilder();

		protected void Return<T>(Var<T> id)
		{
			cmd.Return(id);
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

			var header = CommandBuilder.BuildHeader(method_info.Name, $"{method_info.DeclaringType.Namespace}.{method_info.DeclaringType.Name}".Split('.').ToList());
			return new Var<TResult>(cmd)
			{
				Id = cmd.NewCommand(header, new int[] { input1, input2 })
			};
		}

		protected Var<TResult> Exec<T, T_2, TResult>(FunctionHeader func, Var<T> input1, Var<T_2> input2)
		{
			return new Var<TResult>(cmd)
			{
				Id = cmd.NewCommand(func, new int[] { input1, input2 })
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

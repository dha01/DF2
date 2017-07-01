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

		private void AddCondition(TemplateFunctionRow condition, IEnumerable<TemplateFunctionRow> vals)
		{
			foreach (var inp in vals)
			{
				if (!inp.Conditions.Contains(condition))
				{
					inp.Conditions.Add(condition);
				}
				if (inp.Type == TemplateFunctionRowType.Func)
				{
					AddCondition(condition, inp.Input);
				}
			}
		}

		protected Var<T> Iif<T>(Var<bool> condition, Var<T> val_true, Var<T> val_false)
		{
			/*var result = new Var<T>(cmd)
			{
				Id = cmd.NewCommand(BasicFunctionModel.Iif, new TemplateFunctionRow[] {condition, val_true, val_false})
			};*/
			var true_func = new Var<T>(cmd)
			{
				Id = cmd.NewCommand(BasicFunctionModel.IsTrue, new TemplateFunctionRow[] { condition })
			};

			var false_func = new Var<T>(cmd)
			{
				Id = cmd.NewCommand(BasicFunctionModel.IsFalse, new TemplateFunctionRow[] { condition })
			};

			var resolved_val_true = new Var<T>(cmd)
			{
				Id = cmd.NewCommand(BasicFunctionModel.Set, new TemplateFunctionRow[] { val_true })
			};
			//resolved_val_true.Id.Conditions.Add(true_func);

			var resolved_val_false = new Var<T>(cmd)
			{
				Id = cmd.NewCommand(BasicFunctionModel.Set, new TemplateFunctionRow[] { val_false })
			};
			//resolved_val_false.Id.Conditions.Add(false_func);

			AddCondition(true_func, new List<TemplateFunctionRow>{ resolved_val_true });
			AddCondition(false_func, new List<TemplateFunctionRow> { resolved_val_false });
			//val_false.Id.Conditions.Add(false_func);

			true_func.Id.Triggered.Add(resolved_val_true);
			false_func.Id.Triggered.Add(resolved_val_false);

			/*switch (val_false.Id.Type)
			{
				case TemplateFunctionRowType.Const:
				case TemplateFunctionRowType.Input:
					false_func.Id.Triggered.Add(new Var<T>(cmd)
					{
						Id = cmd.NewCommand(BasicFunctionModel.Set, new TemplateFunctionRow[] { condition })
					});
					break;
				case TemplateFunctionRowType.Func:
					false_func.Id.Triggered.Add(val_false);
					break;
				default:
					throw new NotImplementedException("Не предусмотрено.");
			}

			switch (val_true.Id.Type)
			{
				case TemplateFunctionRowType.Const:
				case TemplateFunctionRowType.Input:
					true_func.Id.Triggered.Add(new Var<T>(cmd)
					{
						Id = cmd.NewCommand(BasicFunctionModel.Set, new TemplateFunctionRow[] { condition })
					});
					break;
				case TemplateFunctionRowType.Func:
					true_func.Id.Triggered.Add(val_true);
					break;
				default:
					throw new NotImplementedException("Не предусмотрено.");
			}*/

			return new Var<T>(cmd)
			{
				Id = cmd.NewCommand(BasicFunctionModel.Any, new TemplateFunctionRow[] { resolved_val_true, resolved_val_false })
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

		protected Var<TResult> Exec<TResult>(string func, params VarInterface[] param)
		{
			var type = GetType();
			//var method_info = func.GetMethodInfo();// SymbolExtensions.GetMethodInfo((T a, T_2 b) => func(a, b));
			//	var method_info = SymbolExtensions.GetMethodInfo(func);
			//return new InvokedControlFunction<T, T_2, TResult>(cmd, method_info).Invoke(input1, input2);

			var header = CommandBuilder.BuildHeader(func, $"{type.Namespace}.{type.Name}".Split('.'));
			var par = param.Select(x => x.Id).ToList();
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

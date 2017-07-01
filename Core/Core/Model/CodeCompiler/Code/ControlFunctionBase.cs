using System;
using System.Reflection;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeCompiler.Build.DataModel;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeCompiler.Code
{
	/// <summary>
	/// Класс для возможности создания управляющих функций в коде C#.
	/// </summary>
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

		private void AddCondition(TemplateFunctionRow condition, params TemplateFunctionRow[] vals)
		{
			foreach (var inp in vals)
			{
				if (!inp.Conditions.Contains(condition))
				{
					inp.Conditions.Add(condition);
				}
				if (inp.Type == TemplateFunctionRowType.Func)
				{
					AddCondition(condition, inp.Input.ToArray());
				}
			}
		}

		/// <summary>
		/// Условный оператор.
		/// </summary>
		/// <typeparam name="T">Возвращаемый тип.</typeparam>
		/// <param name="condition">Условие.</param>
		/// <param name="val_true">Возвращаемой значение, если условие истинно.</param>
		/// <param name="val_false">Вовзращаемое значение, если условие ложно.</param>
		/// <returns>Результат.</returns>
		protected Var<T> Iif<T>(Var<bool> condition, Var<T> val_true, Var<T> val_false)
		{
			var true_func = new Var<T>(cmd).NewCommand(BasicFunctionModel.IsTrue, condition);
			var false_func = new Var<T>(cmd).NewCommand(BasicFunctionModel.IsFalse, condition);
			var resolved_val_true = new Var<T>(cmd).NewCommand(BasicFunctionModel.Set, val_true);
			var resolved_val_false = new Var<T>(cmd).NewCommand(BasicFunctionModel.Set, val_false);

			AddCondition(true_func, resolved_val_true);
			AddCondition(false_func, resolved_val_false);

			true_func.Id.Triggered.Add(resolved_val_true);
			false_func.Id.Triggered.Add(resolved_val_false);

			return new Var<T>(cmd).NewCommand(BasicFunctionModel.Any, resolved_val_true, resolved_val_false);
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

		/// <summary>
		/// Возращает любое значение из списка. Все остальные значения могут быть без результатаэ
		/// </summary>
		/// <typeparam name="T">Возвращаемый тип.</typeparam>
		/// <param name="vals">Значения.</param>
		/// <returns>Возвращенное значение.</returns>
		protected Var<T> Any<T>(params Var<T>[] vals)
		{
			return new Var<T>(cmd).NewCommand(BasicFunctionModel.Any, vals);// {Id = cmd.NewCommand(BasicFunctionModel.Any, vals.Select(id => (TemplateFunctionRow)id))};
		}

		/// <summary>
		/// Исполняет функцию от двух входных значений.
		/// </summary>
		/// <typeparam name="T">Тип первого входного значения.</typeparam>
		/// <typeparam name="T_2">Тип второго входного значения.</typeparam>
		/// <typeparam name="TResult">Тип возвращщаемого значения.</typeparam>
		/// <param name="func">Функция.</param>
		/// <param name="input1">Первое входное значение.</param>
		/// <param name="input2">Второе входное значение.</param>
		/// <returns>Возвращенное значение.</returns>
		protected Var<TResult> Exec<T, T_2, TResult>(Func<T, T_2, TResult> func, Var<T> input1, Var<T_2> input2)
		{
			var method_info = func.GetMethodInfo();
			var header = CommandBuilder.BuildHeader(method_info.Name, $"{method_info.DeclaringType.Namespace}.{method_info.DeclaringType.Name}".Split('.'));
			return new Var<TResult>(cmd).NewCommand(header, input1, input2);
		}

		/// <summary>
		/// Исполняет функцию от произвольного числа входных значений.
		/// </summary>
		/// <typeparam name="TResult">Тип возвращщаемого значения.</typeparam>
		/// <param name="func">Функция.</param>
		/// <param name="param">Входные значения.</param>
		/// <returns>Возвращаемое значение.</returns>
		protected Var<TResult> Exec<TResult>(string func, params IVarInterface[] param)
		{
			var type = GetType();
			var header = CommandBuilder.BuildHeader(func, $"{type.Namespace}.{type.Name}".Split('.'));
			return new Var<TResult>(cmd).NewCommand(header, param);
		}

		protected Var<TResult> Exec<T, T_2, TResult>(FunctionHeader func, Var<T> input1, Var<T_2> input2)
		{
			return new Var<TResult>(cmd).NewCommand(func, input1, input2);
		}

		protected Var<T> Const<T>(T value)
		{
			return new Var<T>(cmd).Constant(value);
		}

		public CommandBuilder GetFunc()
		{
			return cmd;
		}
	}
}

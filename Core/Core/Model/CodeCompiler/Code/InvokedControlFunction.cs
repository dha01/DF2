﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeCompiler.Build.DataModel;

namespace Core.Model.CodeCompiler.Code
{
	public class InvokedControlFunction<T, T_2, TResult>
	{
		private CommandBuilder cmd;
		private MethodInfo _methodInfo;

		public InvokedControlFunction(CommandBuilder command_builder, MethodInfo method_info)
		{
			cmd = command_builder;
			_methodInfo = method_info;
		}

		public Var<TResult> Invoke(Var<T> item1, Var<T_2> item2)
		{
			var header = CommandBuilder.BuildHeader(_methodInfo.Name, $"{_methodInfo.DeclaringType.Namespace}.{_methodInfo.DeclaringType.Name}".Split('.').ToList());
			return new Var<TResult>(cmd)
			{
				Id = cmd.NewCommand(header, new TemplateFunctionRow[] { item1, item2 })
			};
		}
	}
}

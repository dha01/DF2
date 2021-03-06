﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Core.Model.CodeCompiler.Build;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.DataFlowLogics.InstructionExecutionConveyor.Extractors.DataModel;

namespace Core.Model.DataFlowLogics.InstructionExecutionConveyor.Extractors
{
	public class CSharpFunctionExtractor
	{
		public static CSharpAssembly ExtractAssembly(Assembly assembly)
		{
			var cs_assembly = new CSharpAssembly
			{
				CSharpClass = new List<CSharpClass>(),
				ControlFunctions = new List<ControlFunction>()
			};

			foreach (var type in assembly.GetTypes())
			{
				var methods = type.GetMethods();

				// TODO: нужно сделать через атрибут
			//	if (type.Name.Equals("Simple"))
			//	{
					var get_func_method = type.GetMethod("GetFunc");
					foreach (var method in methods)
					{
						// TODO: нужно сделать через атрибут
						if (method.Name.Equals("Main") || method.GetParameters().Any(x => x.ParameterType.Name.StartsWith("Var")))
						{
							cs_assembly.ControlFunctions.Add(CommandBuilder.CompileMethodFromAssembly(assembly, type, get_func_method, method));
							/*var s = Activator.CreateInstance(type);
							var input = new List<object>();

							var builder = (CommandBuilder)get_func_method.Invoke(s, null);
							foreach (var param in method.GetParameters())
							{
								var in_var = Activator.CreateInstance(param.ParameterType, builder);
								param.ParameterType.GetProperty("Id").SetValue(in_var, builder.InputData());
								input.Add(in_var);
							}

							method.Invoke(s, input.ToArray());
							//s._Main((Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData(), (Var<int>)builder.InputData());
							var control_func = CommandBuilder.Build(method.Name, $"{type.Namespace}.{type.Name}".Split('.').ToList(), () => builder);
							cs_assembly.ControlFunctions.Add(control_func);*/
						}
					}
				/*}
				else
				{
					var cs_class = new CSharpClass()
					{
						CSharpFunction = new List<CSharpFunction>()
					};
					foreach (var method in methods)
					{
						//if(method)

						var cs_function = new CSharpFunction()
						{
							Header = new CSharpFunctionHeader()
							{
								CallStack = $"{type.Namespace}.{type.Name}.{method.Name}".Split('.'),
								Name = method.Name,
								//FileName = path,
								FunctionName = method.Name,
								Namespace = type.Namespace,
								Version = assembly.ImageRuntimeVersion,
								Owners = new List<Owner>()
							},
							Namespace = type.Namespace,
							FuncName = method.Name,
							Assembly = assembly,
							ClassName = type.Name
						};
						cs_class.CSharpFunction.Add(cs_function);
					}

					cs_assembly.CSharpClass.Add(cs_class);
				}
				
				*/
			}

			return cs_assembly;
			/*var myInstance = Activator.CreateInstance(myType);

			Assembly assembly = Assembly. LoadFrom(path);

			Type type = assembly.GetType("MyType");

			object instanceOfMyType = Activator.CreateInstance(type);*/
		}

		public static CSharpAssembly ExtractAssembly(string path)
		{
			var fs = new FileStream(path, FileMode.Open);
			var myAssembly = AssemblyLoadContext.Default.LoadFromStream(fs); 
			return ExtractAssembly(myAssembly);
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Core.Model.CodeCompiler.Build.Attributes;
using Core.Model.CodeCompiler.Build.DataModel;
using Core.Model.CodeCompiler.Code;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;
using Core.Model.NetworkLogic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Core.Model.CodeCompiler.Build
{
	/// <summary>
	/// Класс для подготовки и сборки управляющих команд.
	/// </summary>
	public class CommandBuilder
	{
		#region Fields

		private List<int> _dataFromCommandIds = new List<int>() { -2 };

		private List<CommandTemplate> _commandTemplates = new List<CommandTemplate>();

		private List<object> _constants = new List<object>();

		//private int _tmpDataCount = 0;

		private int _inputDataCount;


		private List<TemplateFunctionRow> _rows = new List<TemplateFunctionRow>();

		#endregion

		#region Methods / Static

		public static Assembly CreateFunctionFromSourceCode(string code)
		{
			var dd = typeof(Enumerable).GetTypeInfo().Assembly.Location;
			var coreDir = Directory.GetParent(dd);

			var fileName = Guid.NewGuid().ToString() + ".dll";
			var compilation = CSharpCompilation.Create(fileName)
				.WithOptions(new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary))
				.AddReferences(
					MetadataReference.CreateFromFile(typeof(Object).GetTypeInfo().Assembly.Location),
					MetadataReference.CreateFromFile(typeof(Uri).GetTypeInfo().Assembly.Location),
					MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "mscorlib.dll"),
					MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "System.Runtime.dll"),
					MetadataReference.CreateFromFile(typeof(Var<int>).GetTypeInfo().Assembly.Location)
				)
				.AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));

			var eResult = compilation.Emit(fileName);

			return AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(fileName));
		}

		public static ControlFunctionHeader BuildHeader(string name, IEnumerable<string> name_space)
		{
			var list = new List<string>(name_space);
			list.Add(name);
			return new ControlFunctionHeader()
			{
				FunctionName = name,
				Name = name,
				Owners = new List<Owner>(),
				CallStack = list
			};
		}

		public static ControlFunction Build(string name, List<string> name_space, Func<CommandBuilder> build_func)
		{
			var cmd = build_func.Invoke();
			return cmd.BuildFunction(name, name_space);
		}

		public static ControlFunction CompileMethodFromAssembly(Assembly assembly, string full_name)
		{
			var split_full_name = full_name.Split('.');
			var type_name = string.Join(".", split_full_name.Take(split_full_name.Length - 1));
			var type = assembly.GetType(type_name);
			var method = type.GetMethod(split_full_name.Last());

			var atrribute = method.GetCustomAttribute<ControlFunctionAttribute>();
			if (atrribute != null)
			{
				var get_func_method = type.GetMethod("GetFunc");
				return CompileMethodFromAssembly(assembly, type, get_func_method, method);
			}

			throw new Exception($"Не удалось получить метод {full_name}");
		}

		public static ControlFunction CompileMethodFromAssembly(Assembly assembly, Type type, MethodInfo get_func_method, MethodInfo method_info)
		{
			var s = Activator.CreateInstance(type);
			var input = new List<object>();

			var builder = (CommandBuilder)get_func_method.Invoke(s, null);
			foreach (var param in method_info.GetParameters())
			{
				var in_var = Activator.CreateInstance(param.ParameterType, builder);
				param.ParameterType.GetProperty("Id").SetValue(in_var, builder.InputData());
				input.Add(in_var);
			}

			method_info.Invoke(s, input.ToArray());
			return Build(method_info.Name, $"{type.Namespace}.{type.Name}".Split('.').ToList(), () => builder);
		}

		public static List<DataCell> BuildInputData(IEnumerable<object> data, List<string> call_stack)
		{
			var str = string.Join("/", call_stack);
			call_stack.Add("InputData");
			var index = 0;
			return data.Select(x => new DataCell()
			{
				Data = x,
				HasValue = true,
				Header = new DataCellHeader()
				{
					Owners = new List<Owner>(),
					CallStack = string.Format("{0}/InputData{1}", str, index++).Split('/'),
					HasValue = new Dictionary<Owner, bool>()
				}
			}).ToList();
		}

		#endregion

		/// <summary>
		/// Возвращает идентификатор входного параметра.
		/// </summary>
		/// <returns></returns>
		public TemplateFunctionRow InputData()
		{
			var row = new TemplateFunctionRow
			{
				Type = TemplateFunctionRowType.Input,
				Input = null,
				Triggered = new List<TemplateFunctionRow>()
			};
			_rows.Add(row);
			return row;
		}

		/// <summary>
		/// Подгатавливает команду и новую ячейку данных для результата её выполнения и возвращает идентификатор команды.
		/// </summary>
		/// <param name="out_data_id">Идентификатор ячейки данных с результатом выполнения</param>
		/// <returns>Идентификатор команды.</returns>
		private int GetNewTmpFunctionId(out int out_data_id)
		{
			

			var id = _commandTemplates.Count;
			_dataFromCommandIds.Add(id);
			out_data_id = _dataFromCommandIds.Count - 1;
			return id;
		}

		/// <summary>
		/// Подгатавливает команду и возвращает её идентификатор.
		/// </summary>
		/// <param name="fucntion_header">Заголовок функции.</param>
		/// <param name="input_data">Входные данные.</param>
		/// <returns>Идентификатор команды.</returns>
		public TemplateFunctionRow NewCommand(FunctionHeader fucntion_header, IEnumerable<TemplateFunctionRow> input_data)
		{
			var row = new TemplateFunctionRow
			{
				Type = TemplateFunctionRowType.Func,
				Input = input_data.ToList(),
				Triggered = new List<TemplateFunctionRow>(),
				FunctionHeader = fucntion_header
			};

			foreach (var input in input_data)
			{
				var result = _rows.FirstOrDefault(x => x == input);
				if (result == null)
				{
					throw new Exception("NewCommand неверно указан входной параметр.");
				}

				if (!result.Triggered.Contains(row))
				{
					result.Triggered.Add(row);
				}
			}

			_rows.Add(row);

			return row;
		}

		/// <summary>
		/// Подгатавливает команду и возвращает её идентификатор.
		/// </summary>
		/// <param name="fucntion">Функция.</param>
		/// <param name="input_data">Входные данные.</param>
		/// <returns>Идентификатор команды.</returns>
		public TemplateFunctionRow NewCommand(Function fucntion, IEnumerable<TemplateFunctionRow> input_data)
		{
			return NewCommand((FunctionHeader)fucntion.Header, input_data);
		}

		/// <summary>
		/// Подгатавливает команду и возвращает её идентификатор.
		/// </summary>
		/// <param name="fucntion">Функция.</param>
		/// <param name="input_data">Входные данные.</param>
		/// <returns>Идентификатор команды.</returns>
		public TemplateFunctionRow NewCommand(BasicFunctionModel fucntion, IEnumerable<TemplateFunctionRow> input_data)
		{
			return NewCommand((FunctionHeader)fucntion.BasicFunction.Header, input_data);
		}

		public TemplateFunctionRow Constant(object data)
		{
			var row = new TemplateFunctionRow
			{
				Type = TemplateFunctionRowType.Const,
				Input = null,
				Triggered = new List<TemplateFunctionRow>(),
				Value = data
			};
			_rows.Add(row);
			return row;
		}

		/// <summary>
		/// Данные из указанной ячейки становятся возвращаемыми данными.
		/// </summary>
		/// <param name="output_data_id">Идентификатор ячейки данных с выходным параметром.</param>
		public void Return(TemplateFunctionRow output_data_id)
		{
			var result =_rows.FirstOrDefault(x => x == output_data_id);
			if (result == null)
			{
				throw new Exception("public void Return Не удалось получить строку.");
			}

			if (result.Type == TemplateFunctionRowType.Input)
			{
				result = NewCommand(BasicFunctionModel.Set, new List<TemplateFunctionRow> {output_data_id});
			}

			result.Type = TemplateFunctionRowType.Output;
			result.IsOutput = true;

			//_commandTemplates[_dataFromCommandIds[output_data_id]].OutputDataId = 0;
		}

		/// <summary>
		/// Возвращает список команд.
		/// </summary>
		/// <returns>Список команд.</returns>
		public List<CommandTemplate> BuildCommands()
		{
			

			_rows = _rows.OrderBy(x => x.Type).ToList();
			//var const_shift = _commandTemplates.Count + _constants.Count + _inputDataCount + 1;

			List<CommandTemplate> command_templates = new List<CommandTemplate>();

			var outputs = _rows.Where(x => x.IsOutput).ToList();
			if (outputs == null || !outputs.Any())
			{
				throw new Exception("BuildCommands Не найдено возвращаемое значение.");
			}

			var output = outputs.Count > 1 ? NewCommand(BasicFunctionModel.Any, outputs) : outputs.First();
			
			var funcs = _rows.Where(x => x.Type == TemplateFunctionRowType.Func || x.Type == TemplateFunctionRowType.Output).OrderBy(x => x.Type).ToList();
			
			command_templates.Add(new CommandTemplate()
			{
				InputDataIds = output.Input.Select(x => _rows.IndexOf(x)).ToList(),
				TriggeredCommandIds = output.Triggered.Select(x => funcs.IndexOf(x)).ToList(),
				OutputDataId = _rows.IndexOf(output),
				FunctionHeader = output.FunctionHeader
			});

			foreach (var func in funcs)
			{
				if (func.IsOutput)
				{
					continue;
				}
				var command = new CommandTemplate()
				{
					InputDataIds = func.Input.Select(x => _rows.IndexOf(x)).ToList(),
					TriggeredCommandIds = func.Triggered.Select(x => funcs.IndexOf(x)).ToList(),
					OutputDataId = _rows.IndexOf(func),
					FunctionHeader = func.FunctionHeader
				};
				command_templates.Add(command);
			}

			return command_templates;
		}

		/// <summary>
		/// Возвращает подготовленную управляющую функцию.
		/// </summary>
		/// <param name="name">Название управляющей функции.</param>
		/// <param name="name_space">Пространство имен управляющей функции.</param>
		/// <returns>Управляющая функция.</returns>
		public ControlFunction BuildFunction(string name, List<string> name_space)
		{
			var constants = _rows.Where(x => x.Type == TemplateFunctionRowType.Const).ToList();
			return new ControlFunction()
			{
				Commands = BuildCommands(),
				Constants = constants.Select(x => x.Value).ToList(),
				Header = BuildHeader(name, name_space),
				InputDataCount = _rows.Count(x => x.Type == TemplateFunctionRowType.Input)
			};
		} 
	}
}

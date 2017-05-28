using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;

namespace Core.Model.Commands.Build
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

		#endregion

		#region Methods / Static

		public static ControlFunction Build(string name, List<string> name_space, Func<CommandBuilder> build_func)
		{
			var cmd = build_func.Invoke();
			return cmd.BuildFunction(name, name_space);
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
		public int InputData()
		{
			_inputDataCount++;
			return GetNewTmpDataId(-1);
		}

		/// <summary>
		/// Подгатавливает ячейку для результата выполнения команды и возвращает идентификатор этой ячейки данных.
		/// </summary>
		/// <param name="from_command_id">Идентификатор команды, для которой подготовлена ячейка данных.</param>
		/// <returns>Идентификатор ячейки данных.</returns>
		private int GetNewTmpDataId(int from_command_id)
		{
			_dataFromCommandIds.Add(from_command_id);
			return _dataFromCommandIds.Count - 1;
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
		public int NewCommand(FunctionHeader fucntion_header, IEnumerable<int> input_data)
		{
			var command_id = GetNewTmpFunctionId(out int output_tmp_data_id);

			// Добавляем срабатывемые команды для входных данных.
			foreach (var data in input_data)
			{
				if (data > 0)
				{
					int sorce_command_id = _dataFromCommandIds[data];
					if (sorce_command_id >= 0 && _commandTemplates.Count > sorce_command_id)
					{
						_commandTemplates[_dataFromCommandIds[data]].TriggeredCommandIds.Add(command_id);
					}
				}
			}

			var new_command = new CommandTemplate()
			{
				InputDataIds = input_data.ToList(),
				TriggeredCommandIds = new List<int>(),
				OutputDataId = output_tmp_data_id,
				FunctionHeader = fucntion_header
			};

			_commandTemplates.Add(new_command);

			return output_tmp_data_id;
		}

		/// <summary>
		/// Подгатавливает команду и возвращает её идентификатор.
		/// </summary>
		/// <param name="fucntion">Функция.</param>
		/// <param name="input_data">Входные данные.</param>
		/// <returns>Идентификатор команды.</returns>
		public int NewCommand(Function fucntion, IEnumerable<int> input_data)
		{
			return NewCommand((FunctionHeader)fucntion.Header, input_data);
		}

		public int Constant(object data)
		{
			_constants.Add(data);
			return -_constants.Count();
		}

		/// <summary>
		/// Данные из указанной ячейки становятся возвращаемыми данными.
		/// </summary>
		/// <param name="output_data_id">Идентификатор ячейки данных с выходным параметром.</param>
		public void Return(int output_data_id)
		{
			_commandTemplates[_dataFromCommandIds[output_data_id]].OutputDataId = 0;
		}

		/// <summary>
		/// Возвращает список команд.
		/// </summary>
		/// <returns>Список команд.</returns>
		public List<CommandTemplate> BuildCommands()
		{
			var const_shift = _commandTemplates.Count + _constants.Count + _inputDataCount + 1;
			foreach (var command_template in _commandTemplates)
			{
				for (int i = 0; i < command_template.InputDataIds.Count; i++)
				{
					if (command_template.InputDataIds[i] < 0)
					{
						command_template.InputDataIds[i] += const_shift;
					}
				}
			}
			
			return _commandTemplates;
		}

		/// <summary>
		/// Возвращает подготовленную управляющую функцию.
		/// </summary>
		/// <param name="name">Название управляющей функции.</param>
		/// <param name="name_space">Пространство имен управляющей функции.</param>
		/// <returns>Управляющая функция.</returns>
		public ControlFunction BuildFunction(string name, List<string> name_space)
		{
			return new ControlFunction()
			{
				Commands = BuildCommands(),
				Constants = _constants,
				Header = new ControlFunctionHeader()
				{
					Name = name,
					Owners = new List<Owner>(),
					CallStack = name_space,
				}
			};
		} 
	}
}

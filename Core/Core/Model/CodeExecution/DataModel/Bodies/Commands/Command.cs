using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Base;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Commands;

namespace Core.Model.CodeExecution.DataModel.Bodies.Commands
{
	/// <summary>
	/// Команда. Минимальная единица вычисления.
	/// </summary>
	public class Command : ContainerBase
	{
		/// <summary>
		/// Функция.
		/// </summary>
		public Function Function { get; set; }

		/// <summary>
		/// Список команд, которые зависят от этой.
		/// </summary>
		public List<CommandHeader> TriggeredCommands { get; set; }

		/// <summary>
		/// Список условий, которые должны быть выполнены для начала выполнения этой команды.
		/// </summary>
		public List<DataCell> ConditionData { get; set; }

		/// <summary>
		/// Список ячеек данных входных параметров.
		/// </summary>
		public List<DataCell> InputData { get; set; }

		/// <summary>
		/// Ячейка данных в которую будет помещен результат вычислений.
		/// </summary>
		public DataCell OutputData { get; set; }
	}
}

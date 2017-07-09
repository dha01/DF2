using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.DataFlowLogics.Logics.DataModel
{
    public class StateQueuesInfo
    {
	    /// <summary>
	    /// Все команды.
	    /// </summary>
		public int AllCommandHeaderCount { get; set; }

		/// <summary>
		/// Команды находящиеся в очереди на подготовку.
		/// </summary>
		public int AwaitingPreparationCommandHeaderCount { get; set; }

		/// <summary>
		/// Команды находящиеся в подготовке.
		/// </summary>
		public int PreparationCommandHeaderCount { get; set; }

		/// <summary>
		/// Готовые к исполнению команды ожидающие своей очереди.
		/// </summary>
		public int AwaitingExecutionCommandCount { get; set; }

		/// <summary>
		/// Команды находящиеся в процессе исполнения.
		/// </summary>
		public int ExecutingCommandCount { get; set; }

		/// <summary>
		/// Исполненные команды.
		/// </summary>
		public int ExecutedCommandCount { get; set; }
	}
}

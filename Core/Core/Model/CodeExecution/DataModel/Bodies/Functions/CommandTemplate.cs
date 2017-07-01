using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeExecution.DataModel.Bodies.Functions
{
	/// <summary>
	/// Шаблон управляющей функции.
	/// </summary>
	public class CommandTemplate : Function
	{
		/// <summary>
		/// Заголовок функции.
		/// </summary>
		public FunctionHeader FunctionHeader { get; set; }

		/// <summary>
		/// Список идентификаторов входных параметров.
		/// </summary>
		public List<int> InputDataIds { get; set; }

		/// <summary>
		/// Идентфиикатор выходного параметра.
		/// </summary>
		public int OutputDataId { get; set; }

		/// <summary>
		/// Список идентфикаторов команд, зависимых от этой.
		/// </summary>
		public List<int> TriggeredCommandIds { get; set; }

		/// <summary>
		/// Список идентфиикаторов условий.
		/// </summary>
		public List<int> ConditionId { get; set; } = new List<int>();
	}
}

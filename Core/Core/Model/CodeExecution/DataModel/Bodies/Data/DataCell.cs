using System;

namespace Core.Model.CodeExecution.DataModel.Bodies.Data
{
	/// <summary>
	/// Ячейка дыннх.
	/// </summary>
	public class DataCell : Base.ContainerBase
	{
		/// <summary>
		/// Данные.
		/// </summary>
		public virtual object Data { get; set; }
		
		private bool? _hasValue = null;

		/// <summary>
		/// Содержитли ячейка результат вычислений.
		/// </summary>
		public virtual bool? HasValue { get; set; }

	}
}


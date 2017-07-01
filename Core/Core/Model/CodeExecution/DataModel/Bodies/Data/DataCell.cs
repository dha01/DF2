using System;

namespace Core.Model.CodeExecution.DataModel.Bodies.Data
{
	public class DataCell : Base.ContainerBase
	{
		public virtual object Data { get; set; }


		private bool? _hasValue = null;

		public virtual bool? HasValue
		{
			get { return _hasValue; }
			set
			{
				if (_hasValue.HasValue)
				{
					throw new Exception("Нельзя переопределять это значени!");
				}
				_hasValue = value;
			}
		}

	}
}


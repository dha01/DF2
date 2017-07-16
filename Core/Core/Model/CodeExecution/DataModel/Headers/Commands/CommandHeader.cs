using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Base;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeExecution.DataModel.Headers.Commands
{
	/// <summary>
	/// Заголовок команды.
	/// </summary>
	public class CommandHeader : InvokeHeader, IContainer
	{
		public InvokeHeader Header
		{
			get
			{
				return this;
			}
			set
			{
				CallStack = value.CallStack;
			}
		}

		public virtual List<DataCellHeader> ConditionDataHeaders { get; set; }

		public virtual List<DataCellHeader> InputDataHeaders { get; set; }

		public virtual DataCellHeader OutputDataHeader { get; set; }

		public virtual FunctionHeader FunctionHeader { get; set; }

		public virtual List<InvokeHeader> TriggeredCommands { get; set; }

		public IContainer Clone()
		{
			return (IContainer)MemberwiseClone();
		}

	}
}


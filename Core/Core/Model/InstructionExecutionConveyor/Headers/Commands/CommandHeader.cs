using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Base;
using Core.Model.Headers.Base;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;

namespace Core.Model.Headers.Commands
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
				Owners = value.Owners;
			}
		}

		public virtual List<DataCellHeader> ConditionDataHeaders { get; set; }

		public virtual List<DataCellHeader> InputDataHeaders { get; set; }

		public virtual DataCellHeader OutputDataHeader { get; set; }

		public virtual FunctionHeader FunctionHeader { get; set; }

		public virtual List<InvokeHeader> TriggeredCommands { get; set; }

	}
}


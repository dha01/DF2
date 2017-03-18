using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Functions;
using Core.Model.Repository;

namespace Core.Model.Execution
{
	

	public class ControlExecutionService : IExecutionService<ControlFunction>
	{
		private ICommandRepository _commandRepository;

		public virtual void Execute(ControlFunction function)
		{
			throw new System.NotImplementedException();
		}

	}
}


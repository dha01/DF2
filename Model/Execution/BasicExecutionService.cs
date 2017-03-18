using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Functions;
using Core.Model.Execution;
using Core.Model.Headers.Functions;

namespace Core.Model.Execution
{
	

	public class BasicExecutionService : IExecutionService<BasicFunction>
	{
		public virtual void Execute(BasicFunction function)
		{
			throw new System.NotImplementedException();
		}

	}
}


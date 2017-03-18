using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Functions;

namespace Core.Model.Execution
{
	

	public interface IExecutionService<T>
	{
		void Execute(T function);

	}
}


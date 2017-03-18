using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Functions;

namespace Core.Model.Repository
{
	

	public interface IFunctionRepository  : IContainerRepository<Function, FunctionHeader>
	{
	}
}


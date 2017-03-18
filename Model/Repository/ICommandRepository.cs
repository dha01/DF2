using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Commands;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;

namespace Core.Model.Repository
{


	public interface ICommandRepository : IContainerRepository<CommandHeader, InvokeHeader>
	{
	}
}


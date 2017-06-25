using System;
using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;

namespace Core.Model.CodeExecution.Repository
{
	/// <summary>
	/// Репозиторий команд.
	/// </summary>
	public class CommandRepository : ContainerRepositoryBase<CommandHeader, InvokeHeader>, ICommandRepository
	{
	}
}

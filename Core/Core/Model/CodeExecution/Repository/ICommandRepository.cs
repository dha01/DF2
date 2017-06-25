using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;

namespace Core.Model.CodeExecution.Repository
{


	public interface ICommandRepository : IContainerRepository<CommandHeader, InvokeHeader>
	{
	}
}


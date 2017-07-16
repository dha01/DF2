using Core.Model.CodeExecution.DataModel.Headers.Base;

namespace Core.Model.CodeExecution.DataModel.Bodies.Base
{
	public interface IContainer
	{
		InvokeHeader Header { get; set; }


		Token Token { get; }

		IContainer Clone();
	}
}


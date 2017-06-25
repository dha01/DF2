using System;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;

namespace Core.Model.DataFlowLogics.InstructionExecutionConveyor.Job
{
	

	public interface IJobManager 
	{
		Action<Command> OnReliseJob { get; set; }

		void AddCommand(Command command);

		bool HasFreeJob();

		int GetFreeJobCount();

	}
}


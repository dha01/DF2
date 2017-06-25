using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Headers.Commands;

namespace Core.Model.Job
{
	

	public interface IJobManager 
	{
		Action<Command> OnReliseJob { get; set; }

		void AddCommand(Command command);

		bool HasFreeJob();

		int GetFreeJobCount();

	}
}


using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Headers.Commands;

namespace Core.Model.CodeExecution.Service.Computing
{
	

	public interface ICommandManager 
	{
		void AddHeaders(IEnumerable<CommandHeader> command_headers);
		/*
		void AddHeader(DataCellHeader data_cell_header);

		void AddHeader(FunctionHeader function_header);*/

	}
}


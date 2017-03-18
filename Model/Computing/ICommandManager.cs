using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;

namespace Core.Model.Computing
{
	

	public interface ICommandManager 
	{
		void AddHeaders(IEnumerable<CommandHeader> command_headers);
		/*
		void AddHeader(DataCellHeader data_cell_header);

		void AddHeader(FunctionHeader function_header);*/

	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.Bodies.Base;
using Core.Model.Bodies.Commands;
using Core.Model.Bodies.Data;
using Core.Model.Bodies.Functions;
using Core.Model.Headers.Base;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;
using Core.Model.Headers.Functions;

namespace Core.Model.Computing
{
	

	public interface IComputingCore 
	{
		void Invoke(object first_command);

		void AddCommandHeaders(IEnumerable<CommandHeader> command_headers);
		IEnumerable<CommandHeader> GetCommandHeaders(IEnumerable<InvokeHeader> invoke_header);

		void AddDataCell(IEnumerable<DataCell> data_cells);

		IEnumerable<DataCell> GetDataCell(IEnumerable<DataCellHeader> data_cell_headers);

		void AddFunctions(IEnumerable<Function> function);

		void AddAssembly(string path);

		IEnumerable<Function> GetFunctions(IEnumerable<FunctionHeader> functions);

	}
}


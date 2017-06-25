using System.Collections.Generic;
using Core.Model.CodeExecution.DataModel.Bodies.Data;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Data;
using Core.Model.CodeExecution.DataModel.Headers.Functions;

namespace Core.Model.CodeExecution.Service.Computing
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


using System;
using Core.Model.CodeExecution.DataModel.Bodies.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Base;
using Core.Model.CodeExecution.DataModel.Headers.Commands;
using Core.Model.CodeExecution.DataModel.Headers.Data;

namespace Core.Model.DataFlowLogics.Logics.Service
{
	public interface IPreparationCommandService
	{
		Action<Command> OnPreparedCommand { get; set; }
		
		void PrepareCommand(Token command_header_token);

		void OnDataReady(string data_cell_header_token);

		void Clear(string path);
	}
}

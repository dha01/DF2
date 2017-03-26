using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model.Bodies.Commands;
using Core.Model.Headers.Commands;
using Core.Model.Headers.Data;

namespace Core.Model.DataFlowLogics.Logics.Service
{
	public interface IPreparationCommandService
	{
		Action<Command> OnPreparedCommand { get; set; }
		
		void PrepareCommand(CommandHeader command_header);

		void OnDataReady(DataCellHeader data_cell_header);
	}
}

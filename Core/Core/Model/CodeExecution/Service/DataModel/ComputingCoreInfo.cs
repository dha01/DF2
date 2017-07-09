using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.CodeExecution.DataModel;
using Core.Model.DataFlowLogics.Logics.DataModel;

namespace Core.Model.CodeExecution.Service.DataModel
{
    public class ComputingCoreInfo
    {
		public StateQueuesInfo StateQueuesInfo { get; set; }
		public ConteinerRepositoryInfo DataCellsInfo { get; set; }
    }
}

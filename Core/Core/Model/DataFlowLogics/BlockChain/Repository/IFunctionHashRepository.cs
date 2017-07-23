using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Repository
{
	public interface IFunctionHashRepository
    {
	    FunctionHash Get(string hash);

	    void Set(params FunctionHash[] data_cells);
	}
}

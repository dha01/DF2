using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
	public interface IFunctionHashService
	{

		void Set(params FunctionHash[] function_hashs);
		FunctionHash GetLocal(string function_hash);
		FunctionHash GetGlobal(string function_hash);
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
	public interface ITransactionExecutorService
	{
		void Execute(Transaction transaction);
	}
}

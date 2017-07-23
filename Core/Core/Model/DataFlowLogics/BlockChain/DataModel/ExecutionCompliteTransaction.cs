using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.DataFlowLogics.BlockChain.DataModel
{
    public class ExecutionCompliteTransaction : Transaction
	{
		public ExecutionCompliteTransaction(string from_transaction)
		{
			_hash = from_transaction;
		}

		public int Index { get; set; }
		public string Temp { get; set; }
	}
}

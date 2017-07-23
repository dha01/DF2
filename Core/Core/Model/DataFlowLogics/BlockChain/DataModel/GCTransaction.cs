using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.DataFlowLogics.BlockChain.DataModel
{
	public class GCTransaction : Transaction
	{
		public GCTransaction(string from_transaction)
		{
			_hash = from_transaction;
		}

		public string[] DataCells { get; set; }
	}
}

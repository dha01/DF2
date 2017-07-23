using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.DataFlowLogics.BlockChain.Repository
{
	public class FunctionHashRepository : IFunctionHashRepository
	{
	    private readonly ConcurrentDictionary<string, FunctionHash> _dataCells = new ConcurrentDictionary<string, FunctionHash>();

	    public FunctionHash Get(string hash)
	    {
		    return _dataCells.TryGetValue(hash, out FunctionHash val) ? val : null;
	    }

	    public void Set(params FunctionHash[] function_hashs)
	    {
		    foreach (var function_hash in function_hashs)
		    {
			    if (_dataCells.TryGetValue(function_hash.Hash, out FunctionHash val))
			    {
				    _dataCells.TryUpdate(function_hash.Hash, function_hash, val);
			    }
			    else
			    {
				    _dataCells.TryAdd(function_hash.Hash, function_hash);
				}
		    }
	    }
	}
}

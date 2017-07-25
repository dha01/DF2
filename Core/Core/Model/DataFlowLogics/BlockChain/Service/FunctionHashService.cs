using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Core.Model.CodeExecution.DataModel.Bodies.Functions;
using Core.Model.DataFlowLogics.BlockChain.DataModel;
using Core.Model.DataFlowLogics.BlockChain.Repository;

namespace Core.Model.DataFlowLogics.BlockChain.Service
{
    public class FunctionHashService : IFunctionHashService
    {
	    private readonly IFunctionHashRepository _functionHashRepository;
	    private readonly IDataCellHashRepository _dataCellHashRepository;

		public FunctionHashService(IFunctionHashRepository function_hash_repository, IDataCellHashRepository data_cell_hash_repository)
	    {
		    _functionHashRepository = function_hash_repository;
		    _dataCellHashRepository = data_cell_hash_repository;
	    }
		
		public void Set(params FunctionHash[] function_hashs)
	    {

			var function_constans = new List<DataCellHash>();
			foreach (var function_hash in function_hashs)
		    {
			    if (function_hash.Function is ControlFunction)
			    {
				    var control_function = (ControlFunction) function_hash.Function;
				    for (int i = 0; i < control_function.Constants.Count; i++)
				    {
					    var cons = control_function.Constants[i];
						function_constans.Add(new DataCellHash
					    {
						    Value = cons,
						    Type = cons.GetType().ToString(),
						    Hash = Transaction.GetHash($"{control_function.Token.Hash}/Const_{i}")
					    });
					}
			    }
		    }

		    if (function_constans.Any())
		    {
			    _dataCellHashRepository.Set(function_constans.ToArray());
		    }

			_functionHashRepository.Set(function_hashs);

		}

		public FunctionHash GetLocal(string function_hash)
		{
			return _functionHashRepository.Get(function_hash);
		}

		public FunctionHash GetGlobal(string function_hash)
		{
			throw new NotImplementedException();
		}
	}
}

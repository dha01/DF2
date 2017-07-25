using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model.DataFlowLogics.BlockChain.DataModel
{
    public class ExecutionTransaction : Transaction
	{
		public override void RecalacHash()
		{
			_hash = GetHash($"{TaskHash}/{ParentTransaction}/{ParentFunction}/{Function}<{Index}>/{string.Join(",", Inputs)}");
		}

		public string ParentTransaction { get; set; }
		public int Index { get; set; }
		public string[] Inputs { get; set; }
		public string[] Temps { get; set; }
		public string Function { get; set; }
		public string ParentFunction { get; set; }

		public override Transaction Clone()
		{
			return new ExecutionTransaction
			{
				_hash = _hash,
				TaskHash = TaskHash,
				ParentTransaction = ParentTransaction,
				Index = Index,
				Inputs = Inputs?.ToArray(),
				Temps = Temps?.ToArray(),
				Function = Function,
				ParentFunction = ParentFunction
			};
		}

		public string GetOutputHash()
		{
			return GetHash($"{TaskHash}/{ParentTransaction}/{ParentFunction}/{Function}<{Index}>/{string.Join(",", Inputs)}/result");
		}

		public ExecutionCompliteTransaction GetCompliteExecutionTransaction()
		{
			return new ExecutionCompliteTransaction(ParentTransaction)
			{
				Index = Index,
				TaskHash = TaskHash,
				Temp = GetOutputHash()
			};
		}

		public static ExecutionTransaction operator +(ExecutionTransaction first, ExecutionTransaction second)
		{

			if (first.Temps == null)
			{
				first.Temps = second.Temps;
			}
			else
			{
				if (second.Temps != null)
				{
					for (int i = 0; i < first.Temps.Length; i++)
					{
						if ((first.Temps[i] == null && second.Temps[i] != null) || second.Temps[i] == "clear")
						{
							first.Temps[i] = second.Temps[i];
						}
					}
				}
			}

			return first;
		}

		public static ExecutionTransaction operator +(ExecutionTransaction first, ExecutionCompliteTransaction second)
		{
			if (first.Temps[second.Index] == null || second.Temp == "clear")
			{
				first.Temps[second.Index] = second.Temp;
			}
			return first;
		}
	}
}

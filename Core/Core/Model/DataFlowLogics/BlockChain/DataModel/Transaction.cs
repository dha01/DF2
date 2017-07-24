using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Core.Model.DataFlowLogics.BlockChain.DataModel
{
	public class Transaction
	{
		protected static SHA512 _mySHA512 = SHA512.Create();

		public virtual void RecalacHash()
		{
			IEnumerable<byte> bytes = Encoding.ASCII.GetBytes(TaskHash);
			_hash = Convert.ToBase64String(_mySHA512.ComputeHash(bytes.ToArray()));
		}

		protected string _hash;

		public string Hash
		{
			get
			{
				if (_hash == null)
				{
					RecalacHash();
				}
				return _hash;
			}
		}
		
		public virtual string TaskHash { get; set; }

		public virtual Transaction Clone()
		{
			return new Transaction
			{
				_hash = _hash,
				TaskHash = TaskHash
			};
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Core.Model.DataFlowLogics.BlockChain.DataModel
{
	public class Transaction
	{
		public virtual void RecalacHash()
		{
			_hash = GetHash(TaskHash);
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

		public static string GetHash(string text)
		{
			var my_sha = SHA512.Create();
			return Convert.ToBase64String(my_sha.ComputeHash(Encoding.ASCII.GetBytes(text)));
		}
	}
}

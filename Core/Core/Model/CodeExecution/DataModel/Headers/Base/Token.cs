using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Core.Model.DataFlowLogics.BlockChain.DataModel;

namespace Core.Model.CodeExecution.DataModel.Headers.Base
{
	public struct Token
	{
		


		public static TokenPart Parse(string token)
		{
			Regex myReg = new Regex("<.>");
			Match matche = myReg.Match(token);

			if (!string.IsNullOrWhiteSpace(matche.Value))
			{
				token = token.Replace(matche.Value, "");
			}

			return new TokenPart
			{
				Index = int.TryParse(matche.Value.Trim(new[] { '<', '>' }), out int result) ? result : (int?)null,
				Name = token
			};
		}



		private string _value;

		public string Hash { get; }
		
		public Token(string token)
		{
			_value = token;
			Hash = Transaction.GetHash(_value);
		}

		public enum TokenType
		{
			Func,
			Path
		}

		private char GetSeparator(TokenType type)
		{
			return type == TokenType.Path ? '/' : '.';
		}

		public string Last(TokenType type = TokenType.Path)
		{
			return _value.Split(GetSeparator(type)).Last();
		}

		public Token Prev(TokenType type = TokenType.Path)
		{
			//var index = ;

			return _value.Remove(_value.LastIndexOf(GetSeparator(type)));
		}
		public Token Next(string value, TokenType type = TokenType.Path)
		{
			return $"{_value}{GetSeparator(type)}{value}";
		}

		public IEnumerable<string> ToEnumerable(TokenType type = TokenType.Path)
		{
			return _value.Split(GetSeparator(type));
		}

		public string[] ToArray(TokenType type = TokenType.Path)
		{
			return ToEnumerable(type).ToArray();
		}
		public List<string> ToList(TokenType type = TokenType.Path)
		{
			return ToEnumerable(type).ToList();
		}

		public override string ToString()
		{
			return _value;
		}

		public static implicit operator string(Token token)
		{
			return token._value;
		}

		public static implicit operator Token(string token)
		{
			return new Token(token);
		}
	}
}

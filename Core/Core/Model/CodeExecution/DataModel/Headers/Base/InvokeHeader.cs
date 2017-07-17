using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Model.CodeExecution.DataModel.Headers.Base
{
	/*public enum TokenPartType
	{
		User,
		Process,
		ControlFunction,
		CSharpFunction,
		Const,
		TmpVar,
		Result
	}

	public struct TokenPart
	{
		public TokenPartType Type { get; set; }
		public int? Index { get; set; }
		public string Name { get; set; }
	}

	public struct Token
	{
		private TokenPart[] _parts;
		public TokenPart[] Parts
		{
			get => _parts;

			set
			{
				if (_parts != null)
				{
					throw new Exception("Нельзя переопределять это значение!");
				}

				_stringToken = GetStringToken();
				_parts = value;
			}
		}

		private string GetStringToken()
		{
			var sb = new StringBuilder();

			foreach (var part in Parts)
			{
				sb.Append($"{part.Type}_{part.Name}{(part.Index.HasValue ? $"<{part.Index}>" : "")}");
			}

			return sb.ToString();
		}

		private string _stringToken;


		public Token Next(TokenPart part)
		{
			return new Token
			{
				Parts = _parts.ToList().Concat(new []{ part }).ToArray()
			};
		}

		public Token Prev()
		{
			return new Token
			{
				Parts = _parts.Take(_parts.Length - 1).ToArray()
			};
		}

		public override string ToString()
		{
			return _stringToken;
		}

		public static explicit operator string(Token token)
		{
			return token._stringToken;
		}
	}*/

	public struct TokenPart
	{
		public int? Index { get; set; }
		public string Name { get; set; }
	}

	public struct Token
	{
		public static TokenPart Parse(string token)
		{
			Regex myReg = new Regex("<.>");
			Match matche = myReg.Match(token);

			if(!string.IsNullOrWhiteSpace(matche.Value))
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

		public Token(string token)
		{
			_value = token;
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
		public Token Next(string value,TokenType type = TokenType.Path)
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
	
	
	/// <summary>
	/// Заголовок для исполнения.
	/// </summary>
	public class InvokeHeader : Header
	{
		public virtual Token Token { get; set; }
	}
}


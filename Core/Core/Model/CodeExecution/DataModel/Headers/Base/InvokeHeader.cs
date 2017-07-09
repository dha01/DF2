using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model.CodeExecution.DataModel.Headers.Base
{
	public enum TokenPartType
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
				sb.Append($"{part.Name}{(part.Index.HasValue ? $"<{part.Index}>" : "")}");
				/*
				switch (part.Type)
				{
					case TokenPartType.User:
						sb.Append($"{part.Name}");
						break;
					case TokenPartType.CSharpFunction:
						sb.Append($"{part.Name}{(part.Index.HasValue ? $"<{part.Index}>" : "")}");
						break;
					case TokenPartType.Const:
						sb.Append($"{part.Name}{(part.Index.HasValue ? $"<{part.Index}>" : "")}");
						break;
					case TokenPartType.ControlFunction:
						sb.Append($"{part.Name}{(part.Index.HasValue ? $"<{part.Index}>": "")}");
						break;
					case TokenPartType.Process:
						sb.Append($"{part.Name}");
						break;
					case TokenPartType.Result:
						sb.Append($"{part.Name}");
						break;
					case TokenPartType.TmpVar:
						sb.Append($"{part.Name}{(part.Index.HasValue ? $"<{part.Index}>" : "")}");
						break;
				}*/
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
	}
	
	
	/// <summary>
	/// Заголовок для исполнения.
	/// </summary>
	public class InvokeHeader : Header
	{
		public virtual string[] CallStack { get; set; }

		protected string _token;

		public virtual string Token
		{
			get
			{
				if (string.IsNullOrEmpty(_token))
				{
					_token = string.Join("/", CallStack);
				}
				return _token;
			}
		}

		public bool Equals(InvokeHeader obj)
		{
			return CallStack.SequenceEqual(obj.CallStack);
		}
	}
}


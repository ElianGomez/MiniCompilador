using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompilador.Compiler
{
	public sealed class Lexer
	{
		private readonly string _src;
		private int _i;
		private int _line = 1;
		private int _col = 1;

		public Lexer(string src) => _src = (src ?? string.Empty).Replace("\r\n", "\n");

		public IEnumerable<Token> Scan()
		{
			while (!End())
			{
				char c = Peek();
				if (c == '\n') { Next(); yield return new Token(TokenKind.Eol, "\\n", _line - 1, 1); continue; }
				if (char.IsWhiteSpace(c)) { Next(); continue; }

				if (char.IsDigit(c)) { yield return Int(); continue; }
				if (char.IsLetter(c) || c == '_') { yield return IdOrKw(); continue; }

				yield return c switch
				{
					'+' => Tok(TokenKind.Plus, "+"),
					'-' => Tok(TokenKind.Minus, "-"),
					'*' => Tok(TokenKind.Star, "*"),
					'/' => Tok(TokenKind.Slash, "/"),
					'(' => Tok(TokenKind.LParen, "("),
					')' => Tok(TokenKind.RParen, ")"),
					'=' => Tok(TokenKind.Equal, "="),
					_ => throw LexError($"Carácter inesperado '{c}'")
				};
			}

			yield return new Token(TokenKind.Eof, "<eof>", _line, _col);
		}

		private Token Int()
		{
			int startI = _i; int startC = _col;
			while (!End() && char.IsDigit(Peek())) Next();
			string s = _src[startI.._i];
			return new Token(TokenKind.Int, s, _line, startC, int.Parse(s));
		}

		private Token IdOrKw()
		{
			int startI = _i; int startC = _col;
			while (!End() && (char.IsLetterOrDigit(Peek()) || Peek() == '_')) Next();
			string s = _src[startI.._i];
			return s switch
			{
				"Paper" => new Token(TokenKind.Paper, s, _line, startC),
				"Pen" => new Token(TokenKind.Pen, s, _line, startC),
				"Color" => new Token(TokenKind.Color, s, _line, startC),
				"Line" => new Token(TokenKind.Line, s, _line, startC),
				"Rect" => new Token(TokenKind.Rect, s, _line, startC),
				"Circle" => new Token(TokenKind.Circle, s, _line, startC),
				"let" => new Token(TokenKind.Let, s, _line, startC),
				"true" => new Token(TokenKind.True, s, _line, startC),
				"false" => new Token(TokenKind.False, s, _line, startC),
				_ => new Token(TokenKind.Id, s, _line, startC)
			};
		}

		private Token Tok(TokenKind k, string s)
		{
			var t = new Token(k, s, _line, _col);
			Next();
			return t;
		}

		private bool End() => _i >= _src.Length;
		private char Peek() => _src[_i];

		private void Next()
		{
			if (_src[_i++] == '\n') { _line++; _col = 1; }
			else _col++;
		}

		private System.Exception LexError(string msg) =>
			new System.Exception($"[LEX] {msg} en línea {_line}, columna {_col}");
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompilador.Compiler
{
	public enum TokenKind
	{
		Paper, Pen, Color, Line, Rect, Circle, Let, True, False,
		Id, Int,
		Plus, Minus, Star, Slash,
		LParen, RParen,
		Equal,
		Eol,
		Eof
	}

	public readonly record struct Token(
		TokenKind Kind,
		string Lexeme,
		int Line,
		int Col,
		int IntVal = 0
	)

	{
		public override string ToString() => $"{Kind} '{Lexeme}' @({Line},{Col})";
	}

}

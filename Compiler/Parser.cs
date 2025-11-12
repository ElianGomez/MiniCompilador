using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompilador.Compiler
{
	public sealed class Parser
	{
		private readonly List<Token> _t;
		private int _p;

		public Parser(IEnumerable<Token> tokens)
		{
			// Quitamos EOL para simplificar; cada instrucción es por línea pero no la necesitamos aquí
			_t = tokens.Where(t => t.Kind != TokenKind.Eol).ToList();
		}

		public ProgramNode Parse()
		{
			var list = new List<Stmt>();
			while (!IsAtEnd())              // ← antes: while (!Check(TokenKind.Eof))
				list.Add(Stmt());
			return new ProgramNode(list);
		}


		private Stmt Stmt()
		{
			return Peek().Kind switch
			{
				TokenKind.Paper => Paper(),
				TokenKind.Pen => Pen(),
				TokenKind.Color => Color(),
				TokenKind.Line => Line(),
				TokenKind.Rect => Rect(),
				TokenKind.Circle => Circle(),
				TokenKind.Let => Let(),
				_ => throw Error("Se esperaba inicio de instrucción")
			};
		}

		private Stmt Paper() { Consume(TokenKind.Paper); var s = Expr(); return new PaperStmt(s); }
		private Stmt Pen() { Consume(TokenKind.Pen); var w = Expr(); return new PenStmt(w); }
		private Stmt Color()
		{
			Consume(TokenKind.Color);
			var r = Expr(); var g = Expr(); var b = Expr();
			return new ColorStmt(r, g, b);
		}
		private Stmt Line()
		{
			Consume(TokenKind.Line);
			var x1 = Expr(); var y1 = Expr(); var x2 = Expr(); var y2 = Expr();
			return new LineStmt(x1, y1, x2, y2);
		}
		private Stmt Rect()
		{
			Consume(TokenKind.Rect);
			var x = Expr(); var y = Expr(); var w = Expr(); var h = Expr();
			bool? fill = null;
			if (Match(TokenKind.True)) fill = true;
			else if (Match(TokenKind.False)) fill = false;
			return new RectStmt(x, y, w, h, fill);
		}
		private Stmt Circle()
		{
			Consume(TokenKind.Circle);
			var cx = Expr(); var cy = Expr(); var r = Expr();
			bool? fill = null;
			if (Match(TokenKind.True)) fill = true;
			else if (Match(TokenKind.False)) fill = false;
			return new CircleStmt(cx, cy, r, fill);
		}
		private Stmt Let()
		{
			Consume(TokenKind.Let);
			var id = Consume(TokenKind.Id).Lexeme;
			Consume(TokenKind.Equal);
			var v = Expr();
			return new LetStmt(id, v);
		}

		// --- Expressions (RD) ---
		private Expr Expr()
		{
			var e = Term();
			while (Match(TokenKind.Plus) || Match(TokenKind.Minus))
			{
				string op = Prev().Lexeme;
				var r = Term();
				e = new BinExpr(op, e, r);
			}
			return e;
		}

		private Expr Term()
		{
			var e = Factor();
			while (Match(TokenKind.Star) || Match(TokenKind.Slash))
			{
				string op = Prev().Lexeme;
				var r = Factor();
				e = new BinExpr(op, e, r);
			}
			return e;
		}

		private Expr Factor()
		{
			if (Match(TokenKind.Int)) return new IntExpr(Prev().IntVal);
			if (Match(TokenKind.Id)) return new IdExpr(Prev().Lexeme);
			if (Match(TokenKind.LParen))
			{
				var e = Expr();
				Consume(TokenKind.RParen);
				return e;
			}
			throw Error("Expresión inválida");
		}

		// --- helpers ---
		private bool Match(TokenKind kind)
		{
			if (Check(kind)) { _p++; return true; }
			return false;
		}
		private Token Consume(TokenKind kind)
		{
			if (Check(kind)) return _t[_p++];
			throw Error($"Se esperaba token {kind} y se encontró {Peek().Kind}");
		}
		private Token Consume(TokenKind k1, TokenKind k2)
		{
			if (Check(k1) || Check(k2)) return _t[_p++];
			throw Error($"Se esperaba {k1} o {k2}");
		}
		private bool Check(TokenKind kind) => !IsAtEnd() && Peek().Kind == kind;
		private bool IsAtEnd() => Peek().Kind == TokenKind.Eof;
		private Token Peek() => _t[_p];
		private Token Prev() => _t[_p - 1];

		private Exception Error(string msg)
			=> new Exception($"[PARSE] {msg} en token {Peek()}");
	}
}

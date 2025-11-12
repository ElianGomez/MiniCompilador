using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompilador.Compiler
{
	public class Semantic
	{
		public sealed class SemanticAnalyzer
		{
			private readonly List<string> _errors = new();
			private int? _paperSize;
			private readonly SymbolTable _sym;

			public SemanticAnalyzer(SymbolTable sym) { _sym = sym; }

			public IReadOnlyList<string> Analyze(ProgramNode p)
			{
				try
				{
					foreach (var s in p.Statements) CheckStmt(s);
				}
				catch (Exception ex)
				{
					_errors.Add($"[SEM] {ex.Message}");
				}
				return _errors;
			}

			private void CheckStmt(Stmt s)
			{
				switch (s)
				{
					case PaperStmt ps:
						_paperSize = Eval(ps.Size);
						if (_paperSize <= 0) Err("Paper debe ser > 0");
						break;

					case LetStmt ls:
						_sym.Set(ls.Id, Eval(ls.Value));
						break;

					case PenStmt pen:
						if (Eval(pen.Width) <= 0) Err("Pen debe ser > 0");
						break;

					case ColorStmt c:
						CheckByte(Eval(c.R), "R");
						CheckByte(Eval(c.G), "G");
						CheckByte(Eval(c.B), "B");
						break;

					case LineStmt l:
						CheckInside(l.X1, l.Y1, l.X2, l.Y2);
						break;

					case RectStmt r:
						CheckInside(r.X, r.Y, r.W, r.H);
						break;

					case CircleStmt c:
						// Validación simple: centro y radio dentro del paper
						var cx = Eval(c.Cx); var cy = Eval(c.Cy); var rad = Eval(c.R);
						if (_paperSize is null) Err("Debe definirse Paper antes de dibujar.");
						if (rad <= 0) Err("Radio debe ser > 0");
						if (cx < 0 || cx > _paperSize || cy < 0 || cy > _paperSize) Err("Centro del círculo fuera de Paper");
						if (cx - rad < 0 || cy - rad < 0 || cx + rad > _paperSize || cy + rad > _paperSize) Err("Círculo excede Paper");
						break;
				}
			}

			private int Eval(Expr e) => e switch
			{
				IntExpr i => i.Value,
				IdExpr id => _sym.TryGet(id.Name, out var v)
					? v : throw new Exception($"Variable '{id.Name}' no declarada"),
				BinExpr b => b.Op switch
				{
					"+" => Eval(b.Left) + Eval(b.Right),
					"-" => Eval(b.Left) - Eval(b.Right),
					"*" => Eval(b.Left) * Eval(b.Right),
					"/" => Eval(b.Left) / Math.Max(1, Eval(b.Right)),
					_ => throw new Exception("Operador no soportado")
				},
				_ => throw new Exception("Expresión no soportada")
			};

			private void CheckByte(int v, string c)
			{
				if (v < 0 || v > 255) Err($"Color {c} fuera de rango (0..255)");
			}

			private void CheckInside(params Expr[] list)
			{
				if (_paperSize is null) Err("Debe definirse Paper antes de dibujar.");
				foreach (var e in list)
				{
					var v = Eval(e);
					if (v < 0 || v > _paperSize) Err($"Coordenada {v} fuera de Paper (0..{_paperSize})");
				}
			}

			private void Err(string m) => _errors.Add(m);
		}
	}
}

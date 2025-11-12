using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompilador.Compiler
{
	public readonly record struct Quad(string Op, params int[] Args);

	public sealed class IrGenerator
	{
		private readonly List<Quad> _ir = new();
		private readonly SymbolTable _sym;

		public IrGenerator(SymbolTable sym) { _sym = sym; }

		public IReadOnlyList<Quad> Generate(ProgramNode p)
		{
			foreach (var s in p.Statements) Emit(s);
			return _ir;
		}

		private int E(Expr e) => e switch
		{
			IntExpr i => i.Value,
			IdExpr id => _sym.TryGet(id.Name, out var v) ? v : 0, // si no existe, 0 (ya habría fallado en semántico)
			BinExpr b => b.Op switch
			{
				"+" => E(b.Left) + E(b.Right),
				"-" => E(b.Left) - E(b.Right),
				"*" => E(b.Left) * E(b.Right),
				"/" => (E(b.Right) == 0) ? 0 : E(b.Left) / E(b.Right),
				_ => 0
			},
			_ => 0
		};

		private void Emit(Stmt s)
		{
			switch (s)
			{
				case PaperStmt ps:
					_ir.Add(new Quad("PAPER", E(ps.Size)));
					break;

				case PenStmt pen:
					_ir.Add(new Quad("PEN", E(pen.Width)));
					break;

				case ColorStmt c:
					_ir.Add(new Quad("COLOR", E(c.R), E(c.G), E(c.B)));
					break;

				case LineStmt l:
					_ir.Add(new Quad("LINE", E(l.X1), E(l.Y1), E(l.X2), E(l.Y2)));
					break;

				case RectStmt r:
					_ir.Add(new Quad("RECT", E(r.X), E(r.Y), E(r.W), E(r.H), r.Fill == true ? 1 : 0));
					break;

				case CircleStmt c:
					_ir.Add(new Quad("CIRCLE", E(c.Cx), E(c.Cy), E(c.R), c.Fill == true ? 1 : 0));
					break;

				case LetStmt let:
					// Registrado en simbólica/semántica; aquí solo dejamos constancia
					_ir.Add(new Quad("MOV")); // sin args
					break;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompilador.Compiler
{
	public abstract record Node;
	public abstract record Stmt : Node;
	public abstract record Expr : Node;

	public record ProgramNode(List<Stmt> Statements) : Node;

	// Statements
	public record PaperStmt(Expr Size) : Stmt;
	public record PenStmt(Expr Width) : Stmt;
	public record ColorStmt(Expr R, Expr G, Expr B) : Stmt;
	public record LineStmt(Expr X1, Expr Y1, Expr X2, Expr Y2) : Stmt;
	public record RectStmt(Expr X, Expr Y, Expr W, Expr H, bool? Fill) : Stmt;
	public record CircleStmt(Expr Cx, Expr Cy, Expr R, bool? Fill) : Stmt;
	public record LetStmt(string Id, Expr Value) : Stmt;

	// Expressions
	public record IntExpr(int Value) : Expr;
	public record IdExpr(string Name) : Expr;
	public record BinExpr(string Op, Expr Left, Expr Right) : Expr;
}

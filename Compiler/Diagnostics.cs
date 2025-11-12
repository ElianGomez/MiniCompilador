using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniCompilador.Compiler.Semantic;

namespace MiniCompilador.Compiler
{
	public static class Diagnostics
	{
		public static List<string> CollectAll(string source, out ProgramNode? ast, out List<Token> tokens, out IReadOnlyList<Quad> ir, out string svg, out SymbolTable symbols)
		{
			tokens = new List<Token>(new Lexer(source).Scan());
			var parser = new Parser(tokens);
			ast = parser.Parse();

			symbols = new SymbolTable();
			var sema = new SemanticAnalyzer(symbols);
			var semErrors = sema.Analyze(ast);

			var irGen = new IrGenerator(symbols);
			ir = irGen.Generate(ast);

			var svgCode = new CodeGenSvg();
			svg = svgCode.Generate(ir);

			var errs = new List<string>(semErrors);
			return errs;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompilador.Compiler
{
	public sealed class CodeGenSvg
	{
		public string Generate(IReadOnlyList<Quad> ir)
		{
			int size = 100;
			int pen = 1;
			var color = (r: 0, g: 0, b: 0);

			// Primera pasada: hallar parámetros globales finales (size/pen/color)
			foreach (var q in ir)
			{
				switch (q.Op)
				{
					case "PAPER": size = q.Args[0]; break;
					case "PEN": pen = q.Args[0]; break;
					case "COLOR": color = (q.Args[0], q.Args[1], q.Args[2]); break;
				}
			}

			var sb = new StringBuilder();
			sb.AppendLine($"<svg width=\"{size}\" height=\"{size}\" viewBox=\"0 0 {size} {size}\" xmlns=\"http://www.w3.org/2000/svg\">");

			string Stroke(int? customPen = null, (int r, int g, int b)? customCol = null)
			{
				var (cr, cg, cb) = customCol ?? color;
				int pw = customPen ?? pen;
				return $"stroke=\"rgb({cr},{cg},{cb})\" stroke-width=\"{pw}\"";
			}

			foreach (var q in ir)
			{
				switch (q.Op)
				{
					case "LINE":
						sb.AppendLine($"  <line x1=\"{q.Args[0]}\" y1=\"{q.Args[1]}\" x2=\"{q.Args[2]}\" y2=\"{q.Args[3]}\" {Stroke()} fill=\"none\"/>");
						break;

					case "RECT":
						string fillR = q.Args.Length > 4 && q.Args[4] == 1
							? $"fill=\"rgb({color.r},{color.g},{color.b})\""
							: "fill=\"none\"";
						sb.AppendLine($"  <rect x=\"{q.Args[0]}\" y=\"{q.Args[1]}\" width=\"{q.Args[2]}\" height=\"{q.Args[3]}\" {Stroke()} {fillR}/>");
						break;

					case "CIRCLE":
						string fillC = q.Args.Length > 3 && q.Args[3] == 1
							? $"fill=\"rgb({color.r},{color.g},{color.b})\""
							: "fill=\"none\"";
						sb.AppendLine($"  <circle cx=\"{q.Args[0]}\" cy=\"{q.Args[1]}\" r=\"{q.Args[2]}\" {Stroke()} {fillC}/>");
						break;

					default:
						// MOV, PAPER, PEN, COLOR: no emiten elementos
						break;
				}
			}

			sb.AppendLine("</svg>");
			return sb.ToString();
		}
	}
}

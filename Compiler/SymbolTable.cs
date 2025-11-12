using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompilador.Compiler
{
	public sealed class SymbolTable
	{
		private readonly Dictionary<string, int> _vars = new();

		public bool TryGet(string id, out int v) => _vars.TryGetValue(id, out v);

		public void Set(string id, int v) => _vars[id] = v;

		public IReadOnlyDictionary<string, int> Snapshot() => _vars;
	}
}

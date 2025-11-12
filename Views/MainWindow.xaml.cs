using MiniCompilador.Compiler;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniCompilador
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			txtSource.Text = Sample();
			btnCompile.Click += (_, __) => Compile();
		}

		private void Compile()
		{
			try
			{
				var errors = Diagnostics.CollectAll(
					txtSource.Text,
					out var ast,
					out var tokens,
					out var ir,
					out var svg,
					out var symbols
				);

				dgTokens.ItemsSource = tokens.Where(t => t.Kind != TokenKind.Eof).ToList();
				dgIr.ItemsSource = ir.Select(q => new { q.Op, Args = string.Join(", ", q.Args ?? Array.Empty<int>()) }).ToList();
				txtSvg.Text = svg;
				lstErrors.ItemsSource = errors;

				var tempPath = System.IO.Path.ChangeExtension(
	System.IO.Path.GetTempFileName(), ".svg");
				System.IO.File.WriteAllText(tempPath, svg);

				// Forzar refresco (por si usas el mismo archivo):
				svgView.Navigate(tempPath + "?" + DateTime.Now.Ticks);

			}
			catch (Exception ex)
			{
				lstErrors.ItemsSource = new[] { ex.Message };
			}
		}

		private static string Sample() =>
				@"Paper 100
			Pen 2
			Color 0 0 0 
			let a = 50
			let b = 22
			Line a b 78 b
			Line a b 22 78
			Line 78 b 22 78";
	}
}

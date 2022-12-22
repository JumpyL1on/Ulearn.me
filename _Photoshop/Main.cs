using System;
using System.Windows.Forms;

namespace MyPhotoshop
{
	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var window = new MainWindow();

			window.AddFilter(new PixelFilter<LighteningParameters>(
				"����������/����������",
				(original, parameters) => original * parameters.Coefficient
				));

			window.AddFilter(new PixelFilter<EmptyParameters>(
				"������� ������",
				(original, parameters) =>
				{
                    var average = (original.R + original.G + original.B) / 3;

                    return new Pixel(average, average, average);
                }
				));

			Application.Run(window);
		}
	}
}
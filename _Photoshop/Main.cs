using MyPhotoshop.Transformers;
using System;
using System.Drawing;
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
				(original, parameters) => original * parameters.Coefficient,
				new SimpleParametersHandler<LighteningParameters>()
                ));

			window.AddFilter(new PixelFilter<EmptyParameters>(
				"������� ������",
				(original, parameters) =>
				{
					var average = (original.R + original.G + original.B) / 3;

					return new Pixel(average, average, average);
				},
                new SimpleParametersHandler<EmptyParameters>()
                ));

			window.AddFilter(new TransformFilter(
				"�������� �� �����������",
				size => size,
				(point, size) => new Point(size.Width - point.X - 1, point.Y)
				));

			window.AddFilter(new TransformFilter(
				"��������� ������ �.�.",
				size => new Size(size.Height, size.Width),
				(point, size) => new Point(point.Y, point.X)
				));

			window.AddFilter(new TransformFilter<RotationParameters>(
				"��������� ��������",
				new RotateTransformer(),
				new SimpleParametersHandler<RotationParameters>()
				));

			Application.Run(window);
		}
	}
}
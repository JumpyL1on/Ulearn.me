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
				"Осветление/затемнение",
				(original, parameters) => original * parameters.Coefficient
				));

			window.AddFilter(new PixelFilter<EmptyParameters>(
				"Оттенки серого",
				(original, parameters) =>
				{
					var average = (original.R + original.G + original.B) / 3;

					return new Pixel(average, average, average);
				}
				));

			/*window.AddFilter(new TransformFilter(
				"Отразить по горизонтали",
				size => size,
				(point, size) => new Point(size.Width - point.X - 1, point.Y)
				));

			window.AddFilter(new TransformFilter(
				"Повернуть против ч.с.",
				size => new Size(size.Height, size.Width),
				(point, size) => new Point(point.Y, point.X)
				));*/

			window.AddFilter(new TransformFilter<RotationParameters>(
				"Свободное вращение",
				new RotateTransformer()
				));

			Application.Run(window);
		}
	}
}
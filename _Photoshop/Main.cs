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

			Func<Size, RotationParameters, Size> sizeRotator = (size, parameters) =>
			{
				var angle = Math.PI * parameters.Angle / 180;

				return new Size(
					(int)(size.Width * Math.Abs(Math.Cos(angle)) + size.Height * Math.Abs(Math.Sin(angle))),
					(int)(size.Height * Math.Abs(Math.Cos(angle)) + size.Width * Math.Abs(Math.Sin(angle)))
					);
			};

			window.AddFilter(new TransformFilter<RotationParameters>(
				"Свободное вращение",
				(size, parameters) => sizeRotator(size, parameters),
				(point, size, parameters) =>
                {
                    var newSize = sizeRotator(size, parameters);
                    var angle = Math.PI * parameters.Angle / 180;

                    point = new Point(point.X - newSize.Width / 2, point.Y - newSize.Height / 2);

                    var x = size.Width / 2 + (int)(point.X * Math.Cos(angle) + point.Y * Math.Sin(angle));
                    var y = size.Height / 2 + (int)(-point.X * Math.Sin(angle) + point.Y * Math.Cos(angle));

                    if (x < 0 || x >= size.Width || y < 0 || y >= size.Height)
                    {
                        return null;
                    }

                    return new Point(x, y);
                }
                ));

			Application.Run(window);
		}
	}
}
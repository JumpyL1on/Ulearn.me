using System;
using System.Drawing;

namespace MyPhotoshop.Transformers
{
    public class RotateTransformer : ITransformer<RotationParameters>
    {
        public Size OriginalSize { get; private set; }
        public Size ResultSize { get; private set; }
        private double angle;

        public void Prepare(Size size, RotationParameters parameters)
        {
            OriginalSize = size;
            angle = Math.PI * parameters.Angle / 180;

            ResultSize = new Size(
                (int)(size.Width * Math.Abs(Math.Cos(angle)) + size.Height * Math.Abs(Math.Sin(angle))),
                (int)(size.Height * Math.Abs(Math.Cos(angle)) + size.Width * Math.Abs(Math.Sin(angle)))
                );
        }

        public Point? MapPoint(Point newPoint)
        {
            var point = new Point(newPoint.X - ResultSize.Width / 2, newPoint.Y - ResultSize.Height / 2);

            var x = OriginalSize.Width / 2 + (int)(point.X * Math.Cos(angle) + point.Y * Math.Sin(angle));
            var y = OriginalSize.Height / 2 + (int)(-point.X * Math.Sin(angle) + point.Y * Math.Cos(angle));

            if (x < 0 || x >= OriginalSize.Width || y < 0 || y >= OriginalSize.Height)
            {
                return null;
            }

            return new Point(x, y);
        }
    }
}
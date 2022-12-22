using System;
using System.Drawing;

namespace MyPhotoshop
{
    public class TransformFilter : ParametrizedFilter<EmptyParameters>
    {
        private readonly string filterName;
        private readonly Func<Size, Size> sizeTransform;
        private readonly Func<Point, Size, Point> pointTransform;

        public TransformFilter(
            string filterName,
            Func<Size, Size> sizeTransform,
            Func<Point, Size, Point> pointTransform)
        {
            this.filterName = filterName;
            this.sizeTransform= sizeTransform;
            this.pointTransform= pointTransform;
        }

        public override Photo Process(Photo original, EmptyParameters parameters)
        {
            var size = sizeTransform(new Size(original.Width, original.Height));

            var result = new Photo(size.Width, size.Height);

            for (var x = 0; x < original.Width; x++)
            {
                for (var y = 0; y < original.Height; y++)
                {
                    var point = pointTransform(new Point(x, y), size);

                    result[point.X, point.Y] = original[x, y];
                }
            }

            return result;
        }

        public override string ToString()
        {
            return filterName;
        }
    }
}
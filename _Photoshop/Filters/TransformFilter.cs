using System;
using System.Drawing;

namespace MyPhotoshop
{
    public class TransformFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new()
    {
        private readonly string filterName;
        private readonly Func<Size, TParameters, Size> sizeTransform;
        private readonly Func<Point, Size, TParameters, Point?> pointTransform;

        public TransformFilter(
            string filterName,
            Func<Size, TParameters, Size> sizeTransform,
            Func<Point, Size, TParameters, Point?> pointTransform)
        {
            this.filterName = filterName;
            this.sizeTransform= sizeTransform;
            this.pointTransform= pointTransform;
        }

        public override Photo Process(Photo original, TParameters parameters)
        {
            var size = sizeTransform(new Size(original.Width, original.Height), parameters);

            var result = new Photo(size.Width, size.Height);

            for (var x = 0; x < size.Width; x++)
            {
                for (var y = 0; y < size.Height; y++)
                {
                    var point = pointTransform(new Point(x, y), new Size(original.Width, original.Height), parameters);

                    if (point != null)
                    {
                        result[x, y] = original[point.Value.X, point.Value.Y];
                    }
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
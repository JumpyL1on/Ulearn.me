using System;
using System.Drawing;

namespace MyPhotoshop.Transformers
{
    public class FreeTransformer : ITransformer<EmptyParameters>
    {
        public Size ResultSize { get; private set; }
        private readonly Func<Size, Size> _sizeTransform;
        private readonly Func<Point, Size, Point> _pointTransform;
        private Size oldSize;

        public FreeTransformer(
            Func<Size, Size> sizeTransform,
            Func<Point, Size, Point> pointTransform)
        {
            _sizeTransform = sizeTransform;
            _pointTransform = pointTransform;
        }

        public Point? MapPoint(Point newPoint)
        {
            return _pointTransform(newPoint, oldSize);
        }

        public void Prepare(Size size, EmptyParameters parameters)
        {
            oldSize = size;

            ResultSize = _sizeTransform(size);
        }
    }
}
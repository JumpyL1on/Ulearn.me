using System.Drawing;
using System;
using MyPhotoshop.Transformers;

namespace MyPhotoshop
{
    public class TransformFilter : TransformFilter<EmptyParameters>
    {
        public TransformFilter(
            string filterName,
            ITransformer<EmptyParameters> transformer,
            IParametersHandler<EmptyParameters> handler) : base(filterName, transformer, handler)
        {
        }

        public TransformFilter(
            string filterName,
            Func<Size, Size> sizeTransform,
            Func<Point, Size, Point> pointTransform) : this(
                filterName,
                new FreeTransformer(sizeTransform, pointTransform),
                new SimpleParametersHandler<EmptyParameters>())
        {
        }
    }
}
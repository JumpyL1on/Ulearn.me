using System.Drawing;

namespace MyPhotoshop
{
    public class TransformFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new()
    {
        private readonly string filterName;
        private readonly ITransformer<TParameters> transformer;

        public TransformFilter(
            string filterName,
            ITransformer<TParameters> transformer)
        {
            this.filterName = filterName;
            this.transformer = transformer;
        }

        public override Photo Process(Photo original, TParameters parameters)
        {
            transformer.Prepare(new Size(original.Width, original.Height), parameters);

            var result = new Photo(transformer.ResultSize.Width, transformer.ResultSize.Height);

            for (var x = 0; x < result.Width; x++)
            {
                for (var y = 0; y < result.Height; y++)
                {
                    var point = transformer.MapPoint(new Point(x, y));

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
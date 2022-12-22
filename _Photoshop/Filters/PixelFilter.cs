namespace MyPhotoshop
{
    public abstract class PixelFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new()
    {
        public override Photo Process(Photo original, TParameters parameters)
        {
            var result = new Photo(original.Width, original.Height);

            for (var x = 0; x < original.Width; x++)
            {
                for (var y = 0; y < original.Height; y++)
                {
                    result[x, y] = ProcessPixel(original[x, y], parameters);
                }
            }

            return result;
        }

        protected abstract Pixel ProcessPixel(Pixel original, TParameters parameters);
    }
}
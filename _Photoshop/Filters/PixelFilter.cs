namespace MyPhotoshop
{
    public abstract class PixelFilter : ParametrizedFilter
    {
        public PixelFilter(IParameters parameters) : base(parameters) { }

        public override Photo Process(Photo original, IParameters parameters)
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

        protected abstract Pixel ProcessPixel(Pixel original, IParameters parameters);
    }
}
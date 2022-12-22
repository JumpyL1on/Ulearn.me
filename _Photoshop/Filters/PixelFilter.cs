using System;

namespace MyPhotoshop
{
    public class PixelFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new()
    {
        private readonly string filterName;
        private readonly Func<Pixel, TParameters, Pixel> func;

        public PixelFilter(string filterName, Func<Pixel, TParameters, Pixel> func)
        {
            this.filterName = filterName;
            this.func = func;
        }

        public override Photo Process(Photo original, TParameters parameters)
        {
            var result = new Photo(original.Width, original.Height);

            for (var x = 0; x < original.Width; x++)
            {
                for (var y = 0; y < original.Height; y++)
                {
                    result[x, y] = func(original[x, y], parameters);
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
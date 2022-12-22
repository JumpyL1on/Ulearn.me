namespace MyPhotoshop
{
    public class GrayscaleFilter : IFilter
    {
        public ParameterInfo[] GetParameters()
        {
            return new ParameterInfo[0];
        }

        public Photo Process(Photo original, double[] parameters)
        {
            var result = new Photo(original.Width, original.Height);

            for (var x = 0; x < original.Width; x++)
            {
                for (var y = 0; y < original.Height; y++)
                {
                    result[x, y] = ProcessPixel(original[x, y], parameters)
                }
            }

            return result;
        }

        public override string ToString()
        {
            return "Оттенки серого";
        }

        private Pixel ProcessPixel(Pixel original, double[] parameters)
        {
            var average = (original.R + original.G + original.B) / 3;

            return new Pixel(average, average, average);
        }
    }
}
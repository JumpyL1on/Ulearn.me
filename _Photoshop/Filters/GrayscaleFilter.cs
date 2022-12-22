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
                    var pixel = original[x, y];

                    var average = (pixel.R + pixel.G + pixel.B) / 3;

                    result[x, y] = new Pixel(average, average, average);
                }
            }

            return result;
        }

        public override string ToString()
        {
            return "Оттенки серого";
        }
    }
}
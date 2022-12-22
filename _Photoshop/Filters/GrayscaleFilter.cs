namespace MyPhotoshop
{
    public class GrayscaleFilter : PixelFilter
    {
        public override ParameterInfo[] GetParameters()
        {
            return new ParameterInfo[0];
        }

        public override string ToString()
        {
            return "Оттенки серого";
        }

        protected override Pixel ProcessPixel(Pixel original, double[] parameters)
        {
            var average = (original.R + original.G + original.B) / 3;

            return new Pixel(average, average, average);
        }
    }
}
namespace MyPhotoshop
{
    public class GrayscaleFilter : PixelFilter
    {
        public GrayscaleFilter(IParameters parameters) : base(parameters)
        {
        }

        public override string ToString()
        {
            return "Оттенки серого";
        }

        protected override Pixel ProcessPixel(Pixel original, IParameters parameters)
        {
            var average = (original.R + original.G + original.B) / 3;

            return new Pixel(average, average, average);
        }
    }
}
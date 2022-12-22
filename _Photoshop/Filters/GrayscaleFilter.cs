namespace MyPhotoshop
{
    public class GrayscaleFilter : PixelFilter<EmptyParameters>
    {
        public override string ToString()
        {
            return "Оттенки серого";
        }

        protected override Pixel ProcessPixel(Pixel original, EmptyParameters parameters)
        {
            var average = (original.R + original.G + original.B) / 3;

            return new Pixel(average, average, average);
        }
    }
}
namespace MyPhotoshop
{
    public class LighteningFilter : PixelFilter
    {
        public LighteningFilter(IParameters parameters) : base(parameters)
        {
        }

        public override string ToString()
        {
            return "Осветление/затемнение";
        }

        protected override Pixel ProcessPixel(Pixel original, IParameters parameters)
        {
            return original * (parameters as LighteningParameters).Coefficient;
        }
    }
}
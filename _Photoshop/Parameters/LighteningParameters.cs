namespace MyPhotoshop
{
    public class LighteningParameters : IParameters
    {
        [ParameterInfo("Коэффициент", 1, 0, 10, 0.3)]
        public double Coefficient { get; private set; }
    }
}
namespace MyPhotoshop
{
    public class RotationParameters : IParameters
    {
        [ParameterInfo("Угол", 0, 0, 360, 15)]
        public double Angle { get; private set; }
    }
}
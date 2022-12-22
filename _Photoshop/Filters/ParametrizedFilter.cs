namespace MyPhotoshop
{
    public abstract class ParametrizedFilter : IFilter
    {
        private readonly IParameters parameters;

        public ParametrizedFilter(IParameters parameters)
        {
            this.parameters = parameters;
        }

        public ParameterInfo[] GetParameters()
        {
            return parameters.GetDescription();
        }

        public Photo Process(Photo original, double[] values)
        {
            parameters.SetValues(values);

            return Process(original, parameters);
        }

        public abstract Photo Process(Photo original, IParameters parameters);
    }
}
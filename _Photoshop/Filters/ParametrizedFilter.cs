namespace MyPhotoshop
{
    public abstract class ParametrizedFilter<TParameters> : IFilter
        where TParameters :IParameters, new()
    {
        private readonly IParametersHandler<TParameters> _parametersHandler;

        protected ParametrizedFilter(IParametersHandler<TParameters> parametersHandler)
        {
            _parametersHandler = parametersHandler;
        }

        public ParameterInfo[] GetParameters()
        {
            return _parametersHandler.GetDescription();
        }

        public Photo Process(Photo original, double[] values)
        {
            var parameters = _parametersHandler.CreateParameters(values);

            parameters.SetValues(values);

            return Process(original, parameters);
        }

        public abstract Photo Process(Photo original, TParameters parameters);
    }
}
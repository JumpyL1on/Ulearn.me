using System.Linq;
using System.Reflection;

namespace MyPhotoshop
{
    public class SimpleParametersHandler<TParameters> : IParametersHandler<TParameters>
        where TParameters : IParameters, new()
    {
        private readonly PropertyInfo[] _properties;
        private readonly ParameterInfo[] _parameters;

        public SimpleParametersHandler()
        {
            _properties = typeof(TParameters)
                .GetProperties()
                .Where(property => property.GetCustomAttribute<ParameterInfo>() != null)
                .ToArray();

            _parameters = _properties
                .Select(property => property.GetCustomAttribute<ParameterInfo>())
                .ToArray();
        }

        public TParameters CreateParameters(double[] values)
        {
            var parameters = new TParameters();

            for (var i = 0; i < values.Length; i++)
            {
                _properties[i].SetValue(parameters, values[i]);
            }

            return parameters;
        }

        public ParameterInfo[] GetDescription()
        {
            return _parameters;
        }
    }
}
using System.Linq;
using System.Reflection;

namespace MyPhotoshop
{
    public class SimpleParametersHandler<TParameters> : IParametersHandler<TParameters>
        where TParameters : IParameters, new()
    {
        public TParameters CreateParameters(double[] values)
        {
            var parameters = new TParameters();

            var type = parameters.GetType();

            var i = 0;

            foreach (var property in type
                .GetProperties()
                .Where(property => property.GetCustomAttribute<ParameterInfo>() != null))
            {
                if (values.Length == i)
                {
                    break;
                }

                property.SetValue(parameters, values[i]);

                i++;
            }

            return parameters;
        }

        public ParameterInfo[] GetDescription()
        {
            return typeof(TParameters)
                .GetProperties()
                .Select(property => property.GetCustomAttribute<ParameterInfo>())
                .Where(attribute => attribute != null)
                .ToArray();
        }
    }
}
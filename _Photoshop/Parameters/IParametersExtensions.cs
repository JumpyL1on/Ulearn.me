using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyPhotoshop
{
    public static class IParametersExtensions
    {
        public static ParameterInfo[] GetDescription(this IParameters parameters)
        {
            return parameters
                .GetType()
                .GetProperties()
                .Select(property => property.GetCustomAttribute<ParameterInfo>())
                .Where(attribute => attribute != null)
                .ToArray();
        }

        public static void SetValues(this IParameters parameters, double[] values)
        {
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
        }
    }
}
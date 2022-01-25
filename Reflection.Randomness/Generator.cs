using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflection.Randomness
{
    public class FromDistributionAttribute : Attribute
    {
        public NormalDistribution NormalDistribution { get; }
        public ExponentialDistribution ExponentialDistribution { get; }

        public FromDistributionAttribute(Type type, params object[] numbers)
        {
            switch (numbers.Length)
            {
                case 0:
                    NormalDistribution = type
                        .GetConstructors()[0]
                        .Invoke(numbers) as NormalDistribution;
                    break;
                case 1:
                    ExponentialDistribution = type
                        .GetConstructors()[0]
                        .Invoke(numbers) as ExponentialDistribution;
                    break;
                case 2:
                    NormalDistribution = type
                        .GetConstructors()[1]
                        .Invoke(numbers) as NormalDistribution;
                    break;
                default:
                    throw new ArgumentException(type.Name);
            }
        }
    }

    public class AttributeSetter<T> where T : class, new()
    {
        private string PropertyToSetName { get; }

        public AttributeSetter(string propertyToSetName)
        {
            PropertyToSetName = propertyToSetName;
        }

        public Generator<T> Set(object distribution)
        {
            Generator<T>.Dic[PropertyToSetName] = distribution;
            return new Generator<T>();
        }
    }

    public class Generator<T> where T : class, new()
    {
        // key - propertyName, value - normalDistribution or exponentialDistribution
        public static Dictionary<string, object> Dic { get; } = new Dictionary<string, object>();
        private static Type GeneratorType { get; } = typeof(T);
        private static Type AttributeType { get; } = typeof(FromDistributionAttribute);
        private static Random Rnd { get; } = new Random(DateTime.Now.GetHashCode());

        public AttributeSetter<T> For(Func<T, double> property)
        {
            var generator = GeneratorType
                .GetConstructors()
                .First()
                .Invoke(new object[0]);
            TestGenerate(generator);
            var propertyToSet = GeneratorType
                .GetProperties()
                .FirstOrDefault(x => (double)x.GetValue(generator) == property(generator as T));
            if (propertyToSet == null)
                throw new ArgumentException();
            return new AttributeSetter<T>(propertyToSet.Name);
        }

        public T Generate(Random rnd)
        {
            var generator = GeneratorType
                .GetConstructors()
                .First()
                .Invoke(new object[0]);
            var properties = GeneratorType.GetProperties();
            var propertiesWithoutChangedAttributes = properties.Where(x => !Dic.ContainsKey(x.Name));
            GeneratePropertiesWithoutChangedAttributes(generator, propertiesWithoutChangedAttributes, rnd);
            var propertiesWithChangedAttributes = properties.Where(x => Dic.ContainsKey(x.Name));
            GeneratePropertiesWithChangedAttributes(generator, propertiesWithChangedAttributes, rnd);
            Dic.Clear();
            return generator as T;
        }

        private void GeneratePropertiesWithoutChangedAttributes(
            object generator,
            IEnumerable<PropertyInfo> propertiesWithoutChangedAttributes,
            Random rnd)
        {
            foreach (var property in propertiesWithoutChangedAttributes)
            {
                if (property.GetCustomAttribute(AttributeType) is FromDistributionAttribute attribute)
                {
                    if (attribute.NormalDistribution != null)
                        property.SetValue(generator, attribute.NormalDistribution.Generate(rnd));
                    if (attribute.ExponentialDistribution != null)
                        property.SetValue(generator, attribute.ExponentialDistribution.Generate(rnd));
                }
            }
        }

        private void GeneratePropertiesWithChangedAttributes(
            object generator,
            IEnumerable<PropertyInfo> propertiesWithChangedAttributes,
            Random rnd)
        {
            foreach (var property in propertiesWithChangedAttributes)
            {
                var distribution = Dic[property.Name];
                if (distribution is NormalDistribution normalDistribution)
                    property.SetValue(generator, normalDistribution.Generate(rnd));
                if (distribution is ExponentialDistribution exponentialDistribution)
                    property.SetValue(generator, exponentialDistribution.Generate(rnd));
            }
        }

        private void TestGenerate(object generator)
        {
            foreach (var property in GeneratorType.GetProperties())
            {
                if (property.GetCustomAttribute(AttributeType) is FromDistributionAttribute attribute)
                {
                    if (attribute.NormalDistribution != null)
                        property.SetValue(generator, attribute.NormalDistribution.Generate(Rnd));
                    if (attribute.ExponentialDistribution != null)
                        property.SetValue(generator, attribute.ExponentialDistribution.Generate(Rnd));
                }
            }
        }
    }
}

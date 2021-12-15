using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ddd.Infrastructure
{
	public class ValueType<T>
	{
        public static int[] PrimeNumbers { get; } = new[] { 211, 223, 227, 229, 233 };
        public bool Equals(T obj)
        {
            if (ReferenceEquals(obj, null) && ReferenceEquals(this, null)) return false;
            if (ReferenceEquals(obj, null)) return false;
            if (GetType() != obj.GetType()) return false;
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (var i = 0; i < properties.Length; i++)
            {
                var thisValue = properties[i].GetValue(this);
                var objValue = properties[i].GetValue(obj);
                if (thisValue == null && objValue == null)
                    continue;
                if (thisValue == null || objValue == null)
                    return false;
                if (!thisValue.Equals(objValue))
                    return false;
            }
            return true;
        }
		
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null) && ReferenceEquals(this, null)) return false;
            if (ReferenceEquals(obj, null)) return false;
            if (GetType() != obj.GetType()) return false;
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (var i = 0; i < properties.Length; i++)
            {
                var thisValue = properties[i].GetValue(this);
                var objValue = properties[i].GetValue(obj);
                if (thisValue == null && objValue == null)
                    continue;
                if (thisValue == null || objValue == null)
                    return false;
                if (!thisValue.Equals(objValue))
                    return false;
            }
            return true;
        }
		
        public override int GetHashCode()
        {
            var hash = 0;
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            unchecked
            {
                for (var i = 0; i < properties.Length; i++)
                    hash += properties[i].GetValue(this).GetHashCode() * PrimeNumbers[i];
                return hash;
            }
        }
		
        public override string ToString()
        {
            var str = new StringBuilder($"{GetType().Name}(");
            var dic = new Dictionary<string, object>();
            foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                dic[property.Name] = property.GetValue(this);
            foreach (var e in dic.OrderBy(keyValuePair => keyValuePair.Key))
                str.Append($"{e.Key}: {e.Value}; ");
            return str.Remove(str.Length - 2, 2).Append(")").ToString();
        }
    }
}
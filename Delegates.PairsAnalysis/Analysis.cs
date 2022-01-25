using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.PairsAnalysis
{
    public static class LinqExtensions
    {
        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException();
            var previous = enumerator.Current;
            if (!enumerator.MoveNext())
                throw new ArgumentException();
            var current = enumerator.Current;
            yield return Tuple.Create(previous, current);
            while(enumerator.MoveNext())
            {
                previous = current;
                current = enumerator.Current;
                yield return Tuple.Create(previous, current);
            }
        }
		
        public static int MaxIndex(this IEnumerable<DateTime> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException();
            var previous = enumerator.Current;
            if (!enumerator.MoveNext())
                throw new ArgumentException();
            var current = enumerator.Current;
            var maxPeriod = (current - previous).TotalSeconds;
            var i = 0;
			var bestIndex = i;
            while (enumerator.MoveNext())
            {
				i++;
                previous = current;
                current = enumerator.Current;
                var period = (current - previous).TotalSeconds;
                if (period >= maxPeriod)
                {
                    bestIndex = i;
                    maxPeriod = period;
                }
            }
            return bestIndex;
        }
		
        public static int MaxIndex(this IEnumerable<int> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException();
            var previous = enumerator.Current;
            if (!enumerator.MoveNext())
                throw new ArgumentException();
            var current = enumerator.Current;
            var maxPeriod = current - previous;
            var i = 1;
			var bestIndex = i;
            while(enumerator.MoveNext())
            {
				i++;
                previous = current;
                current = enumerator.Current;
                var period = current - previous;
                if (period >= maxPeriod)
                {
                    bestIndex = i;
                    maxPeriod = period;
                }
            }
            return bestIndex;
        }
    }
	
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data)
        {
            return data.MaxIndex();
        }
		
        public static double FindAverageRelativeDifference(params double[] data)
        {
            var sum = 0d;
            var count = 0;
            foreach (var e in data.Pairs())
            {
                sum += (e.Item2 - e.Item1) / e.Item1;
                count++;
            }
            return sum / count;
        }
    }
}

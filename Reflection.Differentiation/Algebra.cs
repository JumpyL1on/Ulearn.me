using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Reflection.Differentiation
{
    public class Algebra
    {
        private static readonly Expression D0 = Expression.Constant(0d);
        private static readonly Expression D1 = Expression.Constant(1d);

        public static Expression<Func<double, double>> Differentiate(
            Expression<Func<double, double>> function)
        {
            var p = function.Parameters[0];
            var f = function.ToString();
            if (f.Contains("Sin") || f.Contains("Cos"))
            {
                var isSin = f.Contains("Sin");
                f = f.Remove(0, 8).Replace(" ", "").Replace("(", "").Replace(")", "");
                return Expression.Lambda<Func<double, double>>(SinOrCosCase(f, p, isSin), p);
            }
            else
            {
                f = f.Remove(0, 5).Replace(" ", "").Replace("(", "").Replace(")", "");
                return Expression.Lambda<Func<double, double>>(SimpleCase(f, p), p);
            }
        }

        private static Expression SinOrCosCase(string f, ParameterExpression p, bool isSin)
        {
            var left = D0;
            foreach (var e in f.Split('+'))
            {
                var right = D1;
                e.Split('*').ToList().ForEach(value => right = value == p.Name
                    ? Expression.Multiply(p, right)
                    : Expression.Multiply(Expression.Constant(double.Parse(value)), right));
                left = Expression.Add(left, right);
            }
            if (isSin)
            {
                var cos = typeof(Math).GetMethod("Cos");
                return Expression.Multiply(SimpleCase(f, p), Expression.Call(cos, left));
            }
            var sin = typeof(Math).GetMethod("Sin");
            return Expression.Multiply(SimpleCase(f, p), Expression.Negate(Expression.Call(sin, left)));
        }

        private static Expression SimpleCase(string f, ParameterExpression p)
        {
            var expressions = new List<Expression>();
            foreach (var e in f.Split('+'))
            {
                var fs = new List<Expression>();
                var dfs = new List<Expression>();
                foreach (var value in e.Split('*'))
                {
                    var flag = value == p.Name;
                    if (flag) fs.Add(p);
                    else fs.Add(Expression.Constant(double.Parse(value)));
                    dfs.Add(flag ? D1 : D0);
                }
                for (var i = 0; i < fs.Count; i++)
                {
                    var left = dfs[i];
                    for (var j = 0; j < dfs.Count; j++)
                        if (j != i) left = Expression.Multiply(left, fs[j]);
                    expressions.Add(left);
                }
            }
            var result = D0;
            expressions.ForEach(e => result = Expression.Add(result, e));
            return result;
        }
    }
}
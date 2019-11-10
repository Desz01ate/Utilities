using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Utilities.Shared
{
    internal static class ExpressionExtension
    {
        internal static Func<TClass, object> CompileGetter<TClass>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TClass));
            var body = Expression.Convert(Expression.Property(param, propertyName), typeof(object));
            return Expression.Lambda<Func<TClass, object>>(body, param).Compile();
        }
    }
}

using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Utilities.Classes;
using System.Linq.Expressions;
using System.Data;

namespace Utilities.Shared
{
    public static partial class GenericExtension
    {
        /// <summary>
        /// Verify if the object is a generic list.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        internal static bool IsGenericList(this object o)
        {
            var type = o.GetType();
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
        }

        /// <summary>
        /// Internally convert object to target type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static dynamic Cast(dynamic obj, Type targetType)
        {
            return Convert.ChangeType(obj, targetType);
        }
    }
    public static partial class GenericExtension
    {
        /// <summary>
        /// Compiled version of PropertyInfo.GetValue().
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static PropertyGetterInfo<TSource>[] CompileGetter<TSource>()
        {
            var type = typeof(TSource);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            return CompileGetter<TSource>(properties);
        }
        /// <summary>
        /// Compiled version of PropertyInfo.GetValue().
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static PropertyGetterInfo<TSource>[] CompileGetter<TSource>(PropertyInfo[] properties)
        {
            if (properties is null) return null;
            var funcs = new PropertyGetterInfo<TSource>[properties.Length];
            for (var idx = 0; idx < properties.Length; idx++)
            {
                var property = properties[idx];
                var targetExpr = Expression.Parameter(typeof(TSource), "target");
                var value = Expression.Convert(Expression.Property(targetExpr, property), typeof(object));
                var func = Expression.Lambda<Func<TSource, object>>(value, targetExpr).Compile();
                var psi = new PropertyGetterInfo<TSource>(property, func);
                funcs[idx] = psi;
            }
            return funcs;
        }
        /// <summary>
        /// Compiled version of PropertyInfo.GetValue().
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static PropertyGetterInfo<TSource> CompileGetter<TSource>(PropertyInfo property)
        {
            if (property is null) return null;
            var targetExpr = Expression.Parameter(typeof(TSource), "target");
            var value = Expression.Convert(Expression.Property(targetExpr, property), typeof(object));
            return new PropertyGetterInfo<TSource>(property, Expression.Lambda<Func<TSource, object>>(value, targetExpr).Compile());
        }
        /// <summary>
        /// Compiled version of PropertyInfo.SetValue().
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static PropertySetterInfo<TSource>[] CompileSetter<TSource>()
        {
            var type = typeof(TSource);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            return CompileSetter<TSource>(properties);
        }
        /// <summary>
        /// Compiled version of PropertyInfo.SetValue().
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static PropertySetterInfo<TSource>[] CompileSetter<TSource>(PropertyInfo[] properties)
        {
            if (properties is null) return null;

            var actions = new PropertySetterInfo<TSource>[properties.Length];
            for (var idx = 0; idx < properties.Length; idx++)
            {
                var property = properties[idx];
                //target variable (provide on compile).
                var variableExpr = Expression.Parameter(typeof(TSource), "target");
                //target property from above variable.
                var propertyExpr = Expression.Property(variableExpr, property.Name);
                //value to assign to above property.
                var valueExpr = Expression.Parameter(typeof(object), "value");

                var assignExpr = Expression.Assign(propertyExpr, Expression.Convert(valueExpr, property.PropertyType));
                //var assignNullExpr = Expression.Assign(propertyExpr, Expression.Default(property.PropertyType));
                ////if valueExpr equal to DBNull.Value then assign default of property type else assign unboxed valueExpr.
                //var dbnullFilterExpr = Expression.Condition(
                //                            Expression.Equal(valueExpr, Expression.Constant(DBNull.Value)),
                //                            assignNullExpr,
                //                            assignExpr
                //                       );

                //compile expression to  : void action(variable,value) with body as dbnullFilterExpr.
                var action = Expression.Lambda<Action<TSource, object>>(assignExpr, variableExpr, valueExpr);
                var psi = new PropertySetterInfo<TSource>(property, idx, action.Compile());
                actions[idx] = psi;
            }
            return actions;
        }
    }
}
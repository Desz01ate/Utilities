﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Utilities.Shared;
using Utilities.SQL.Interfaces;

namespace Utilities.SQL.Translator
{
    /// <summary>
    /// alternative to reflection builder with MUCH better on performance, implementation taken from https://stackoverflow.com/questions/19841120/generic-dbdatareader-to-listt-mapping/19845980#19845980
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Converter<T> : IDataMapper<T> where T : new()
    {
        /// <summary>
        /// Underlying object SHOULD always be ConcurrentDictionary of Type => Func of IDataReader return T
        /// </summary>
        private static ConcurrentDictionary<Type, object> _internalCache = new ConcurrentDictionary<Type, object>();
        readonly Func<IDataReader, T> _converter;
        readonly IDataReader dataReader;

        private Func<IDataReader, T> GetMapFunc()
        {
            var exps = new List<Expression>();

            var paramExp = Expression.Parameter(typeof(IDataRecord), "datareader");

            var targetExp = Expression.Variable(typeof(T));
            //var target = new T();
            exps.Add(Expression.Assign(targetExp, Expression.New(targetExp.Type)));

            //does int based lookup
            var indexerInfo = typeof(IDataRecord).GetProperty("Item", new[] { typeof(int) });

            var columnNames = Enumerable.Range(0, dataReader.FieldCount)
                                        .Select(i => new { i, name = dataReader.GetName(i) });
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var column in columnNames)
            {

                var property = AttributeExtension.GetUnderlyingPropertyByName(properties, column.name);
                if (property == null)
                    continue;

                var readerIndex = Expression.Constant(column.i);
                // equivalent to datareader[(int)readerIndex] where datareader is incoming parameter.
                var valueExpr =
                    Expression.MakeIndex(
                    paramExp, indexerInfo, new[] { readerIndex });

                var actualType = dataReader.GetFieldType(column.i);
                Expression safeCastExpression;
                //if property type in model doesn't match the underlying type in SQL, we first convert into actual SQL type.
                if (actualType != property.PropertyType)
                {
                    safeCastExpression = Expression.Convert(valueExpr, actualType);
                }
                //otherwise we do nothing.
                else
                {
                    safeCastExpression = valueExpr;
                }
                var isReaderDbNull = Expression.Call(paramExp, "IsDBNull", null, readerIndex);
                var propertyExpression = Expression.Property(targetExp, property);
                /*
                 if(datareader.IsDBNull((int)readerIndex){
                    target.property = default;
                 }else{
                    target.property = (castType)value;
                 }
                 */
                var assignmentBlock = Expression.Condition(
                                            Expression.IsTrue(isReaderDbNull),
                                                Expression.Assign(propertyExpression, Expression.Default(property.PropertyType)),
                                                Expression.Assign(propertyExpression, Expression.Convert(safeCastExpression, property.PropertyType)
                                             )
                                     );



                exps.Add(assignmentBlock);
            }
            //return target;
            exps.Add(targetExp);
            var func = Expression.Lambda<Func<IDataReader, T>>(
                Expression.Block(new[] { targetExp }, exps), paramExp);
            return func.Compile();
        }

        public Converter(IDataReader dataReader)
        {
            this.dataReader = dataReader;
            if (_internalCache.TryGetValue(typeof(T), out var res))
            {
                _converter = (Func<IDataReader, T>)res;
            }
            else
            {
                var func = GetMapFunc();
                _converter = func;
                _internalCache.TryAdd(typeof(T), func);
            }

        }
        public T GenerateObject()
        {
            return _converter(dataReader);
        }
        public IEnumerable<T> GenerateObjects()
        {
            while (dataReader.Read())
            {
                yield return _converter(dataReader);
            }
        }
    }
}

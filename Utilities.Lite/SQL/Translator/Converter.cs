using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Utilities.Shared;

namespace Utilities.SQL.Translator
{
    /// <summary>
    /// alternative to reflection builder with MUCH better on performance, implementation taken from https://stackoverflow.com/questions/19841120/generic-dbdatareader-to-listt-mapping/19845980#19845980
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Converter<T>
    {
        readonly Func<IDataReader, T> _converter;
        readonly IDataReader dataReader;

        private Func<IDataReader, T> GetMapFunc()
        {
            var exps = new List<Expression>();

            var paramExp = Expression.Parameter(typeof(IDataRecord), "datareader");

            var targetExp = Expression.Variable(typeof(T));
            exps.Add(Expression.Assign(targetExp, Expression.New(targetExp.Type)));

            //does int based lookup
            var indexerInfo = typeof(IDataRecord).GetProperty("Item", new[] { typeof(int) });

            var columnNames = System.Linq.Enumerable.Range(0, dataReader.FieldCount)
                                        .Select(i => new { i, name = dataReader.GetName(i) });
            foreach (var column in columnNames)
            {

                var property = targetExp.Type.GetUnderlyingPropertyByName(column.name);
                if (property == null)
                    continue;

                var columnNameExp = Expression.Constant(column.i);
                var propertyExp = Expression.MakeIndex(
                    paramExp, indexerInfo, new[] { columnNameExp });

                var actualType = dataReader.GetFieldType(column.i);
                Expression safeCastExpression;
                //if property type in model doesn't match the underlying type in SQL, we first convert into actual SQL type.
                if (actualType != property.PropertyType)
                {
                    safeCastExpression = Expression.Convert(propertyExp, actualType);
                }
                //otherwise we do nothing.
                else
                {
                    safeCastExpression = propertyExp;
                }
                var propertyExpression = Expression.Property(targetExp, property);
                var ifDbNull = Expression.Assign(propertyExpression, Expression.Default(property.PropertyType));
                var ifNotDbNull = Expression.Assign(propertyExpression, Expression.Convert(safeCastExpression, property.PropertyType));
                var bindExpression = Expression.Condition(
                                            Expression.Equal(propertyExp, Expression.Constant(DBNull.Value)), //if field is null then assign default value to specified field, otherwise assign the real value.
                                            ifDbNull,
                                            ifNotDbNull);

                exps.Add(bindExpression);
            }

            exps.Add(targetExp);
            return Expression.Lambda<Func<IDataReader, T>>(
                Expression.Block(new[] { targetExp }, exps), paramExp).Compile();
        }

        internal Converter(IDataReader dataReader)
        {
            this.dataReader = dataReader;
            _converter = GetMapFunc();

        }
        internal T GenerateObject()
        {
            return _converter(dataReader);
        }
        internal IEnumerable<T> GenerateObjects()
        {
            while (dataReader.Read())
            {
                yield return _converter(dataReader);
            }
        }
    }
}

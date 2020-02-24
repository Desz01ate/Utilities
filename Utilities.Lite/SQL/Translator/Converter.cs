using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Utilities.Interfaces;
using Utilities.Shared;

namespace Utilities.SQL.Translator
{
    /// <summary>
    /// alternative to reflection builder with MUCH better on performance, implementation taken from https://stackoverflow.com/questions/19841120/generic-dbdatareader-to-listt-mapping/19845980#19845980
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Converter<T> : IDataMapper<T> where T : new()
    {
        // ReSharper disable once InconsistentNaming
        private static Func<IDataReader, T> _converter;
        private readonly IDataReader _dataReader;

        private Func<IDataReader, T> CompileMapperFunction()
        {
            var exps = new List<Expression>();

            var dataReaderExprParam = Expression.Parameter(typeof(IDataRecord), "datareader");

            var targetExp = Expression.Variable(typeof(T));
            //var target = new T();
            exps.Add(Expression.Assign(targetExp, Expression.New(targetExp.Type)));

            //does int based lookup
            var indexerInfo = typeof(IDataRecord).GetProperty("Item", new[] { typeof(int) });

            var columnNames = Enumerable.Range(0, _dataReader.FieldCount)
                                        .Select(i => new { index = i, name = _dataReader.GetName(i) });
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var column in columnNames)
            {

                var property = AttributeExtension.GetUnderlyingPropertyByName(properties, column.name);
                if (property == null)
                    continue;

                var readerIndex = Expression.Constant(column.index);
                // equivalent to datareader[(int)readerIndex] where datareader is incoming parameter.
                var valueExpr = Expression.MakeIndex(dataReaderExprParam, indexerInfo, new[] { readerIndex });

                var actualType = _dataReader.GetFieldType(column.index);
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
                var isReaderDbNull = Expression.Call(dataReaderExprParam, "IsDBNull", null, readerIndex);
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
                Expression.Block(new[] { targetExp }, exps), dataReaderExprParam);
            return func.Compile();
        }

        public Converter(IDataReader dataReader)
        {
            this._dataReader = dataReader;
            if (_converter != null) return;
            _converter = CompileMapperFunction();
        }
        public T GenerateObject()
        {
            return _converter(_dataReader);
        }
        public IEnumerable<T> GenerateObjects()
        {
            while (_dataReader.Read())
            {
                yield return _converter(_dataReader);
            }
        }
    }
    internal static class Converter
    {
        internal static IEnumerable<dynamic> Convert(IDataReader reader)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    yield return Shared.DataExtension.RowBuilder(reader);
                }
            }
        }
        internal static IEnumerable<T> Convert<T>(IDataReader reader) where T : class, new()
        {
            using (reader)
            {
                IDataMapper<T> converter = new Converter<T>(reader);
                while (reader.Read())
                {
                    yield return converter.GenerateObject();
                }
            }
        }
        internal static IEnumerable<T1> Convert<T1, T2>(IDataReader reader, Func<T1, T2, T1> map)
where T1 : class, new()
where T2 : class, new()
        {
            using (reader)
            {
                IDataMapper<T1> converter = new Converter<T1>(reader);
                IDataMapper<T2> childConverter = new Converter<T2>(reader);
                while (reader.Read())
                {
                    yield return map(converter.GenerateObject(), childConverter.GenerateObject());
                }
            }
        }
        internal static IEnumerable<T1> Convert<T1, T2, T3>(IDataReader reader, Func<T1, T2, T3, T1> map)
    where T1 : class, new()
    where T2 : class, new()
            where T3 : class, new()
        {
            using (reader)
            {
                IDataMapper<T1> converter = new Converter<T1>(reader);
                IDataMapper<T2> converter2 = new Converter<T2>(reader);
                IDataMapper<T3> converter3 = new Converter<T3>(reader);

                while (reader.Read())
                {
                    yield return map(converter.GenerateObject(), converter2.GenerateObject(), converter3.GenerateObject());
                }
            }
        }
        internal static IEnumerable<T1> Convert<T1, T2, T3, T4>(IDataReader reader, Func<T1, T2, T3, T4, T1> map)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new()
            where T4 : class, new()
        {
            using (reader)
            {
                IDataMapper<T1> converter = new Converter<T1>(reader);
                IDataMapper<T2> converter2 = new Converter<T2>(reader);
                IDataMapper<T3> converter3 = new Converter<T3>(reader);
                IDataMapper<T4> converter4 = new Converter<T4>(reader);
                while (reader.Read())
                {
                    yield return map(converter.GenerateObject(), converter2.GenerateObject(), converter3.GenerateObject(), converter4.GenerateObject());
                }
            }
        }
    }
}

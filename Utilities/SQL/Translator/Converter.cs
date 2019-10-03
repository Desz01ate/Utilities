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
                //var property = targetExp.Type.GetProperty(
                //    column.name,
                //    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var property = targetExp.Type.GetUnderlyingPropertyByName(column.name);
                if (property == null)
                    continue;

                var columnNameExp = Expression.Constant(column.i);
                var propertyExp = Expression.MakeIndex(
                    paramExp, indexerInfo, new[] { columnNameExp });

                var dbnullFilter = Expression.IfThenElse(
                    Expression.TypeIs(propertyExp, typeof(DBNull)),
                    Expression.Assign(
                        Expression.Property(targetExp, property), Expression.Default(property.PropertyType)),
                    Expression.Assign(
                        Expression.Property(targetExp, property), Expression.Convert(propertyExp, property.PropertyType))
                    );
                exps.Add(dbnullFilter);
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

        internal T CreateItemFromRow()
        {
            return _converter(dataReader);
        }
    }
    /// <summary>
    /// alternative to dynamic builder, it characteristic is almost on par with native code so no differences here.
    /// </summary>
    //internal class Converter
    //{
    //    readonly Func<IDataReader, dynamic> _converter;
    //    readonly IDataReader dataReader;

    //    private Func<IDataReader, dynamic> GetMapFunc()
    //    {
    //        var exps = new List<Expression>();

    //        var paramExp = Expression.Parameter(typeof(IDataRecord), "datareader");

    //        var targetExp = Expression.Variable(typeof(ExpandoObject));

    //        var addExp = typeof(IDictionary<string, object>).GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string), typeof(object) }, null);

    //        exps.Add(Expression.Assign(targetExp, Expression.New(targetExp.Type)));

    //        //does int based lookup
    //        var indexerInfo = typeof(IDataRecord).GetProperty("Item", new[] { typeof(int) });

    //        var columnNames = System.Linq.Enumerable.Range(0, dataReader.FieldCount)
    //                                    .Select(i => new { i, name = dataReader.GetName(i) });
    //        foreach (var column in columnNames)
    //        {
    //            var columnNameExp = Expression.Constant(column.i);
    //            var propertyExp = Expression.MakeIndex(
    //                paramExp, indexerInfo, new[] { columnNameExp });

    //            var key = Expression.Constant(column.name);
    //            var assignExp = Expression.Call(targetExp, addExp, key, propertyExp);

    //            exps.Add(assignExp);
    //        }

    //        exps.Add(targetExp);
    //        return Expression.Lambda<Func<IDataReader, dynamic>>(
    //            Expression.Block(new[] { targetExp }, exps), paramExp).Compile();
    //    }

    //    internal Converter(IDataReader dataReader)
    //    {
    //        this.dataReader = dataReader;
    //        _converter = GetMapFunc();
    //    }

    //    internal dynamic CreateItemFromRow()
    //    {
    //        return _converter(dataReader);
    //    }
    //}
}

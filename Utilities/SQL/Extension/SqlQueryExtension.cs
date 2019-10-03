using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Utilities.Enum;
using Utilities.Interfaces;
using Utilities.Shared;
using Utilities.SQL.Translator;

namespace Utilities.SQL.Extension
{
    /// <summary>
    /// Provide extension for SQL generate.
    /// </summary>
    public static class SqlQueryExtension
    {
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top"></param>
        /// <returns></returns>
        public static string SelectQueryGenerate<T, TParameterType>(int? top = null)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var query = string.Format("SELECT {0} * FROM {1}", top.HasValue ? $"TOP({top.Value})" : "", tableName);
            return query;
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<TParameterType> parameters) SelectQueryGenerate<T, TParameterType>(IDatabaseConnectorProperty connector, Expression<Func<T, bool>> predicate, int? top = null)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T, TParameterType>(connector.SQLFunctionConfiguration);
            var translateResult = translator.Translate(predicate);
            var query = string.Format("SELECT {0} * FROM {1} WHERE {2}", top.HasValue ? $"TOP({top.Value})" : "", tableName, translateResult.Expression);
            return (query, translateResult.Parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<TParameterType> parameters) SelectQueryGenerate<T, TParameterType>(object primaryKey)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var parameter = new TParameterType()
            {
                ParameterName = primaryKeyAttribute.Name,
                Value = primaryKey
            };
            return (query, new[] { parameter });
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<TParameterType> parameters) InsertQueryGenerate<T, TParameterType>(T obj)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var kvMapper = Shared.DataExtension.CRUDDataMapping(obj, SqlType.Insert);
            var query = $@"INSERT INTO {tableName}
                              ({string.Join(",", kvMapper.Select(field => field.Key))})
                              VALUES
                              ({string.Join(",", kvMapper.Select(field => $"@{field.Key}"))})";
            var parameters = kvMapper.Select(field => new TParameterType()
            {
                ParameterName = $"@{field.Key}",
                Value = field.Value
            });
            return (query, parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<TParameterType> parameters) InsertQueryGenerate<T, TParameterType>(IEnumerable<T> obj)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var kvMapper = Shared.DataExtension.CRUDDataMapping(obj.First(), SqlType.Insert);
            var query = new StringBuilder($@"INSERT INTO {tableName}({string.Join(",", kvMapper.Select(field => field.Key))}) VALUES");
            var values = new List<string>();
            var parameters = new List<TParameterType>();
            for (var idx = 0; idx < obj.Count(); idx++)
            {
                var o = obj.ElementAt(idx);
                var map = Shared.DataExtension.CRUDDataMapping(o, SqlType.Insert);

                values.Add($"({ string.Join(",", map.Select(field => $"@{field.Key}{idx}"))})");
                parameters.AddRange(map.Select(field => new TParameterType()
                {
                    ParameterName = $"@{field.Key}{idx}",
                    Value = field.Value
                }));
            }
            var joinedValue = string.Join(",", values);
            query.Append(joinedValue);
            return (query.ToString(), parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<TParameterType> parameters) UpdateQueryGenerate<T, TParameterType>(T obj)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();
            var pkValue = primaryKey.GetValue(obj);
            var parameters = Shared.DataExtension.CRUDDataMapping(obj, SqlType.Update);
            parameters.Remove(primaryKey.Name);
            var query = $@"UPDATE {tableName} SET
                               {string.Join(",", parameters.Select(x => $"{x.Key} = @{x.Key}"))}
                                WHERE
                               {primaryKey.Name} = @{primaryKey.Name}";
            var parametersArray = parameters.Select(x => new TParameterType()
            {
                ParameterName = $"@{x.Key}",
                Value = x.Value
            }).ToList();
            parametersArray.Add(new TParameterType() { ParameterName = $"@{primaryKey.Name}", Value = primaryKey.GetValue(obj) });
            return (query, parametersArray);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<TParameterType> parameters) DeleteQueryGenerate<T, TParameterType>(T obj)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();

            var query = $"DELETE FROM {tableName} WHERE {primaryKey.Name} = @{primaryKey.Name}";
            var parameters = new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKey.Name,
                        Value = primaryKey.GetValue(obj)
                    } };
            return (query, parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<TParameterType> parameters) DeleteQueryGenerate<T, TParameterType>(object primaryKey)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"DELETE FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var parameters = new[] {
                    new TParameterType()
                    {
                        ParameterName = primaryKeyAttribute.Name,
                        Value = primaryKey
                    }
                };
            return (query, parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<TParameterType> parameters) DeleteQueryGenerate<T, TParameterType>(IDatabaseConnectorProperty connector, Expression<Func<T, bool>> predicate)
            where T : class, new()
            where TParameterType : DbParameter, new()
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T, TParameterType>(connector.SQLFunctionConfiguration);
            var translateResult = translator.Translate(predicate);
            var query = $@"DELETE FROM {tableName} WHERE {translateResult.Expression}";
            return (query, translateResult.Parameters);
        }
    }
}
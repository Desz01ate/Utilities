using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Utilities.Classes;
using Utilities.Enum;
using Utilities.Interfaces;
using Utilities.Shared;
using Utilities.SQL.Abstracts;
using Utilities.SQL.Translator;

namespace Utilities.SQL.Extension
{

    #region NON-GENERIC
    /// <summary>
    /// Provide extension for SQL generate.
    /// </summary>
    public static partial class SqlQueryExtension
    {
        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top"></param>
        /// <returns></returns>
        public static string SelectQueryGenerate<T>(this DatabaseConnectorBase connector, int? top = null)
            where T : class
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var query = $"SELECT {(top.HasValue ? $"TOP({top.Value})" : "")} * FROM {tableName}";
            return query;
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<DatabaseParameter> parameters) SelectQueryGenerate<T>(this DatabaseConnectorBase connector, Expression<Func<T, bool>> predicate, int? top = null)
            where T : class
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T>(connector.CompatibleFunctionName);
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
        public static (string query, IEnumerable<DatabaseParameter> parameters) SelectQueryGenerate<T>(this DatabaseConnectorBase connector, object primaryKey)
            where T : class
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var parameter = new DatabaseParameter(primaryKeyAttribute.Name, primaryKey);
            return (query, new[] { parameter });
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<DatabaseParameter> parameters) InsertQueryGenerate<T>(this DatabaseConnectorBase connector, T obj)
            where T : class
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var tableName = typeof(T).TableNameAttributeValidate();
            var kvMapper = DataExtension.CRUDDataMapping(obj, SqlType.Insert);
            var query = $@"INSERT INTO {tableName}
                              ({string.Join(",", kvMapper.Select(field => field.Key))})
                              VALUES
                              ({string.Join(",", kvMapper.Select(field => $"@{field.Key}"))})";
            var parameters = kvMapper.Select(field => new DatabaseParameter($"@{field.Key}", field.Value));
            return (query, parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<DatabaseParameter> parameters) InsertQueryGenerate<T>(this DatabaseConnectorBase connector, IEnumerable<T> source)
            where T : class
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var enumerable = source as T[] ?? source.ToArray();
            var parametersMap = DataExtension.CRUDDataMapping(enumerable.First(), SqlType.Insert);

            //values placeholder before append.
            var values = new List<string>();

            var parameters = new List<DatabaseParameter>();
            var query = new StringBuilder($@"INSERT INTO {tableName}");
            query.Append("(");
            query.Append($"{string.Join(",", parametersMap.Select(field => field.Key))}");
            query.AppendLine(")");
            query.AppendLine("VALUES");
            for (var idx = 0; idx < enumerable.Count(); idx++)
            {
                var obj = enumerable.ElementAt(idx);
                var mapper = DataExtension.CRUDDataMapping(obj, SqlType.Insert);
                // ReSharper disable AccessToModifiedClosure
                var currentValueStatement = $"({string.Join(",", mapper.Select(x => $"@{x.Key}{idx}"))})";
                values.Add(currentValueStatement);
                var currentParameters = mapper.Select(x => new DatabaseParameter($"@{x.Key}{idx}", x.Value));
                parameters.AddRange(currentParameters);
                // ReSharper restore AccessToModifiedClosure

            }
            query.AppendLine(string.Join(",", values));
            return (query.ToString(), parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<DatabaseParameter> parameters) UpdateQueryGenerate<T>(this DatabaseConnectorBase connector, T obj)
            where T : class
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();
            var parametersMap = DataExtension.CRUDDataMapping(obj, SqlType.Update);

            var parameters = new List<DatabaseParameter>();


            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"UPDATE {tableName} SET");
            var setter = new List<string>();
            foreach (var parameter in parametersMap)
            {
                //if the parameter is not a primary key, add it to the SET block.
                if (parameter.Key != primaryKey.Name)
                    setter.Add($"{parameter.Key} = @{parameter.Key}");
                parameters.Add(new DatabaseParameter($"@{parameter.Key}", parameter.Value));
            }
            stringBuilder.AppendLine(string.Join(",", setter));
            stringBuilder.AppendLine("WHERE");
            stringBuilder.AppendLine($"{primaryKey.Name} = @{primaryKey.Name}");

            var query = stringBuilder.ToString();

            return (query, parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<DatabaseParameter> parameters) DeleteQueryGenerate<T>(this DatabaseConnectorBase connector, T obj)
            where T : class
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKey = type.PrimaryKeyAttributeValidate();

            var query = $"DELETE FROM {tableName} WHERE {primaryKey.Name} = @{primaryKey.Name}";
            var parameters = new[] {
                    new DatabaseParameter(primaryKey.Name,primaryKey.GetValue(obj))
            };
            return (query, parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<DatabaseParameter> parameters) DeleteQueryGenerate<T>(this DatabaseConnectorBase connector, object primaryKey)
            where T : class
        {
            var type = typeof(T);
            var tableName = type.TableNameAttributeValidate();
            var primaryKeyAttribute = type.PrimaryKeyAttributeValidate();
            var query = $"DELETE FROM {tableName} WHERE {primaryKeyAttribute.Name} = @{primaryKeyAttribute.Name}";
            var parameters = new[] {
                    new DatabaseParameter(primaryKeyAttribute.Name,primaryKey)
                };
            return (query, parameters);
        }

        /// <summary>
        /// Generate SQL query with sql parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static (string query, IEnumerable<DatabaseParameter> parameters) DeleteQueryGenerate<T>(this DatabaseConnectorBase connector, Expression<Func<T, bool>> predicate)
            where T : class
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var translator = new ExpressionTranslator<T>(connector.CompatibleFunctionName);
            var translateResult = translator.Translate(predicate);
            var query = $@"DELETE FROM {tableName} WHERE {translateResult.Expression}";
            return (query, translateResult.Parameters);
        }
        /// <summary>
        /// Generate SQL create table query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <returns></returns>
        public static string GenerateCreateTableStatement<T>(this DatabaseConnectorBase connector)
            where T : class
        {
            var tableName = typeof(T).TableNameAttributeValidate();
            var fields = new List<string>();
            var properties = typeof(T).PropertiesBindingFlagsAttributeValidate();
            var referenceProperties = typeof(T).ForeignKeyAttributeValidate();
            foreach (var property in properties)
            {
                var propertyName = AttributeExtension.FieldNameAttributeValidate(property);
                var IsNotNull = AttributeExtension.NotNullAttributeValidate(property);
                var primaryKeyPostfix = property.IsSqlPrimaryKeyAttribute() ? " PRIMARY KEY " : "";
                var notNullPostfix = IsNotNull ? " NOT NULL " : "";
                var sqlType = connector.CompatibleSQLType(property.PropertyType);
                fields.Add($"{propertyName} {sqlType} {primaryKeyPostfix} {notNullPostfix}");
            }
            foreach (var foreignKey in referenceProperties)
            {
                var propertyName = AttributeExtension.FieldNameAttributeValidate(foreignKey);
                var targetTable = foreignKey.DeclaringType.TableNameAttributeValidate();
                fields.Add($"CONSTRAINT fk_{typeof(T).Name}_{targetTable} FOREIGN KEY ({foreignKey.ForeignKeyName}) REFERENCES {targetTable} ({propertyName})");
            }
            return $"CREATE TABLE {tableName}({string.Join(",", fields)})";
        }
    }
    #endregion
}

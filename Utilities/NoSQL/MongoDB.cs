using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Classes;
using Utilities.Enum;
using Utilities.Interfaces;
using Utilities.Shared;

namespace Utilities.NoSQL
{
    public class MongoDB //: INoSQLConnectorExtension
    {
        private MongoClient _connector { get; }
        private List<string> _databases { get; }
        public string ConnectionString { get; }

        public bool IsOpen => throw new NotImplementedException();

        public Dictionary<SqlFunction, string> SQLFunctionConfiguration => throw new NotImplementedException();
        public MongoDB(string connectionString)
        {
            connectionString = connectionString;
            _connector = new MongoClient(connectionString);
            _databases = _connector.ListDatabases().ToList().Select(x => x["name"].ToString()).ToList();
        }
        public IMongoDatabase UseDatabase(string databaseName, MongoDatabaseSettings mongoDatabaseSettings = null)
        {
            if (!_databases.Contains(databaseName))
                throw new Exception($"Database '{databaseName}' is not exists in current connection (found {string.Join(",", _databases)})");
            return _connector.GetDatabase(databaseName, mongoDatabaseSettings);
        }
        [Obsolete("This method will throw NotImplementedException, please use the overload one instead.")]
        public int CreateTable<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }
        public int CreateTable<T>(IMongoDatabase database) where T : class
        {
            var tableName = Utilities.Shared.AttributeExtension.TableNameAttributeValidate(typeof(T));
            database.CreateCollection(tableName);
            _databases.Add(tableName);
            return 1;
        }
    }
    public static class MongoDBDatabaseExtension
    {

        public static DeleteResult Delete<T>(this IMongoDatabase database, T obj) where T : class, new()
        {
            var primaryKey = typeof(T).PrimaryKeyAttributeValidate();
            var field = primaryKey.Name;
            var value = primaryKey.GetValue(obj);
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return collection.DeleteOne(filter);
        }

        public static DeleteResult Delete<T>(this IMongoDatabase database, object primaryKey) where T : class, new()
        {
            var pk = typeof(T).PrimaryKeyAttributeValidate();
            var field = pk.Name;
            var value = primaryKey;
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return collection.DeleteOne(filter);
        }

        public static DeleteResult Delete<T>(this IMongoDatabase database, Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            return collection.DeleteOne(predicate);
        }

        public static async Task<DeleteResult> DeleteAsync<T>(this IMongoDatabase database, T obj) where T : class, new()
        {
            var primaryKey = typeof(T).PrimaryKeyAttributeValidate();
            var field = primaryKey.Name;
            var value = primaryKey.GetValue(obj);
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return await collection.DeleteOneAsync(filter);
        }

        public static async Task<DeleteResult> DeleteAsync<T>(this IMongoDatabase database, object primaryKey) where T : class, new()
        {
            var pk = typeof(T).PrimaryKeyAttributeValidate();
            var field = pk.Name;
            var value = primaryKey;
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return await collection.DeleteOneAsync(filter);
        }

        public static async Task<DeleteResult> DeleteAsync<T>(this IMongoDatabase database, Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            return await collection.DeleteOneAsync(predicate);
        }


        public static int DROP_TABLE_USE_WITH_CAUTION<T>(this IMongoDatabase database) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            database.DropCollection(table);
            return 0;
        }


        public static int Insert<T>(this IMongoDatabase database, T obj) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            collection.InsertOne(obj);
            return 0;
        }

        public static int Insert<T>(this IMongoDatabase database, IEnumerable<T> obj) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            collection.InsertMany(obj);
            return 0;
        }

        public static async Task<int> InsertAsync<T>(this IMongoDatabase database, T obj) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            await collection.InsertOneAsync(obj);
            return 0;
        }


        public static IEnumerable<T> Select<T>(this IMongoDatabase database, int? top = null) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var buffer = collection.Find<T>(new BsonDocument());
            var result = top == null ? buffer.ToList() : buffer.Limit(top.Value).ToList();
            return result;
        }

        public static IEnumerable<T> Select<T>(this IMongoDatabase database, Expression<Func<T, bool>> predicate, int? top = null) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var buffer = collection.Find<T>(predicate);
            var result = top == null ? buffer.ToList() : buffer.Limit(top.Value).ToList();
            return result;
        }

        public static T Select<T>(this IMongoDatabase database, object primaryKey) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(typeof(T).PrimaryKeyAttributeValidate().Name, primaryKey);
            var buffer = collection.Find<T>(filter);
            return buffer.FirstOrDefault();
        }

        public static int Update<T>(this IMongoDatabase database, T obj) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public static async Task<int> UpdateAsync<T>(this IMongoDatabase database, T obj) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}

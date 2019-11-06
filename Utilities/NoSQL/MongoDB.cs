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
    /// <summary>
    /// Wrapper class for MongoClient instance.
    /// </summary>
    public class MongoDB : IDisposable
    {
        /// <summary>
        /// Instance of underlying MongoClient.
        /// </summary>
        public MongoClient Connection { get; private set; }
        /// <summary>
        /// Active database found by internal worker.
        /// </summary>
        public List<string> Databases { get; }
        /// <summary>
        /// Connection string of current connection.
        /// </summary>
        public string ConnectionString { get; }
        /// <summary>
        /// Active database proposed by user.
        /// </summary>
        public IMongoDatabase ActiveDatabase { get; private set; }
        protected bool _disposed { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public MongoDB(string connectionString)
        {
            ConnectionString = connectionString;
            Connection = new MongoClient(connectionString);
            Databases = Connection.ListDatabases().ToList().Select(x => x["name"].ToString()).ToList();
        }
        /// <summary>
        /// Tell the instance on what database should be active.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="mongoDatabaseSettings"></param>
        /// <returns></returns>
        public IMongoDatabase UseDatabase(string databaseName, MongoDatabaseSettings mongoDatabaseSettings = null)
        {
            if (!Databases.Contains(databaseName))
                throw new Exception($"Database '{databaseName}' is not exists in current connection (found {string.Join(",", Databases)})");
            ActiveDatabase = Connection.GetDatabase(databaseName, mongoDatabaseSettings);
            return ActiveDatabase;
        }
        /// <summary>
        /// Create collection on active database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <returns></returns>
        public int CreateCollection<T>() where T : class, new()
        {
            var tableName = Utilities.Shared.AttributeExtension.TableNameAttributeValidate(typeof(T));
            ActiveDatabase.CreateCollection(tableName);
            Databases.Add(tableName);
            return 1;
        }
        /// <summary>
        /// Object disposer which release managed resources of this object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                Databases.Clear();
                _disposed = true;
            }
        }
        /// <summary>
        /// Object disposer which release managed resources of this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Verify if active database is available.
        /// </summary>
        protected virtual void VerifyActiveDatabase()
        {
            if (ActiveDatabase == null)
            {
                throw new NullReferenceException("Active database is not set, you must call 'UseDatabase' before attempt to execute any action.");
            }
        }
        /// <summary>
        /// Delete specified document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public DeleteResult Delete<T>(T obj) where T : class, new()
        {
            VerifyActiveDatabase();
            var primaryKey = typeof(T).PrimaryKeyAttributeValidate();
            var field = primaryKey.Name;
            var value = primaryKey.GetValue(obj);
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return collection.DeleteOne(filter);
        }
        /// <summary>
        /// Delete document using key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public DeleteResult Delete<T>(object primaryKey) where T : class, new()
        {
            VerifyActiveDatabase();
            var pk = typeof(T).PrimaryKeyAttributeValidate();
            var field = pk.Name;
            var value = primaryKey;
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return collection.DeleteOne(filter);
        }
        /// <summary>
        /// Delete document using predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public DeleteResult Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            VerifyActiveDatabase();
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            return collection.DeleteOne(predicate);
        }
        /// <summary>
        /// Delete specified document in an asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteAsync<T>(T obj) where T : class, new()
        {
            VerifyActiveDatabase();
            var primaryKey = typeof(T).PrimaryKeyAttributeValidate();
            var field = primaryKey.Name;
            var value = primaryKey.GetValue(obj);
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return await collection.DeleteOneAsync(filter);
        }
        /// <summary>
        /// Delete document using key in asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteAsync<T>(object primaryKey) where T : class, new()
        {
            VerifyActiveDatabase();
            var type = typeof(T);
            var pk = type.PrimaryKeyAttributeValidate();
            var field = pk.Name;
            var value = primaryKey;
            var table = type.TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(field, value);
            return await collection.DeleteOneAsync(filter);
        }
        /// <summary>
        /// Delete document using predicate in asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            VerifyActiveDatabase();
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            return await collection.DeleteOneAsync(predicate);
        }

        /// <summary>
        /// Drop collection on active database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int DropCollection<T>() where T : class, new()
        {
            VerifyActiveDatabase();
            var table = typeof(T).TableNameAttributeValidate();
            ActiveDatabase.DropCollection(table);
            return 0;
        }

        /// <summary>
        /// Insert specified document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Insert<T>(T obj) where T : class, new()
        {
            VerifyActiveDatabase();
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            collection.InsertOne(obj);
            return 0;
        }
        /// <summary>
        /// Insert documents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Insert<T>(IEnumerable<T> obj) where T : class, new()
        {
            VerifyActiveDatabase();
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            collection.InsertMany(obj);
            return 0;
        }
        /// <summary>
        /// Insert specified document in an asynchronous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync<T>(T obj) where T : class, new()
        {
            VerifyActiveDatabase();
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            await collection.InsertOneAsync(obj);
            return 0;
        }

        /// <summary>
        /// Read documents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(int? top = null) where T : class, new()
        {
            VerifyActiveDatabase();
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var buffer = collection.Find<T>(new BsonDocument());
            var result = top == null ? buffer.ToList() : buffer.Limit(top.Value).ToList();
            return result;
        }
        /// <summary>
        /// Read documents which match predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(Expression<Func<T, bool>> predicate, int? top = null) where T : class, new()
        {
            VerifyActiveDatabase();
            var table = typeof(T).TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var buffer = collection.Find<T>(predicate);
            var result = top == null ? buffer.ToList() : buffer.Limit(top.Value).ToList();
            return result;
        }
        /// <summary>
        /// Read document using key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public T Query<T>(object primaryKey) where T : class, new()
        {
            VerifyActiveDatabase();
            var type = typeof(T);
            var table = type.TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(type.PrimaryKeyAttributeValidate().Name, primaryKey);
            var buffer = collection.Find<T>(filter);
            return buffer.FirstOrDefault();
        }
        /// <summary>
        /// Update specified document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ReplaceOneResult Update<T>(T obj) where T : class, new()
        {
            VerifyActiveDatabase();
            var type = typeof(T);
            var table = type.TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var primaryKey = type.PrimaryKeyAttributeValidate();
            var filter = Builders<T>.Filter.Eq(primaryKey.Name, primaryKey.GetValue(obj));
            var result = collection.ReplaceOne(filter, obj);
            return result;
        }
        /// <summary>
        /// Update specified document in an asynchrounous manner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<ReplaceOneResult> UpdateAsync<T>(T obj) where T : class, new()
        {
            VerifyActiveDatabase();
            var type = typeof(T);
            var table = type.TableNameAttributeValidate();
            var collection = ActiveDatabase.GetCollection<T>(table);
            var primaryKey = type.PrimaryKeyAttributeValidate();
            var filter = Builders<T>.Filter.Eq(primaryKey.Name, primaryKey.GetValue(obj));
            var result = await collection.ReplaceOneAsync(filter, obj);
            return result;
        }
    }
    /// <summary>
    /// Extension set of methods which integrated with native MongoDB Driver.
    /// </summary>
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


        public static IEnumerable<T> Query<T>(this IMongoDatabase database, int? top = null) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var buffer = collection.Find<T>(new BsonDocument());
            var result = top == null ? buffer.ToList() : buffer.Limit(top.Value).ToList();
            return result;
        }

        public static IEnumerable<T> Query<T>(this IMongoDatabase database, Expression<Func<T, bool>> predicate, int? top = null) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var buffer = collection.Find<T>(predicate);
            var result = top == null ? buffer.ToList() : buffer.Limit(top.Value).ToList();
            return result;
        }

        public static T Query<T>(this IMongoDatabase database, object primaryKey) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq(typeof(T).PrimaryKeyAttributeValidate().Name, primaryKey);
            var buffer = collection.Find<T>(filter);
            return buffer.FirstOrDefault();
        }

        public static ReplaceOneResult Update<T>(this IMongoDatabase database, T obj) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var primaryKey = typeof(T).PrimaryKeyAttributeValidate();
            var filter = Builders<T>.Filter.Eq(primaryKey.Name, primaryKey.GetValue(obj));
            var result = collection.ReplaceOne(filter, obj);
            return result;
        }

        public static async Task<ReplaceOneResult> UpdateAsync<T>(this IMongoDatabase database, T obj) where T : class, new()
        {
            var table = typeof(T).TableNameAttributeValidate();
            var collection = database.GetCollection<T>(table);
            var primaryKey = typeof(T).PrimaryKeyAttributeValidate();
            var filter = Builders<T>.Filter.Eq(primaryKey.Name, primaryKey.GetValue(obj));
            var result = await collection.ReplaceOneAsync(filter, obj);
            return result;
        }
    }
}

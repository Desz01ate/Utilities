﻿using MySql.Data.MySqlClient;
using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Utilities.Testing.Models;
using Utilities.Testing.SQLConnectors;

namespace Utilities.Testing
{
    [TestFixture]
    internal class SQL
    {
        private string _msSqlConnection;
        private string _mySqlConnection;
        private string _sqliteConnection;

        [SetUp]
        public void Setup()
        {
            _msSqlConnection = @"Server=localhost;Database=Local;user=sa;password=sa;";
            _mySqlConnection = @"Server=localhost;Database=Local;Uid=root;Pwd=;";
            _sqliteConnection = $@"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files\Local.db")};Version=3;";
        }

        [Test]
        public void SQLServerConnectorCRUD()
        {
            using (var connection = new SQLServer(_msSqlConnection))
            {
                try
                {
                    connection.CreateTable<TestTable>();
                    for (var iter = 0; iter < 10; iter++)
                    {
                        TestTable testTable = new TestTable() { id = iter, value = $"test" };
                        var affectedCreate = connection.Insert(testTable);
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = connection.Select<TestTable>(primaryKey: iter);
                        Assert.AreEqual(selectedById.id, iter);
                        Assert.AreEqual(selectedById.value, "test");
                        var selectedByIdLambda = connection.Select<TestTable>(x => x.id == iter);
                        Assert.AreEqual(selectedByIdLambda.First().id, iter);
                        Assert.AreEqual(selectedByIdLambda.First().value, "test");
                        var selectedAll = connection.Select<TestTable>();
                        Assert.AreEqual(selectedAll.First().id, iter);
                        Assert.AreEqual(selectedAll.First().value, "test");
                        testTable.value = "updated";
                        var affectedUpdate = connection.Update(testTable);
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDeleteLambdaInvalid = connection.Delete<TestTable>(x => x.id == iter + 1);
                        Assert.AreEqual(affectedDeleteLambdaInvalid, 0);
                        var affectedDelete = connection.Delete(testTable);
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = connection.Select<TestTable>();
                    Assert.AreEqual(affectedSelectScalar.Count(), 0);
                    Assert.Pass();
                }
                finally
                {
                    connection.DROP_TABLE_USE_WITH_CAUTION<TestTable>();
                }
            }
        }

        [Test]
        public void SQLServerConnector()
        {
            using (var connection = new SQLServer(_msSqlConnection))
            {
                try
                {
                    connection.ExecuteNonQuery($@"CREATE TABLE [dbo].[TestTable]([id] int primary key,[value] nvarchar(255))");
                    for (var iter = 0; iter < 10; iter++)
                    {
                        var affectedCreate = connection.ExecuteNonQuery(@"INSERT INTO [dbo].[TestTable]([id],[value]) VALUES(@id,@value)", new[] { new SqlParameter("id", iter), new SqlParameter("value", "test") });
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = connection.ExecuteReader<TestTable>(@"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(selectedById.First().id, iter);
                        Assert.AreEqual(selectedById.First().value, "test");
                        var selectedByIdDynamic = connection.ExecuteReader(@"SELECT * FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                        Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                        var affectedUpdate = connection.ExecuteNonQuery(@"UPDATE [dbo].[TestTable] SET [value] = @value WHERE [id] = @id", new[] { new SqlParameter("id", iter), new SqlParameter("value", "updated") });
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDelete = connection.ExecuteNonQuery(@"DELETE FROM [dbo].[TestTable] WHERE [id] = @id", new[] { new SqlParameter("id", iter) });
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = connection.ExecuteScalar<int>(@"SELECT COUNT(1) FROM [dbo].[TestTable]");
                    Assert.AreEqual(affectedSelectScalar, 0);
                    Assert.Pass();
                }
                finally
                {
                    connection.ExecuteNonQuery($@"DROP TABLE [dbo].[TestTable]");
                }
            }
        }

        public void MySQLConnector()
        {
            using (var connection = new MySQL(_mySqlConnection))
            {
                try
                {
                    connection.ExecuteNonQuery($@"CREATE TABLE TestTable(id int primary key,value nvarchar(255))");
                    for (var iter = 0; iter < 10; iter++)
                    {
                        var affectedCreate = connection.ExecuteNonQuery(@"INSERT INTO TestTable(id,value) VALUES(@id,@value)", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "test") });
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = connection.ExecuteReader<TestTable>(@"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(selectedById.First().id, iter);
                        Assert.AreEqual(selectedById.First().value, "test");
                        var selectedByIdDynamic = connection.ExecuteReader(@"SELECT * FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                        Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                        var affectedUpdate = connection.ExecuteNonQuery(@"UPDATE TestTable SET value = @value WHERE id = @id", new[] { new MySqlParameter("id", iter), new MySqlParameter("value", "updated") });
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDelete = connection.ExecuteNonQuery(@"DELETE FROM TestTable WHERE id = @id", new[] { new MySqlParameter("id", iter) });
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = connection.ExecuteScalar<long>(@"SELECT COUNT(1) FROM TestTable");
                    Assert.AreEqual(affectedSelectScalar, 0);
                    Assert.Pass();
                }
                finally
                {
                    connection.ExecuteNonQuery($@"DROP TABLE TestTable");
                }
            }
        }

        [Test]
        public void SQLiteConnector()
        {
            using (var connection = new SQLite(_sqliteConnection))
            {
                try
                {
                    connection.ExecuteNonQuery($@"CREATE TABLE TestTable(id int primary key,value nvarchar(255))");
                    for (var iter = 0; iter < 10; iter++)
                    {
                        var affectedCreate = connection.ExecuteNonQuery(@"INSERT INTO TestTable(id,value) VALUES(@id,@value)", new[] { new SQLiteParameter("id", iter), new SQLiteParameter("value", "test") });
                        Assert.AreEqual(affectedCreate, 1);
                        var selectedById = connection.ExecuteReader<TestTable>(@"SELECT * FROM TestTable WHERE id = @id", new[] { new SQLiteParameter("id", iter) });
                        Assert.AreEqual(selectedById.First().id, iter);
                        Assert.AreEqual(selectedById.First().value, "test");
                        var selectedByIdDynamic = connection.ExecuteReader(@"SELECT * FROM TestTable WHERE id = @id", new[] { new SQLiteParameter("id", iter) });
                        Assert.AreEqual(selectedByIdDynamic.First().id, iter);
                        Assert.AreEqual(selectedByIdDynamic.First().value, "test");
                        var affectedUpdate = connection.ExecuteNonQuery(@"UPDATE TestTable SET value = @value WHERE id = @id", new[] { new SQLiteParameter("id", iter), new SQLiteParameter("value", "updated") });
                        Assert.AreEqual(affectedUpdate, 1);
                        var affectedDelete = connection.ExecuteNonQuery(@"DELETE FROM TestTable WHERE id = @id", new[] { new SQLiteParameter("id", iter) });
                        Assert.AreEqual(affectedDelete, 1);
                    }
                    var affectedSelectScalar = connection.ExecuteScalar<long>(@"SELECT COUNT(1) FROM TestTable");
                    Assert.AreEqual(affectedSelectScalar, 0);
                    Assert.Pass();
                }
                finally
                {
                    connection.ExecuteNonQuery($@"DROP TABLE TestTable");
                }
            }
        }
    }
}
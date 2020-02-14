using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Utilities.SQL;
using System.Data.Common;
using Utilities.Enum;
using Utilities.SQL.Extension;
using Utilities.Testing.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Utilities.Interfaces;

namespace Utilities.Testing
{
    internal class UnrelatedTest
    {
        public class AuthorizationModel
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public DateTime ValidUntil { get; set; }
        }
        public class Response<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
        abstract class DatabaseConnectorContract
        {
            protected abstract string CompatibleFunction();
            protected abstract string CompatibleType();
            protected virtual string MapAnything()
            {
                return string.Empty;
            }
            public int ExecuteNonQuery()
            {
                return 1;
            }
        }
        class MyConnector : DatabaseConnectorContract
        {
            protected override string MapAnything()
            {
                return "New Value";
            }
            protected override string CompatibleFunction()
            {
                throw new NotImplementedException();
            }

            protected override string CompatibleType()
            {
                throw new NotImplementedException();
            }
        }
        //[Test]
        public async Task Playground()
        {
            var connector = new DatabaseConnector(null);
            connector.OnqueryExecuted += (s) => { };
            IDatabaseConnector con2 = connector;
        }
    }
}
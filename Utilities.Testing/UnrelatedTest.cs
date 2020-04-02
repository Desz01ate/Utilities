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
using System.Dynamic;
using Utilities.Shared;

namespace Utilities.Testing
{
    internal class UnrelatedTest
    {
        [Test]
        public async Task Playground()
        {
            using var connector = new DatabaseConnector(typeof(DatabaseConnector), "");
            var data = connector.ExecuteReader($@"SELECT TOP (10) [ID]
      ,[Activity]
      ,[Module]
      ,[ReferenceID]
      ,[Detail]
      ,[Username]
      ,[TimeStamp]
      ,[ObjJson]
  FROM[dbo].[ActivityLog]");
            var dt = data.ToDataTable();

        }
    }
}
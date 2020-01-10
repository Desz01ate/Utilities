using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Utilities.SQL.Abstract;
using Utilities.Testing.Models;
using Utilities.Testing.SQLConnectors;
using Utilities.Shared;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace Utilities.Testing
{
    internal class UnrelatedTest
    {
        class Config : IConfiguration
        {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "JwtIssuer" , "Tyche Co,Ltd." },
                {"JwtAudience" ,"Tyche Co,Ltd." },
                { "JwtKey" , "VHljaGUgQ28sTHRkLg==" },
                { "JwtExpireMinutes" , "30" }
            };
            public string this[string key]
            {
                get
                {
                    return data[key];
                }
                set
                {
                    data.Add(key, value);
                }
            }

            public IEnumerable<IConfigurationSection> GetChildren()
            {
                throw new NotImplementedException();
            }

            public IChangeToken GetReloadToken()
            {
                throw new NotImplementedException();
            }

            public IConfigurationSection GetSection(string key)
            {
                throw new NotImplementedException();
            }
        }
        class Controller : Utilities.Asp.Core.Based.JwtAuthorizationBasedController
        {
            private ConcurrentDictionary<string, string> refreshTokenCache = new ConcurrentDictionary<string, string>();
            public Controller(IConfiguration configuration) : base(configuration)
            {
            }

            protected override string RetrieveRefreshTokenFromPreferredDataSource(string username)
            {
                if (refreshTokenCache.TryGetValue(username, out var value))
                {
                    return value;
                }
                return string.Empty;
            }

            protected override void SaveRefreshTokenToPreferredDataSource(string username, string refreshToken)
            {
                refreshTokenCache.AddOrUpdate(username, refreshToken, AddOrUpdate);
            }

            private string AddOrUpdate(string arg1, string arg2)
            {
                return arg2;
            }

            protected override bool VerifyAuthentication(string id)
            {
                return true;
            }
        }
        [Test]
        public async Task Playground()
        {
            var controller = new Controller(new Config());
            var res = controller.Authenticate("admin");
            var res2 = controller.RefreshToken(res.Data);
            //Console.ReadLine();
        }
        public string Test(IEnumerable<string> a)
        {
            foreach (var x in a)
            {
                Console.WriteLine(x);
            }
            var all = new StringBuilder();
            foreach (var x in a)
            {
                all.Append(a);
            }
            return all.ToString();
        }
    }
}
using System;
using Utilities.Attributes.SQL;

namespace Utilities.Testing.Models
{
    public class Users
    {
        [PrimaryKey]
        public Guid id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}

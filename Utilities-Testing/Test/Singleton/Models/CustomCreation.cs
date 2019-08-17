using System;
using Utilities.Attributes.SQL;

namespace Utilities.Testing1.Models
{
    public class CustomCreation
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int? Score { get; set; }
    }
}

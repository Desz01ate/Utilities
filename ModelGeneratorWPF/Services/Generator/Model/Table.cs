using System.Collections.Generic;
using Utilities.Classes;

namespace ModelGenerator.Services.Generator.Model
{
    public class Table
    {
        public string Name { get; set; }
        public string PrimaryKey { get; set; }
        public IEnumerable<TableSchema> Columns { get; set; }
    }
}

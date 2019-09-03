using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Classes;

namespace ModelGenerator.Services.Generator.Model
{
    public class Table
    {
        public string Name { get; set; }
        public IEnumerable<TableSchema> Columns { get; set; }
    }
}

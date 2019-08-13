using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.SQL.Generator.Model
{
    public class Table
    {
        public string Name { get; set; }
        public List<SqlColumn> Columns { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Attributes.SQL;

namespace Utilities.Testing.Models
{
    public class TestTable
    {
        [PrimaryKey]
        public int id { get; set; }
        public string value { get; set; }
    }
}
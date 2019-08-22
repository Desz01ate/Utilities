using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.Structs
{
    public struct QueryParamsCombination<TParameter> where TParameter : DbParameter, new()
    {
        public string query { get; set; }
        public IEnumerable<TParameter> parameters { get; set; }
        public QueryParamsCombination(string q, IEnumerable<TParameter> p)
        {
            query = q;
            parameters = p;
        }
    }
}

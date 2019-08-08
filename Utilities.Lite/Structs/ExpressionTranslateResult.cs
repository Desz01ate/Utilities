using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.Structs
{
    public struct ExpressionTranslateResult<TSqlParameter> where TSqlParameter : DbParameter, new()
    {
        public string Expression { get; }
        public IEnumerable<TSqlParameter> Parameters { get; }
        public ExpressionTranslateResult(string expression, IEnumerable<TSqlParameter> parameters)
        {
            Expression = expression;
            Parameters = parameters;
        }
    }
}

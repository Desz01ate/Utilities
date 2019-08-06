using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Utilities.Enumerables;
using Utilities.Shared;

namespace Utilities.SQL.Translator
{
    //implementation taken from https://stackoverflow.com/questions/7731905/how-to-convert-an-expression-tree-to-a-partial-sql-query with some customized
    public class ExpressionTranslator<TObject, TSqlParameter> : ExpressionVisitor
        where TObject : class, new()
        where TSqlParameter : DbParameter, new()
    {
        private StringBuilder sb;
        private string _orderBy = string.Empty;
        private int? _skip = null;
        private int? _take = null;
        private string _whereClause = string.Empty;
        private Dictionary<SqlFunction, string> _platformFunctionConfig;
        private Dictionary<string, string> _fieldsConfiguration;
        private List<TSqlParameter> _sqlParameters;
        public int? Skip
        {
            get
            {
                return _skip;
            }
        }

        public int? Take
        {
            get
            {
                return _take;
            }
        }

        public string OrderBy
        {
            get
            {
                return _orderBy;
            }
        }

        public string WhereClause
        {
            get
            {
                return _whereClause;
            }
        }

        public ExpressionTranslator(Dictionary<SqlFunction, string> platformFunctionConfiguration)
        {
            _platformFunctionConfig = platformFunctionConfiguration;
            _fieldsConfiguration = new Dictionary<string, string>();
            _sqlParameters = new List<TSqlParameter>();
            foreach (var property in typeof(TObject).PropertiesValidate())
            {
                var key = property.Name;
                var value = property.FieldNameValidate();
                _fieldsConfiguration.Add(key, value);
            }
            //_fieldsConfiguration = fieldsConfiguration;
        }


        public string Translate(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
            _whereClause = this.sb.ToString();
            return _whereClause;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }
            else if (m.Method.Name == "Take")
            {
                if (this.ParseTakeExpression(m))
                {
                    Expression nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "Skip")
            {
                if (this.ParseSkipExpression(m))
                {
                    Expression nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "OrderBy")
            {
                if (this.ParseOrderByExpression(m, "ASC"))
                {
                    Expression nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "OrderByDescending")
            {
                if (this.ParseOrderByExpression(m, "DESC"))
                {
                    Expression nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "Contains")
            {
                var field = _fieldsConfiguration[((MemberExpression)m.Object).Member.Name];
                var expression = m.Arguments[0].ToString().Replace("\"", "");
                _sqlParameters.Add(new TSqlParameter()
                {
                    ParameterName = field,
                    Value = expression
                });
                sb.Append($@"({field} LIKE '%' + @{field} + '%')");
                return m;

            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS ");
                    }
                    else
                    {
                        sb.Append(" = ");
                    }
                    break;

                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS NOT ");
                    }
                    else
                    {
                        sb.Append(" <> ");
                    }
                    break;

                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                case ExpressionType.Add:
                    sb.Append(" + ");
                    break;
                case ExpressionType.Subtract:
                    sb.Append(" - ");
                    break;
                case ExpressionType.Divide:
                    sb.Append(" / ");
                    break;
                case ExpressionType.Multiply:
                    sb.Append(" * ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

            }

            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q == null && c.Value == null)
            {
                sb.Append("NULL");
            }
            else if (q == null)
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? 1 : 0);
                        break;

                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;

                    case TypeCode.DateTime:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;

                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));

                    default:
                        sb.Append(c.Value);
                        break;
                }
            }

            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null)
            {
                switch (m.Expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        sb.Append(_fieldsConfiguration[m.Member.Name]);
                        return m;
                    case ExpressionType.Constant:
                        var f = Expression.Lambda(m).Compile();
                        var v = f.DynamicInvoke();
                        var fieldInfo = (m.Member as FieldInfo);
                        if (IsQuoteNeeded(fieldInfo.FieldType))
                            sb.Append($"'{v}'");
                        else
                            sb.Append(v);
                        return m;
                    //need more research on this
                    case ExpressionType.MemberAccess:
                        var accessingProperty = m.Member.Name.ToLower();
                        var accessingPropetyType = m.Member.GetType();
                        switch (accessingProperty)
                        {
                            case "length":
                                var member = (m.Expression as MemberExpression).Member.Name;
                                var lengthFunction = _platformFunctionConfig[SqlFunction.Length];
                                sb.Append($"{lengthFunction}({member})");
                                break;
                            default:
                                var objectMember = Expression.Convert(m, typeof(object));
                                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                                var getter = getterLambda.Compile();
                                if (IsQuoteNeeded((m.Member as PropertyInfo).PropertyType))
                                    sb.Append($"'{getter()}'");
                                else
                                    sb.Append(getter());
                                break;
                        }
                        return m;
                }
            }
            throw new NotSupportedException($"Expression contains unsupported statement ({m}).");
        }
        private bool IsQuoteNeeded(Type propertyType)
        {
            return
               propertyType == typeof(string) ||
               propertyType == typeof(char) ||
               propertyType == typeof(char?) ||
               propertyType == typeof(DateTime) ||
               propertyType == typeof(DateTime?) ||
               propertyType == typeof(Guid) ||
               propertyType == typeof(Guid?);
        }
        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }

        private bool ParseOrderByExpression(MethodCallExpression expression, string order)
        {
            UnaryExpression unary = (UnaryExpression)expression.Arguments[1];
            LambdaExpression lambdaExpression = (LambdaExpression)unary.Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            MemberExpression body = lambdaExpression.Body as MemberExpression;
            if (body != null)
            {
                if (string.IsNullOrEmpty(_orderBy))
                {
                    _orderBy = string.Format("{0} {1}", body.Member.Name, order);
                }
                else
                {
                    _orderBy = string.Format("{0}, {1} {2}", _orderBy, body.Member.Name, order);
                }

                return true;
            }

            return false;
        }

        private bool ParseTakeExpression(MethodCallExpression expression)
        {
            ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (int.TryParse(sizeExpression.Value.ToString(), out size))
            {
                _take = size;
                return true;
            }

            return false;
        }

        private bool ParseSkipExpression(MethodCallExpression expression)
        {
            ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (int.TryParse(sizeExpression.Value.ToString(), out size))
            {
                _skip = size;
                return true;
            }

            return false;
        }
    }
}

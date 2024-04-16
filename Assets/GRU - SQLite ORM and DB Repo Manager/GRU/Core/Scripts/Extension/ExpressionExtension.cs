using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace SpatiumInteractive.Libraries.Unity.GRU.Extensions
{
    public static class ExpressionExtension
    {
        private const string VALID_C_SHARP_PROP_NAMING_SYNTAX_REGEX = @"^@?[a-zA-Z_]\w*(\.@?[a-zA-Z_]\w*)*$";

        public static IWhereExpression<TEntity> ToWhereExpression<TEntity>(this Expression<Func<TEntity, bool>> expression)
        {
            return expression.ToWhereExpression(WhereOrder.BeforeSelect);
        }

        public static IWhereExpression<TEntity> ToWhereExpression<TEntity>(this Expression<Func<TEntity, bool>> expression, WhereOrder whereOrder)
        {
            return new WhereExpression<TEntity>(expression, whereOrder);
        }

        public static bool IsLambdaMemberExpression(this Expression expression, out string rightSideArg)
        {
            rightSideArg = string.Empty;
            string expr = expression.ToString();
            bool hasDotSeperator = expr.Contains(".");
            if (hasDotSeperator)
            {
                var sides = expr.Split(".");
                string left = sides[0];
                string right = sides[1];

                bool isLeftValidProp = Regex.IsMatch(left, VALID_C_SHARP_PROP_NAMING_SYNTAX_REGEX);
                bool isRightValidProp = Regex.IsMatch(right, VALID_C_SHARP_PROP_NAMING_SYNTAX_REGEX);

                bool isLambdaMemberExpr = isLeftValidProp && isRightValidProp;

                if (isLambdaMemberExpr)
                {
                    rightSideArg = right;
                }

                return isLambdaMemberExpr;
            }
            return false;
        }
    }
}

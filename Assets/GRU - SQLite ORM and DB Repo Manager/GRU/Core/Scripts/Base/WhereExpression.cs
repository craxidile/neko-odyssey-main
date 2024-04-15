using System;
using System.Linq.Expressions;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace SpatiumInteractive.Libraries.Unity.GRU.Base
{
    internal class WhereExpression<TEntity> : IWhereExpression<TEntity>
    {
        #region Properties

        public Expression<Func<TEntity, bool>> Expression { get; }

        public WhereOrder WhereOrder { get; }

        #endregion

        #region Constructors

        public WhereExpression(Expression<Func<TEntity, bool>> expression, WhereOrder whereOrder)
        {
            Expression = expression;
            WhereOrder = whereOrder;
        }

        #endregion
    }
}

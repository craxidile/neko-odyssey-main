using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Base
{
    public class QueryExpression<TEntity>
        where TEntity : IEntityBase, IAggregateRoot
    {
        #region Properties

        public Expression<Func<TEntity, TEntity>> SelectExpression { get; private set; }

        public IList<IWhereExpression<TEntity>> WhereExpression { get; }

        public IList<OrderExpression<TEntity>> OrderExpressions { get; }

        public bool ShouldDistinct { get; private set; }

        #endregion

        #region Constructors

        public QueryExpression()
        {
            OrderExpressions = new List<OrderExpression<TEntity>>();
            WhereExpression = new List<IWhereExpression<TEntity>>();
        }

        #endregion

        #region Public Methods

        public void AddSelectExpression(Expression<Func<TEntity, TEntity>> expression, bool shouldDistinct = false)
        {
            SelectExpression = expression;
            ShouldDistinct = shouldDistinct;
        }

        public void AddWhereExpression(Expression<Func<TEntity, bool>> expression)
        {
            AddWhereExpression(expression, WhereOrder.BeforeSelect);
        }

        public void AddWhereExpression(Expression<Func<TEntity, bool>> expression, WhereOrder whereOrder)
        {
            WhereExpression.Add(expression.ToWhereExpression(whereOrder));
        }

        public void AddOrderExpression(Expression<Func<TEntity, object>> expression, OrderDirection direction, bool ignoreCasing = true)
        {
            OrderExpressions.Add(new OrderExpression<TEntity>(expression, direction, ignoreCasing));
        }

        #endregion
    }
}

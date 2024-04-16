using System;
using System.Linq.Expressions;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace SpatiumInteractive.Libraries.Unity.GRU.Base
{
    public class OrderExpression<TEntity>
        where TEntity : IEntityBase, IAggregateRoot
    {
        #region Properties

        public Expression<Func<TEntity, object>> Property { get; }

        public OrderDirection Direction { get; }

        public bool IgnoreCasing { get; }

        #endregion

        #region Constructors

        public OrderExpression(Expression<Func<TEntity, object>> property, OrderDirection direction, bool ignoreCasing)
        {
            Property = property;
            Direction = direction;
            IgnoreCasing = ignoreCasing;
        }

        #endregion
    }
}

using SpatiumInteractive.Libraries.Unity.GRU.Base;
using System;
using System.Linq.Expressions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Contracts
{
    public interface IWhereExpression<TEntity>
    {
        Expression<Func<TEntity, bool>> Expression { get; }

        WhereOrder WhereOrder { get; }
    }
}

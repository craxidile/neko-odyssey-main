using SpatiumInteractive.Libraries.Unity.GRU.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Contracts
{
    public interface IReadOnlyRepositoryBase<T, TId> 
        where T : IAggregateRoot 
        where TId : struct
    {
        T FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, EntityBase<TId>>>[] includeExpressions);
        
        IEnumerable<T> FindAll(params Expression<Func<T, EntityBase<TId>>>[] includeExpressions);
        
        IEnumerable<T> FindAllBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, EntityBase<TId>>>[] includeExpressions);
    }
}

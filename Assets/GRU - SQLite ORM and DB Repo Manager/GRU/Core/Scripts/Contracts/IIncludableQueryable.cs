using System.Collections.Generic;

namespace SpatiumInteractive.Libraries.Unity.GRU.Contracts
{
    public interface IIncludableQueryable<out TEntity, out TProperty, TId>
    {
        TEntity Source { get; }
        TProperty Property { get; }
        IEnumerable<TProperty> ListProperty { get; }
    }
}

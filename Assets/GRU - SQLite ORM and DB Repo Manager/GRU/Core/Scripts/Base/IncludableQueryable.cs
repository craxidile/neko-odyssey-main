using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using System.Collections.Generic;

namespace SpatiumInteractive.Libraries.Unity.GRU.Base
{
    public class IncludableQueryable<TEntity, TProperty, TId> : IIncludableQueryable<TEntity, TProperty, TId>
        where TEntity : IEntityBase
        where TId: struct
    {
        #region Properties

        public TEntity Source { get; private set; }

        public TProperty Property { get; private set; }

        public IEnumerable<TProperty> ListProperty { get; private set; }

        #endregion

        #region Constructors

        public IncludableQueryable(TEntity source, TProperty prop)
        {
            Source = source;
            Property = prop;
        }

        public IncludableQueryable(TEntity source, IEnumerable<TProperty> listProp)
        {
            Source = source;
            ListProperty = listProp;
        }

        #endregion
    }
}

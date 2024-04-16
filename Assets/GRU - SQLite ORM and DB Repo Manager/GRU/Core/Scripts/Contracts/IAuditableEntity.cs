using System;

namespace SpatiumInteractive.Libraries.Unity.GRU.Contracts
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }

        void SetCreated();
        void SetUpdated();
    }
}

using System;
using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace SpatiumInteractive.Libraries.Unity.GRU.Base
{
    /// <summary>
    /// Base that each databse table class must inherit.
    /// This is the infra support for creating entity's repo later on.
    /// </summary>
    /// <typeparam name="TId">
    /// data type used for Primary Key. 
    /// Best to pass <see cref="int"/> for this one as that's the only type 
    /// that's fully supported by sqlite at the moment
    /// </typeparam>
    public abstract class EntityBase<TId>: IEntityBase 
        where TId : struct
    {
        #region Properties

        [PrimaryKey, AutoIncrement]
        public TId Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Ignore] public bool IsNew => Id.Equals(default(TId));

        #endregion

        #region Constructors

        protected EntityBase()
        {
        }

        #endregion

        #region IAuditableEntity

        public void SetCreated()
        {
            CreatedAt = DateTime.Now;
            IsActive = true;
        }

        public void SetUpdated()
        {
            UpdatedAt = DateTime.Now;
        }

        public void SetDeleted()
        {
            IsActive = false;
            IsDeleted = true;
        }

        public void SetActive()
        {
            IsActive = true;
            IsDeleted = false;
        }

        #endregion

        #region Operational Overrides

        public override bool Equals(object entity)
        {
            return entity != null
                    && entity is EntityBase<TId>
                    && this == (EntityBase<TId>)entity;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(EntityBase<TId> entity1,
                                        EntityBase<TId> entity2)
        {
            if ((object)entity1 == null && (object)entity2 == null)
            {
                return true;
            }

            if ((object)entity1 == null || (object)entity2 == null)
            {
                return false;
            }

            if (entity1.Id.ToString() == entity2.Id.ToString())
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(EntityBase<TId> entity1,
                                        EntityBase<TId> entity2)
        {
            return (!(entity1 == entity2));
        }

        #endregion
    }
}

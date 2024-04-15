using System.Linq;
using System.Collections.Generic;
using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace SpatiumInteractive.Libraries.Unity.GRU.Domain
{
    public abstract class Repository<TEntity, TId, TDbContext> : ReadOnlyRepository<TEntity, TId, TDbContext>, IRepository<TEntity, TId>
         where TEntity : EntityBase<TId>, IAggregateRoot
         where TId : struct
         where TDbContext : SQLiteConnection
    {
        #region Constructors

        protected Repository(IDbContext<TDbContext> dbContext) : base(dbContext) { }

        #endregion

        #region IRepository

        public TEntity Add(TEntity entity)
        {
            entity.SetCreated();

            _dbContext.Context.RunInTransaction(() =>
            {
                _dbContext.Context.Insert(entity);
            });

            return entity;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            entities.All(entity =>
            {
                entity.SetUpdated();
                return true;
            });

            _dbContext.Context.RunInTransaction(() =>
            {
                _dbContext.Context.InsertAll(entities);
            });

            return entities;
        }

        public TEntity Update(TEntity entity)
        {
            entity.SetUpdated();

            _dbContext.Context.RunInTransaction(() =>
            {
                _dbContext.Context.Update(entity);
            });
            
            return entity;
        }

        public void Remove(TId id)
        {
            var entity = _dbContext.Context.FindAbstract<TEntity>(id);

            _dbContext.Context.RunInTransaction(() =>
            {
                _dbContext.Context.Delete(entity);
            });
        }

        public TEntity Remove(TEntity entity)
        {
            _dbContext.Context.RunInTransaction(() =>
            {
                _dbContext.Context.Delete(entity);
            });

            return entity;
        }

        public IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Context.RunInTransaction(() =>
            {
                _dbContext.Context.DeleteAll(entities);
            });
            return entities;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Call only if you manually started the transaction and 
        /// now you want to commit it. See  <see cref="NegotiateTransaction"/> 
        /// for more details.
        /// </summary>
        protected void SaveChanges()
        {
            if (_dbContext.Context.IsInTransaction)
            {
                _dbContext.Context.Commit();
            }
        }

        /// <summary>
        /// Call only if you manually started the transaction and 
        /// now you want to roll it back. See  <see cref="NegotiateTransaction"/> 
        /// for more details.
        /// </summary>
        protected void RollbackChanges()
        {
            if (_dbContext.Context.IsInTransaction)
            {
                _dbContext.Context.Rollback();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Starts transaction for event create, read or update query 
        /// only if transaction was not already started. You can use this
        /// if you want custom control over how transaction is being executed.
        /// For most of the scenarios out there, wrapping your db query in 
        /// _dbContext.Context.RunInTransaction would be enough.
        /// However, if this is what you really need, then make sure that you wrap the action in 
        /// try-catch block and then also manually call <see cref="SaveChanges"/> if everything goes alright
        /// or <see cref="RollbackChanges"/> if you catch an exception, so that transaction can be rolled-back.
        /// </summary>
        private void NegotiateTransaction()
        {
            if (_dbContext.Context.IsInTransaction == false)
            {
                _dbContext.Context.BeginTransaction();
            }
        }


        #endregion
    }
}

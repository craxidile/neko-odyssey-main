using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Domain
{
    public abstract class ReadOnlyRepository<TEntity, TId, TDbContext> : IReadOnlyRepository<TEntity, TId>
           where TEntity : EntityBase<TId>, IAggregateRoot
           where TId : struct
           where TDbContext : SQLiteConnection

    {
        #region Fields

        protected readonly IDbContext<TDbContext> _dbContext;

        protected string _connectionString => _dbContext.Context.DatabasePath;

        #endregion

        #region Constructors

        protected ReadOnlyRepository(IDbContext<TDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region SpatiumInteractive.Libraries.Unity.GRU.Domain.IReadOnlyRepository

        /// <inheritdoc />
        public virtual TEntity Get(TId id)
        {
            return Get(id, null);
        }

        /// <inheritdoc />
        public virtual TEntity Get(TId id, QueryExpression<TEntity> queryMetadata)
        {
            if (queryMetadata != null && queryMetadata.WhereExpression.Any())
            {
                throw new ArgumentException("where expression is not allowed in SpatiumInteractive.Librarires.Unity.GRU.ReadOnlyRepository.Get(TId id, QueryExpression<TEntity> queryMetadata) method as its meant to fetch entity by Id only. Rather use Find() method if you want to fetch entity by some other parameter(s)");
            }

            TEntity entity;

            if (queryMetadata != null && queryMetadata.SelectExpression != null)
            {
                entity = _dbContext.Context.TableAbstract<TEntity>()
                                    .Select(queryMetadata.SelectExpression, queryMetadata.ShouldDistinct)
                                    .Where(x => x.Id.Equals(id))
                                    .FirstOrDefault();
            }
            else
            {
                entity = _dbContext.Context.FindAbstract<TEntity>(id);
            }

            return entity;
        }

        /// <inheritdoc />
        public virtual TEntity Get(QueryExpression<TEntity> queryMetadata)
        {
            var entities = Find(queryMetadata);
            return entities.FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<TEntity> GetAll()
        {
            return GetAll(null);
        }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<TEntity> GetAll(QueryExpression<TEntity> queryMetadata)
        {
            if (queryMetadata != null && queryMetadata.WhereExpression.Any())
            {
                throw new ArgumentException("where expression is not allowed in SpatiumInteractive.Librarires.Unity.GRU.GetAll(QueryExpression<TEntity> queryMetadata) method. Use FindAllBy method if you want to query with special where condition.");
            }

            var dynamicQuery = _dbContext.Context.TableAbstract<TEntity>();

            dynamicQuery = AppendSelectExpressions(dynamicQuery, queryMetadata);
            dynamicQuery = AppendOrderExpressions(dynamicQuery, queryMetadata);

            var dynamicQueryResult = dynamicQuery.ToList();

            return dynamicQueryResult.ToReadOnlyCollection();
        }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<TEntity> Find(QueryExpression<TEntity> queryMetadata)
        {
            var dynamicQuery = _dbContext.Context.TableAbstract<TEntity>();

            if (queryMetadata != null && queryMetadata.SelectExpression != null)
            {
                dynamicQuery.Select(queryMetadata.SelectExpression, queryMetadata.ShouldDistinct);
            }

            dynamicQuery = AppendWhereExpressions(dynamicQuery, queryMetadata, WhereOrder.BeforeSelect);
            dynamicQuery = AppendWhereExpressions(dynamicQuery, queryMetadata, WhereOrder.AfterSelect);
            dynamicQuery = AppendOrderExpressions(dynamicQuery, queryMetadata);

            var dynamicQueryResult = dynamicQuery.ToList();

            return dynamicQueryResult.ToReadOnlyCollection();
        }

        /// <inheritdoc/>
        public virtual TEntity FindBy(Expression<Func<TEntity, bool>> predicate, params string[] includeItems)
        {
            var query = _dbContext.Context.TableAbstract<TEntity>().Where(predicate);

            var entity = query.FirstOrDefault();

            if (includeItems != null && includeItems.Any())
            {
                throw new NotSupportedException("Current version of GRU is not fully suporting including properties as strings in in IReadOnlyRepository.FindBy method. This feature will get implemented in future versions, until then, you can use include expression directly over an entity (e.g. entity.Inlcude(x=>x.someProp, _repoReference)... Refer to GRU's documentation if you need more info.");
            }

            return entity;
        }

        #endregion

        #region SpatiumInteractive.Libraries.Unity.GRU.Contracts.IReadOnlyRepositoryBase

        public virtual TEntity FindBy(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, EntityBase<TId>>>[] includeExpressions)
        {
            var query = _dbContext.Context.TableAbstract<TEntity>().Where(predicate);

            var entity = query.FirstOrDefault();

            if (includeExpressions != null && includeExpressions.Any())
            {
                throw new NotSupportedException("Current version of GRU is not fully suporting usage of include expressions in IReadOnlyRepositoryBase.FindBy method. This feature will get implemented in future versions, until then, you can use include expression directly over an entity (e.g. entity.Inlcude(x=>x.someProp, _repoReference)... Refer to GRU's documentation if you need more info.");
            }

            return entity;
        }

        public virtual IEnumerable<TEntity> FindAll(params Expression<Func<TEntity, EntityBase<TId>>>[] includeExpressions)
        {
            var query = _dbContext.Context.TableAbstract<TEntity>();

            if (includeExpressions != null && includeExpressions.Any())
            {
                throw new NotSupportedException("Current version of GRU is not fully suporting usage of include expressions in FindAll method. This feature will get implemented in future versions, until then, you can use include expression directly over an entity (e.g. entity.Inlcude(x=>x.someProp, _repoReference)... Refer to GRU's documentation if you need more info.");
            }

            return query.ToList();
        }

        public virtual IEnumerable<TEntity> FindAllBy(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, EntityBase<TId>>>[] includeExpressions)
        {
            var query = _dbContext.Context.TableAbstract<TEntity>().Where(predicate);
            
            var entities = query.ToList();

            if (includeExpressions != null && includeExpressions.Any())
            {
                throw new NotSupportedException("Current version of GRU is not fully suporting usage of include expressions in FindAllBy method. This feature will get implemented in future versions, until then, you can use include expression directly over an entity (e.g. entity.Inlcude(x=>x.someProp, _repoReference)... Refer to GRU's documentation if you need more info.");
            }

            return entities;
        }

        #endregion

        #region Private methods

        private TableQuery<TEntity> AppendSelectExpressions(TableQuery<TEntity> dynamicQuery, QueryExpression<TEntity> queryMetadata)
        {
            if (queryMetadata == null) return dynamicQuery;

            var selectExpr = queryMetadata.SelectExpression;
            bool shouldDistinct = queryMetadata.ShouldDistinct;

            dynamicQuery = dynamicQuery.Select(selectExpr, shouldDistinct);

            return dynamicQuery;
        }

        private TableQuery<TEntity> AppendWhereExpressions(TableQuery<TEntity> dynamicQuery, QueryExpression<TEntity> queryMetadata, WhereOrder whereOrder)
        {
            if (queryMetadata == null) return dynamicQuery;

            var expressions = queryMetadata.WhereExpression.Where(x => x.WhereOrder == whereOrder);

            if (expressions.Any())
            {
                foreach (var whereExpression in expressions)
                {
                    dynamicQuery = dynamicQuery.Where(whereExpression.Expression);
                }
            }

            return dynamicQuery;
        }

        private TableQuery<TEntity> AppendOrderExpressions(TableQuery<TEntity> dynamicQuery, QueryExpression<TEntity> queryMetadata)
        {
            if (queryMetadata == null) return dynamicQuery;

            if (queryMetadata.OrderExpressions.Any())
            {
                foreach (var expression in queryMetadata.OrderExpressions)
                {
                    if (expression.Direction == OrderDirection.Ascending)
                    {
                        dynamicQuery = dynamicQuery.OrderBy(expression.Property, expression.IgnoreCasing);
                    }
                    else
                    {
                        dynamicQuery = dynamicQuery.OrderByDescending(expression.Property, expression.IgnoreCasing);
                    }
                }
            }

            return dynamicQuery;
        }
 
        #endregion
    }
}

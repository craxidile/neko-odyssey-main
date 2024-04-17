using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;
using SQLite4Unity3d;

namespace SpatiumInteractive.Libraries.Unity.GRU.Extensions
{
    public static class EntityBaseExtension
    {
        /// <summary>
        /// Execute multiple different include expressions over
        /// a single <i>TEntity</i>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="entity"></param>
        /// <param name="includeExpressions"></param>
        /// <param name="repo"></param>
        public static void IncludeAll<TEntity, TId, TProperty, TDbContext>
        (
            this TEntity entity,
            IList<Expression<Func<TEntity, TProperty>>> includeExpressions,
            ReadOnlyRepository<TProperty, TId, TDbContext> repo
        )
           where TEntity : EntityBase<TId>, IAggregateRoot
           where TProperty: EntityBase<TId>, IAggregateRoot
           where TDbContext : SQLiteConnection
           where TId : struct
        {
            foreach (var includeExpr in includeExpressions)
            {
                entity.Include(includeExpr, repo);
            }
        }

        /// <summary>
        /// Include a complex entity property using lambda expression
        /// and it will automatically be fetched and filled-in 
        /// from the database
        /// </summary>
        /// <typeparam name="TEntity">source object's type (entity base of some sort)</typeparam>
        /// <typeparam name="TProperty">type of property to include (entity base of some sort)</typeparam>
        /// <param name="source"></param>
        /// <param name="navigationPropertyPath">include expression, selected prop must be entity base of some sort</param>
        /// <param name="repo">corresponding repo (data source). Must correspond to tprop type.</param>
        /// <returns>
        ///     e.g. <i>book.<u>Include(x => x.Author</u>, _authorRepository);</i>
        ///     <br/>
        ///     would fill-in book's <i><u>Author</u></i> property with its author
        /// </returns>
        public static IIncludableQueryable<TEntity, TProperty, TId> Include<TEntity, TProperty, TId>
        (
            this TEntity source,
            Expression<Func<TEntity, TProperty>> navigationPropertyPath,
            IReadOnlyRepository<TProperty, TId> repo
        )
             where TEntity : EntityBase<TId>, IAggregateRoot
             where TId : struct
             where TProperty : EntityBase<TId>, IAggregateRoot
        {

            var memberSelected = (navigationPropertyPath.Body as MemberExpression).Member;
            string fkPropName = memberSelected.Name + Orm.ImplicitPkName;

            TId id = (TId)source.GetPropValue(fkPropName);
            var propertyEntity = repo.Get(id);

            source.SetPropValue(memberSelected.Name, propertyEntity);
            var entityIncluded = new IncludableQueryable<TEntity, TProperty, TId>(source, propertyEntity);

            return entityIncluded;
        }

        /// <summary>
        /// Include list property of your choice using lambda expression and
        /// it will automatically be fetched and filled-in from the database.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="navigationPropertyPath"></param>
        /// <param name="repo"></param>
        /// <returns>
        /// e.g. <i>author.<u>Include(x => x.Books</u>, _bookRepository)</i>
        /// <br/>
        /// would fill-in author's <i><u>Books</u></i> property with all his books
        /// </returns>
        public static IIncludableQueryable<TEntity, TProperty, TId> Include<TEntity, TProperty, TId>
        (
             this TEntity source,
             Expression<Func<TEntity, IEnumerable<TProperty>>> navigationPropertyPath,
             IReadOnlyRepository<TProperty, TId> repo
        )
             where TEntity : EntityBase<TId>, IAggregateRoot
             where TId : struct
             where TProperty : EntityBase<TId>, IAggregateRoot
        {
            var memberSelected = (navigationPropertyPath.Body as MemberExpression).Member;
            var query = new QueryExpression<TProperty>();
            string fkPropName = source.GetType().Name + Orm.ImplicitPkName;

            ParameterExpression x = Expression.Parameter(typeof(TProperty), "x");
            var xFKProperty = Expression.Property(x, fkPropName);
            Expression body = Expression.Equal(xFKProperty, Expression.Constant(source.Id));
            var lambda = Expression.Lambda<Func<TProperty, bool>>(body, x);

            query.AddWhereExpression(lambda);
            var propertyEntities = repo.Find(query);

            source.SetPropValue(memberSelected.Name, propertyEntities);
            var entityIncluded = new IncludableQueryable<TEntity, TProperty, TId>(source, propertyEntities);

            return entityIncluded;
        }

        /// <summary>
        /// Continue with your previous include call and now
        /// include another <i>TProperty</i> of your choice.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPreviousProperty"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="navigationPropertyPath"></param>
        /// <param name="repo"></param>
        /// <returns>
        /// e.g. <i>author.Include(x => x.Books).<u>ThenInclude(x => x.Author)</u>;</i>
        /// <br/>
        /// Will fill-in an author's <i>Books</i> prop with all his books and 
        /// each <i>Book</i>, from that <i>Books</i>  property, 
        /// wil have its <i><u>Author</u></i> property set as well
        /// </returns>
        public static IIncludableQueryable<TEntity, TProperty, TId> ThenInclude<TEntity, TPreviousProperty, TProperty, TId>
        (
            this IIncludableQueryable<TEntity, TPreviousProperty, TId> source,
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath,
            IReadOnlyRepository<TProperty, TId> repo
        )
            where TEntity : EntityBase<TId>, IAggregateRoot
            where TId: struct
            where TProperty : EntityBase<TId>, IAggregateRoot
            where TPreviousProperty : EntityBase<TId>, IAggregateRoot
        {
            IIncludableQueryable<TEntity, TProperty, TId> entityIncluded;
            bool isPreviousPropList = source.ListProperty != null;

            if (isPreviousPropList)
            {
                List<TProperty> oldCollectionUpdated = new List<TProperty>() { };
                foreach (var tPreviousProp in source.ListProperty)
                {
                    var previousPropUpdated = tPreviousProp.Include(navigationPropertyPath, repo);
                    oldCollectionUpdated.Add(previousPropUpdated.Property);
                }
                entityIncluded = new IncludableQueryable<TEntity, TProperty, TId>(source.Source, oldCollectionUpdated);
            }
            else
            {
                var includeableEntity = source.Property.Include(navigationPropertyPath, repo);
                entityIncluded = new IncludableQueryable<TEntity, TProperty, TId>(source.Source, includeableEntity.Property);
            }

            return entityIncluded;
        }

        /// <summary>
        /// Continue with your previous include call and now include 
        /// list property of type <i>TProperty</i>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPreviousProperty"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="navigationPropertyPath">complex list property of type TProperty set to be included </param>
        /// <param name="repo"></param>
        /// <returns>
        /// //e.g. <i>author.Include(x => x.Colleague).<u>ThenInclude(x => x.Books)</u></i>
        /// <br/>
        /// would fill-in author's <i>Colleague</i> property and then it would fill-in Colleague's <i><u>Books</u></i> list with all the books that Colleague has.
        /// </returns>
        public static IIncludableQueryable<TEntity, TProperty, TId> ThenInclude<TEntity, TPreviousProperty, TProperty, TId>
        (
            this IIncludableQueryable<TEntity, TPreviousProperty, TId> source,
            Expression<Func<TPreviousProperty, IEnumerable<TProperty>>> navigationPropertyPath,
            IReadOnlyRepository<TProperty, TId> repo
        )
            where TEntity : EntityBase<TId>, IAggregateRoot
            where TProperty : EntityBase<TId>, IAggregateRoot
            where TId: struct
            where TPreviousProperty : EntityBase<TId>, IAggregateRoot
        {
            IIncludableQueryable<TEntity, TProperty, TId> entityIncluded;
            bool isPreviousPropList = source.ListProperty != null;
            if (isPreviousPropList)
            {
                List<TProperty> oldCollectionUpdated = new List<TProperty>() { };
                foreach (var tPreviousProp in source.ListProperty)
                {
                    var previousPropUpdated = tPreviousProp.Include(navigationPropertyPath, repo);
                    oldCollectionUpdated.AddRange(previousPropUpdated.ListProperty);
                }
                entityIncluded = new IncludableQueryable<TEntity, TProperty, TId>(source.Source, oldCollectionUpdated);
            }
            else
            {
                var includeableEntity = source.Property.Include(navigationPropertyPath, repo);
                entityIncluded = new IncludableQueryable<TEntity, TProperty, TId>(source.Source, includeableEntity.ListProperty);
            }

            return entityIncluded;
        }
    }
}

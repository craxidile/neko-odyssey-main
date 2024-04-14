using System.Linq;
using System.Collections.Generic;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using Assets.Scripts.Integration.GRU.DbContexts;
using Database.Repository;

namespace Assets.Scripts.Integration.GRU.Repositories
{
    public class AuthorRepository : Repository<Author, int, BookshopexampledbDbContext>, IAuthorRepository
    {
        public AuthorRepository(IDbContext<BookshopexampledbDbContext> dbContext) : base(dbContext)
        {
        }

        public List<Author> TestGetAllOrderedDescdendingByName()
        {
            var query = new QueryExpression<Author>();
            query.AddOrderExpression(x => x.Name, OrderDirection.Descending);
            var entities = GetAll(query);
            return entities.ToList();
        }

        public List<Author> TestGetAllAuthorsWithIdAndNameSelectedOnly()
        {
            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Name = x.Name,
                Id = x.Id,
            });
            var entities = GetAll(query);
            return entities.ToList();
        }

        public List<Author> TestGetAllWhereNameStartsWithCharacter(char character)
        {
            var query = new QueryExpression<Author>();
            var startsWith = character.ToString();
            var entities = FindAllBy(x => x.Name.StartsWith(startsWith));
            return entities.ToList();
        }

        public Author TestGetAuthorWithIdAndNameSelectedOnly(int id)
        {
            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
                Name = x.Name,
            });
            var entity = Get(id, query);
            return entity;
        }

        public List<Author> GetAllDistinctByName()
        {
            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author() { Name = x.Name }, shouldDistinct: true);
            var entities = GetAll(query);
            return entities.ToList();
        }

        public int GetTotalCount()
        {
            var totalCount = _dbContext.Context.TableAbstract<Author>().Count();
            return totalCount;
        }

        public Author GetAuthorWithIdRawSelected(int id)
        {
            var entity = _dbContext.Context
                            .Table<Author>()
                            .Select(x => x.Id)
                            .Where(x => x.Id == id)
                            .FirstOrDefault();
            return entity;
        }

        public Author GetAuthorWithNameRawSelected(int id)
        {
            var entity = _dbContext.Context
                      .Table<Author>()
                      .Select(x => x.Name)
                      .Where(x => x.Id == id)
                      .FirstOrDefault();
            return entity;
        }


        public Author GetAuthorWithBirthdayRawSelected(int id)
        {
            var entity = _dbContext.Context
                      .Table<Author>()
                      .Select(x => x.Birthday)
                      .Where(x => x.Id == id)
                      .FirstOrDefault();
            return entity;
        }

        public Author GetAuthorWithIdAndNameRawSelected(int id)
        {
            var entity = _dbContext.Context
                      .Table<Author>()
                      .Select(x => new Author(x.Id, x.Name))
                      .Where(x => x.Id == id)
                      .FirstOrDefault();
            return entity;
        }

        public Author GetAuthorWithIdAndNameAndBirthdayRawSelected(int id)
        {
            var entity = _dbContext.Context
                          .Table<Author>()
                          // as you can see, selecting using both:
                          //    -direct constructor calls (like in GetAuthorWithIdAndNameRawSelected() above)  and
                          //    -selecting props manually in body ({..}) like here
                          // are supported, so that you can use what suits you better
                          .Select(x => new Author() { Id = x.Id, Name = x.Name, Birthday = x.Birthday })
                          .Where(x => x.Id == id)
                          .FirstOrDefault();
            return entity;
        }

        public Author GetAuthorWithAllPropsRawSelected(int id)
        {
            Author entity = _dbContext.Context
              .Table<Author>()
              .Select(x => new Author()
              {
                  Id = x.Id,
                  Name = x.Name,
                  City = x.City,
                  Birthday = x.Birthday,
                  CreatedAt = x.CreatedAt,
                  UpdatedAt = x.UpdatedAt,
                  IsActive = x.IsActive,
                  IsDeleted = x.IsDeleted
              })
              .Where(x => x.Id == id)
              .FirstOrDefault();
            return entity;
        }

        public List<Author> GetAuthorsBirthdayRawDistinctSelected()
        {
            var entities = _dbContext.Context
               .Table<Author>()
               .Select(x => new Author() { Birthday = x.Birthday }, shouldDistinct: true)
               .ToList();
            return entities;
        }

        public List<Author> GetAuthorsNameRawDistinctSelected()
        {
            var entities = _dbContext.Context
                .Table<Author>()
                .Select(x => new Author() { Name = x.Name }, shouldDistinct: true)
                .ToList();
            return entities;
        }

        public List<Author> GetAuthorsRawWhereIdGreaterThanN(int n)
        {
            var entitites = _dbContext.Context
                                .Table<Author>()
                                .Where(x => x.Id > n)
                                .ToList();
            return entitites;
        }

        public List<Author> GetAuthorsRawWhereIdLessThanN(int n)
        {
            var entitites = _dbContext.Context
                                .Table<Author>()
                                .Where(x => x.Id < n)
                                .ToList();
            return entitites;
        }

        public Author GetAuthorRawWhereIdEqualToN(int n)
        {
            var entity = _dbContext.Context
                                .Table<Author>()
                                .Where(x => x.Id.Equals(n))
                                .SingleOrDefault();
            return entity;
        }

        public List<Author> GetAuthorsRawWhereIdEqualToNOrLessThanY(int n, int y)
        {
            var entities = _dbContext.Context
                                .Table<Author>()
                                .Where(x => x.Id.Equals(n) || x.Id < y)
                                .ToList();
            return entities;
        }

        public List<Author> GetAuthorsRawWhereNameStartsWith(string str)
        {
            var entities = _dbContext.Context
                                .Table<Author>()
                                .Where(x => x.Name.StartsWith(str))
                                .ToList();
            return entities;
        }
    }
}
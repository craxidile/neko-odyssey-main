using System.Linq;
using System.Collections.Generic;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using Assets.Scripts.Integration.GRU.DbContexts;
using Database.Repository;

namespace Assets.Scripts.Integration.GRU.Repositories
{
	public class BookRepository : Repository<Book, int, BookshopexampledbDbContext>, IBookRepository
	{
		public BookRepository(IDbContext<BookshopexampledbDbContext> dbContext) : base(dbContext)
		{
		}

        public List<Book> GetAllBooksOfAuthor(int authorId)
        {
			var entities = _dbContext.Context
										.Table<Book>()
										.Where(x => x.AuthorId == authorId)
										.ToList();
			return entities;
        }

        public List<Book> GetAllBooksSortedDescByTitle()
        {
			var entities = _dbContext.Context
										.Table<Book>()
										.OrderByDescending(x => x.Title)
										.ToList();
			return entities;
        }

		public List<Book> GetAllBooksSortedAscByTitle()
		{
			var entities = _dbContext.Context
										.Table<Book>()
										.OrderBy(x => x.Title)
										.ToList();
			return entities;
		}
	}
}


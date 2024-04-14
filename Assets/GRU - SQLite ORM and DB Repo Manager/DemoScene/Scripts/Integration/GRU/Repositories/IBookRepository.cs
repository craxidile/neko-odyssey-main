using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using Database.Repository;
using System.Collections.Generic;

namespace Assets.Scripts.Integration.GRU.Repositories
{
	public interface IBookRepository : IRepository<Book, int>
	{
		List<Book> GetAllBooksOfAuthor(int authorId);

		List<Book> GetAllBooksSortedDescByTitle();
	
		List<Book> GetAllBooksSortedAscByTitle();
	}
}


using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using Database.Repository;
using System.Collections.Generic;

namespace Assets.Scripts.Integration.GRU.Repositories
{
	public interface IAuthorRepository : IRepository<Author, int>
	{
		public List<Author> TestGetAllOrderedDescdendingByName();

		public List<Author> TestGetAllAuthorsWithIdAndNameSelectedOnly();

		public List<Author> TestGetAllWhereNameStartsWithCharacter(char character);

		public Author TestGetAuthorWithIdAndNameSelectedOnly(int id);

		public List<Author> GetAllDistinctByName();

		public int GetTotalCount();

		public Author GetAuthorWithIdRawSelected(int id);

		public Author GetAuthorWithNameRawSelected(int id);

		public Author GetAuthorWithBirthdayRawSelected(int id);

		public Author GetAuthorWithIdAndNameRawSelected(int id);

		public Author GetAuthorWithIdAndNameAndBirthdayRawSelected(int id);

		public Author GetAuthorWithAllPropsRawSelected(int id);

		public List<Author> GetAuthorsBirthdayRawDistinctSelected();

		public List<Author> GetAuthorsNameRawDistinctSelected();

		public List<Author> GetAuthorsRawWhereIdGreaterThanN(int n);

		public List<Author> GetAuthorsRawWhereIdLessThanN(int n);

		public Author GetAuthorRawWhereIdEqualToN(int n);

		public List<Author> GetAuthorsRawWhereIdEqualToNOrLessThanY(int n, int y);

		public List<Author> GetAuthorsRawWhereNameStartsWith(string str);
	}
}


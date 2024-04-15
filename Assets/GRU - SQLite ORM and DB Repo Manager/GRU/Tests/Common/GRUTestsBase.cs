using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using Database.Repository;
using Assets.Scripts.Integration.GRU.DbContexts;
using Assets.Scripts.Integration.GRU.Repositories;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public abstract class GRUTestsBase
    {
        #region Constants

        protected const string EXAMPLE_BOOKSHOP_DB_HANDLER_GO_NAME = "DbHandler_BookshopExampleDb1";

        #endregion

        #region Private Fields

        protected string[] _propertiesToIgnoreWhenComparing = new string[]
        {
            "Length",
            "Chars"
        };

        protected IAuthorRepository _authorRepository;
        protected IBookRepository _bookRepository;
        protected ICoauthorRepository _coauthorRepository;
        protected AuthorMock _authorMock;
        protected BookMock _bookMock;
        protected int _currentAuthorsCount;

        #endregion

        #region Setup & Configuration of Dependencies 

        [OneTimeSetUp]
        public void Initialize()
        {
            bool isCacheInit = GRUTestsCache<BookshopexampledbDbContext>.HasInitCache();

            if (!isCacheInit)
            {
                var _dbHandlerBookShopExampleDb = GameObject
                    .Find(EXAMPLE_BOOKSHOP_DB_HANDLER_GO_NAME).gameObject
                    .GetComponent<DBHandler>();
                var dbConnection = _dbHandlerBookShopExampleDb.GetConnection();
                var dbContext = new BookshopexampledbDbContext(dbConnection);

                GRUTestsCache<BookshopexampledbDbContext>.InitCache
                (
                    _dbHandlerBookShopExampleDb,
                    dbConnection,
                    dbContext
                );
            }

            var dbContextCached = GRUTestsCache<BookshopexampledbDbContext>.GetDbContext();
            _authorRepository = new AuthorRepository(dbContextCached);
            _bookRepository = new BookRepository(dbContextCached);
            _coauthorRepository = new CoauthorRepository(dbContextCached);

            _authorMock = new AuthorMock();
            _bookMock = new BookMock();
        }

        [SetUp]
        public void PreTestInit()
        {
            var dbConnection = GRUTestsCache<BookshopexampledbDbContext>.GetDbConnection();
            dbConnection.Commit();

            _currentAuthorsCount = _authorRepository.GetAll().Count;
        }

        #endregion

        #region Protected Methods

        protected virtual Coauthor CreateNewCoauthor(Book book)
        {
            var author = CreateNewAuthor();
            var coauthor = new Coauthor(author.Id, book.Id);
            coauthor = _coauthorRepository.Add(coauthor);
            return coauthor;
        }

        protected virtual Author CreateNewAuthor(NameFormat nameFormat = NameFormat.Default, bool addColleague = false)
        {
            int? colleagueId = addColleague ? _authorRepository.GetAll().First().Id : null;
            var author = _authorMock.GetValidAuthorForCreate(colleagueId);

            switch (nameFormat)
            {
                case NameFormat.Default:
                    break;
                case NameFormat.AllUpper:
                    author.Name = author.Name.ToUpper();
                    break;
                case NameFormat.AllLower:
                    author.Name = author.Name.ToLower();
                    break;
                case NameFormat.WrapedWithSpaces:
                    author.Name = author.Name.WrapInSpaces();
                    break;
                default:
                    break;
            }

            var newEntity = _authorRepository.Add(author);
            return newEntity;
        }

        protected virtual List<Author> CreateNewAuthors(int count)
        {
            var authors = new List<Author>();
            for (int i = 0; i < count; i++)
            {
                var author = CreateNewAuthor();
                authors.Add(author);
            }
            return authors;
        }

        protected virtual Book CreateNewBook(NameFormat nameFormat = NameFormat.Default, Author author = null)
        {
            author = author ?? _authorRepository.GetAll().First();
            var book = _bookMock.GetValidBookForCreate(author.Id);

            switch (nameFormat)
            {
                case NameFormat.Default:
                    break;
                case NameFormat.AllUpper:
                    book.Title = book.Title.ToUpper();
                    break;
                case NameFormat.AllLower:
                    book.Title = book.Title.ToLower();
                    break;
                case NameFormat.WrapedWithSpaces:
                    book.Title = book.Title.WrapInSpaces();
                    break;
                default:
                    break;
            }

            var newEntity = _bookRepository.Add(book);
            return newEntity;
        }

        public Book GetBookWithNonExistingAuthor()
        {
            return _bookMock.GetBookWithNonExistingAuthor();
        }

        public int GetRandomAuthorId()
        {
            var allAuthors = _authorRepository.GetAll().ToList();
            var randomAuthor = allAuthors[Random.Range(0, allAuthors.Count - 1)];
            return randomAuthor.Id;
        }

        #endregion
    }
}

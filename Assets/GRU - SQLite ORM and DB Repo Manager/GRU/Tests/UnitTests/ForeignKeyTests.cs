using System.Linq;
using Database.Repository;
using NUnit.Framework;
using SQLite4Unity3d;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class ForeignKeyTests : GRUTestsBase
    {
        [Test]
        public void AddBookWithNonExistingAuthorId_RaisesFKError()
        {
            var book = GetBookWithNonExistingAuthor();

            Assert.Throws<SQLiteException>(() => _bookRepository.Add(book));
        }

        [Test]
        public void AddBookWithExistingAuthorId_AddsBook()
        {
            Book entity = null;
            Assert.DoesNotThrow(() => entity = CreateNewBook());
            Assert.NotNull(entity);
            Assert.NotZero(entity.Id);
        }

        [Test]
        public void UpdateBookWithNonExistingAuthorId_RaisesFKError()
        {
            var book = _bookRepository.GetAll().Last();
            book.AuthorId = -1;
            Assert.Throws<SQLiteException>(() => _bookRepository.Update(book));
        }

        [Test]
        public void UpdateBookWithExistingAuthorId_UpdatesBook()
        {
            var book = _bookRepository.GetAll().Last();
            book.AuthorId = GetRandomAuthorId();
            Assert.DoesNotThrow(() => _bookRepository.Update(book));

            var entity = _bookRepository.Get(book.Id);
            Assert.IsTrue(entity.AuthorId == book.AuthorId);
        }

        [Test]
        public void RemoveAuthorReferencedToABook_FailsToRemoveAuthor()
        {
            var book = _bookRepository.GetAll().Last();
            var author = _authorRepository.Get(book.AuthorId);
            Assert.Throws<SQLiteException>(() => _authorRepository.Remove(author));
        }

        [Test]
        public void RemoveBook_RemovesBookButKeepsAuthor()
        {
            var author = CreateNewAuthor();
            var book = CreateNewBook(author: author);

            _bookRepository.Remove(book);

            var entityBook = _bookRepository.Get(book.Id);
            var entityAuthor = _authorRepository.Get(author.Id);

            Assert.IsNull(entityBook);
            Assert.IsNotNull(entityAuthor);
        }
    }
}

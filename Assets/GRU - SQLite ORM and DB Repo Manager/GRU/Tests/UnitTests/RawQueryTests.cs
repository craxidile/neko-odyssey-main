using System.Linq;
using Database.Repository;
using NUnit.Framework;
using SpatiumInteractive.Libraries.Unity.GRU.Tests.Extension;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class RawQueryTests : GRUTestsBase
    {
        #region Raw Select Tests

        [Test]
        public void SelectAuthorId_ReturnsAuthorOnlyWithIdPropSet()
        {
            var newEntity = CreateNewAuthor();

            var entity = _authorRepository.GetAuthorWithIdRawSelected(newEntity.Id);

            bool isOnlyIDSelected = entity.Id.IsSelectedInDb() &&
                                    entity.Name.IsNotSelectedInDb() &&
                                    entity.City.IsNotSelectedInDb() &&
                                    entity.Birthday.IsNotSelectedInDb() &&
                                    entity.CreatedAt.IsNotSelectedInDb();

            Assert.IsTrue(isOnlyIDSelected);
        }

        [Test]
        public void SelectAuthorName_ReturnsAuthorOnlyWithNamePropSet()
        {
            var newEntity = CreateNewAuthor();

            var entity = _authorRepository.GetAuthorWithNameRawSelected(newEntity.Id);

            bool isOnlyNameSelected = entity.Id.IsNotSelectedInDb() &&
                                    entity.Name.IsSelectedInDb() &&
                                    entity.City.IsNotSelectedInDb() &&
                                    entity.Birthday.IsNotSelectedInDb() &&
                                    entity.CreatedAt.IsNotSelectedInDb();

            Assert.IsTrue(isOnlyNameSelected);
        }

        [Test]
        public void SelectAuthorBirthday_ReturnsAuthorOnlyWithBirthdayPropSet()
        {
            var newEntity = CreateNewAuthor();

            var entity = _authorRepository.GetAuthorWithBirthdayRawSelected(newEntity.Id);

            bool isOnlyBirthdaySelected = entity.Id.IsNotSelectedInDb() &&
                                    entity.Name.IsNotSelectedInDb() &&
                                    entity.City.IsNotSelectedInDb() &&
                                    entity.Birthday.IsSelectedInDb() &&
                                    entity.CreatedAt.IsNotSelectedInDb();

            Assert.IsTrue(isOnlyBirthdaySelected);
        }

        [Test]
        public void SelectAuthorIdAndName_ReturnsAuthorOnlyWithIdAndNameSelected()
        {
            var newEntity = CreateNewAuthor();

            var entity = _authorRepository.GetAuthorWithIdAndNameRawSelected(newEntity.Id);

            bool areOnlyIDAndNameSelected = entity.Id.IsSelectedInDb() &&
                                    entity.Name.IsSelectedInDb() &&
                                    entity.City.IsNotSelectedInDb() &&
                                    entity.Birthday.IsNotSelectedInDb() &&
                                    entity.CreatedAt.IsNotSelectedInDb();

            Assert.IsTrue(areOnlyIDAndNameSelected);
        }

        [Test]
        public void SelectAuthorIdAndNameAndBirthday_ReturnsAuthorOnlyWithIdAndNameAndBirthdaySelected()
        {
            var newEntity = CreateNewAuthor();

            var entity = _authorRepository.GetAuthorWithIdAndNameAndBirthdayRawSelected(newEntity.Id);

            bool areOnlyIDAndNameAndBirthdaySelected = entity.Id.IsSelectedInDb() &&
                                    entity.Name.IsSelectedInDb() &&
                                    entity.City.IsNotSelectedInDb() &&
                                    entity.Birthday.IsSelectedInDb() &&
                                    entity.CreatedAt.IsNotSelectedInDb();

            Assert.IsTrue(areOnlyIDAndNameAndBirthdaySelected);
        }

        [Test]
        public void SelectAllAuthorProps_ReturnsAuthorWithAllPropsSelected()
        {
            var newEntity = CreateNewAuthor();

            var entityToUpdate = _authorMock.DeepCloneAuthorAndUpdateName(newEntity);
            var updatedEntity = _authorRepository.Update(entityToUpdate);


            var entity = _authorRepository.GetAuthorWithAllPropsRawSelected(updatedEntity.Id);

            bool areAllPropsSelected = entity.Id.IsSelectedInDb() &&
                                    entity.Name.IsSelectedInDb() &&
                                    entity.City.IsSelectedInDb() &&
                                    entity.Birthday.IsSelectedInDb() &&
                                    entity.CreatedAt.IsSelectedInDb() &&
                                    entity.UpdatedAt.IsSelectedInDb() &&
                                    entity.IsActive.IsSelectedInDb(expectedValue: true) &&
                                    entity.IsDeleted.IsSelectedInDb(expectedValue: false);

            Assert.IsTrue(areAllPropsSelected);
        }

        [Test]
        public void SelectDistinctAuthorBirthday_ReturnsAuthorOnlyWithBirthdayPropDistinctSelected()
        {
            var entities = _authorRepository.GetAuthorsBirthdayRawDistinctSelected();

            bool allUnique = entities.GroupBy(x => x.Birthday).All(g => g.Count() == 1);

            Assert.IsTrue(allUnique);
        }

        [Test]
        public void SelectDistinctAuthordName_ReturnsAuthorOnlyWithNameDistinctSelected()
        {
            var entities = _authorRepository.GetAuthorsNameRawDistinctSelected();
            bool allUnique = entities.GroupBy(x => x.Name).All(g => g.Count() == 1);
            Assert.IsTrue(allUnique);
        }

        #endregion

        #region Raw Where Tests

        [Test]
        public void WhereAuthorId_GreaterThanX_ReturnsAllAuthorsWithIdGreaterThanX([Values(2)] int nAuthorsToCreateAfterFirst)
        {
            var newEntity = CreateNewAuthor();
            var entitiesPostAdded = CreateNewAuthors(nAuthorsToCreateAfterFirst);

            var matches = _authorRepository.GetAuthorsRawWhereIdGreaterThanN(newEntity.Id);

            Assert.AreEqual(matches.Count, nAuthorsToCreateAfterFirst);
        }

        [Test]
        public void WhereAuthorId_LessThanX_ReturnsAllAuthorsWithIdLessThanX()
        {
            var entities = _authorRepository.GetAll();
            var idForCompare = entities.Last().Id;
            var matches = _authorRepository.GetAuthorsRawWhereIdLessThanN(idForCompare);

            Assert.AreEqual(matches.Count, entities.Count - 1);
        }

        [Test]
        public void WhereAuthorId_Equals_To_X_ReturnsSingleAuthorWithIdBeingEqualTo_X()
        {
            var newEntity = CreateNewAuthor();

            var match = _authorRepository.GetAuthorRawWhereIdEqualToN(newEntity.Id);

            Assert.IsNotNull(match);
        }

        [Test]
        public void WhereAuthorId_Equals_To_X_OrLessThan_Y_ReturnsMathcingAuthor()
        {
            var entities = _authorRepository.GetAll();
            var newEntity = CreateNewAuthor();

            var secondEntityId = entities.ToList()[1].Id;

            var match = _authorRepository.GetAuthorsRawWhereIdEqualToNOrLessThanY(newEntity.Id, secondEntityId);

            Assert.IsTrue(match.Count == 2);
        }

        [Test]
        public void WhereAuthorNameStartsWithSomething_ReturnsAuthorsThatMatchTheCondition()
        {
            var newEntity = CreateNewAuthor();
            string nameToCheck = newEntity.Name;
            var entities = _authorRepository.GetAll();
            int numOfEntitiesStartingWithTheSameName =
                entities.Count
                (
                    x => x.Name.StartsWith(nameToCheck, System.StringComparison.InvariantCultureIgnoreCase
                ));

            var matches = _authorRepository.GetAuthorsRawWhereNameStartsWith(nameToCheck);

            Assert.IsTrue(matches.Count == numOfEntitiesStartingWithTheSameName);
        }

        #endregion

        #region Raw Include / ThenInclude Tests

        [Test]
        public void IncludeColleagueInAuthorEntity_ReturnsAuthorEntityWithHisColleaguePopulated()
        {
            var newAuthor = CreateNewAuthor(addColleague: true);

            var author = _authorRepository.Get(newAuthor.Id);
            var authorColleague = _authorRepository.Get(newAuthor.ColleagueId.Value);

            author.Include(x => x.Colleague, _authorRepository);

            bool areTheSame = author.Colleague.IsEqualTo(authorColleague);

            Assert.IsNotNull(author.Colleague);
            Assert.IsTrue(areTheSame);
        }

        [Test]
        public void IncludeAuthorInBookEntity_ReturnsBookEntityWithItsAuthorPopulated()
        {
            var newAuthor = CreateNewAuthor();
            var newBook = CreateNewBook(author: newAuthor);

            var book = _bookRepository.Get(newBook.Id);
            var bookAuthor = _authorRepository.Get(book.AuthorId);

            book.Include(x => x.Author, _authorRepository);

            bool areTheSame = book.Author.IsEqualTo(bookAuthor);

            Assert.IsNotNull(book.Author);
            Assert.IsTrue(areTheSame);
        }

        [Test]
        public void IncludeAllBooksInAuthorEntity_ReturnsAuthorEntityWithAllHisBooksPopulated()
        {
            var newAuthor = CreateNewAuthor();
            var authorBook1 = CreateNewBook(author: newAuthor);
            var authorBook2 = CreateNewBook(author: newAuthor);
            var addedBooks = new Book[2] { authorBook1, authorBook2 };

            var author = _authorRepository.Get(newAuthor.Id);
            author.Include(x => x.Books, _bookRepository);

            Assert.IsNotNull(author.Books);

            bool hasIncludedAll = addedBooks.All(x => author.Books.Any(y => y.Id == x.Id));

            Assert.IsTrue(hasIncludedAll);
        }

        [Test]
        public void IncludeColleagueAndAllHisBooksInAuthorEntity_ReturnsAuthorEntityWithColleagueAndAllColleaguesBooksPopulated()
        {
            var newAuthor = CreateNewAuthor(addColleague: true);
            var colleague = _authorRepository.Get(newAuthor.ColleagueId.Value);

            var colleagueBook1 = CreateNewBook(author: colleague);
            var colleagueBook2 = CreateNewBook(author: colleague);
            var addedBooks = new Book[2] { colleagueBook1, colleagueBook2 };

            var author = _authorRepository.Get(newAuthor.Id);
            author
                .Include(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Books, _bookRepository);

            Assert.IsNotNull(author.Colleague);
            Assert.IsNotNull(author.Colleague.Books);

            bool isColleagueIncluded = author.Colleague.Id.IsEqualTo(colleague.Id);
            bool areBooksIncluded = addedBooks.All(x => author.Colleague.Books.Any(y => y.Id == x.Id));

            Assert.IsTrue(isColleagueIncluded);
            Assert.IsTrue(areBooksIncluded);
        }

        [Test]
        public void IncludeColleagueAndAllHisBooksWithHimselfAsAnAuthorIncluded_Returns_AuthorEntityWithColleagueAndAllHisBooksAlongWithHimselfAsAnAuthorPopulated()
        {
            var newAuthor = CreateNewAuthor(addColleague: true);
            var colleague = _authorRepository.Get(newAuthor.ColleagueId.Value);

            var colleagueBook1 = CreateNewBook(author: colleague);
            var colleagueBook2 = CreateNewBook(author: colleague);
            var addedBooks = new Book[2] { colleagueBook1, colleagueBook2 };

            var author = _authorRepository.Get(newAuthor.Id);
            author
                .Include(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Books, _bookRepository)
                .ThenInclude(x => x.Author, _authorRepository);

            Assert.IsNotNull(author.Colleague);
            Assert.IsNotNull(author.Colleague.Books);
            foreach (var book in author.Colleague.Books)
            {
                Assert.IsNotNull(book.Author);
            }

            bool isColleagueIncluded = author.Colleague.Id.IsEqualTo(colleague.Id);
            bool areBooksIncluded = addedBooks.All(x => author.Colleague.Books.Any(y => y.Id == x.Id));
            bool doesEachBookHaveAuthorPopulated = author.Colleague.Books.All(x => x.AuthorId == colleague.Id);

            Assert.IsTrue(isColleagueIncluded);
            Assert.IsTrue(areBooksIncluded);
            Assert.IsTrue(doesEachBookHaveAuthorPopulated);
        }

        [Test]
        public void IncludeColleagueAndAllHisBooksAndTheirCoauthorsAuthors_ReturnsAuthorEntityWithColleagueAndAllHisBooksAndTheirCoauthorsAuthorsPopulated()
        {
            var newAuthor = CreateNewAuthor(addColleague: true);
            var colleague = _authorRepository.Get(newAuthor.ColleagueId.Value);

            var colleagueBook1 = CreateNewBook(author: colleague);
            var colleagueBook2 = CreateNewBook(author: colleague);
            var addedBooks = new Book[2] { colleagueBook1, colleagueBook2 };

            var book1Coauthor1 = CreateNewCoauthor(colleagueBook1);
            var book1Coauthor2 = CreateNewCoauthor(colleagueBook1);
            var book2Coauthor1 = CreateNewCoauthor(colleagueBook2);
            var book2Coauthor2 = CreateNewCoauthor(colleagueBook2);
            var addedCoauthors = new Coauthor[4] { book1Coauthor1, book1Coauthor2, book2Coauthor1, book2Coauthor2 };

            var author = _authorRepository.Get(newAuthor.Id);

            author
                .Include(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Books, _bookRepository)
                .ThenInclude(x => x.Coauthors, _coauthorRepository)
                .ThenInclude(x => x.Author, _authorRepository);

            Assert.IsNotNull(author.Colleague);
            Assert.IsNotNull(author.Colleague.Books);
            foreach (var book in author.Colleague.Books)
            {
                Assert.IsNotNull(book.Coauthors);
                foreach (var coauthor in book.Coauthors)
                {
                    Assert.IsNotNull(coauthor.Author);
                }
            }

            bool isColleagueIncluded = author.Colleague.Id.IsEqualTo(colleague.Id);
            bool areBooksIncluded = addedBooks.All(x => author.Colleague.Books.Any(y => y.Id == x.Id));
            bool areCoauthorsAuthorsIncluded = addedCoauthors.All(x => author.Colleague.Books.Any(y => y.Coauthors.Any(z => z.Id == x.Id)));

            Assert.IsTrue(isColleagueIncluded);
            Assert.IsTrue(areBooksIncluded);
            Assert.IsTrue(areCoauthorsAuthorsIncluded);
        }


        [Test]
        public void IncludeColleagueAndAllHisBooksAndTheirCoauthorsBooks_ReturnsAuthorEntityWithColleagueAndAllHisBooksAndTheirCoauthorsBooksPopulated()
        {
            var newAuthor = CreateNewAuthor(addColleague: true);
            var colleague = _authorRepository.Get(newAuthor.ColleagueId.Value);

            var colleagueBook1 = CreateNewBook(author: colleague);
            var colleagueBook2 = CreateNewBook(author: colleague);
            var addedBooks = new Book[2] { colleagueBook1, colleagueBook2 };

            var book1Coauthor1 = CreateNewCoauthor(colleagueBook1);
            var book1Coauthor2 = CreateNewCoauthor(colleagueBook1);
            var book2Coauthor1 = CreateNewCoauthor(colleagueBook2);
            var book2Coauthor2 = CreateNewCoauthor(colleagueBook2);
            var addedCoauthors = new Coauthor[4] { book1Coauthor1, book1Coauthor2, book2Coauthor1, book2Coauthor2 };

            var author = _authorRepository.Get(newAuthor.Id);

            author.Include(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Books, _bookRepository)
                .ThenInclude(x => x.Coauthors, _coauthorRepository)
                .ThenInclude(x => x.Book, _bookRepository);

            Assert.IsNotNull(author.Colleague);
            Assert.IsNotNull(author.Colleague.Books);
            foreach (var book in author.Colleague.Books)
            {
                Assert.IsNotNull(book.Coauthors);
                foreach (var coauthor in book.Coauthors)
                {
                    Assert.IsNotNull(coauthor.Book);
                }
            }

            bool isColleagueIncluded = author.Colleague.Id.IsEqualTo(colleague.Id);
            bool areBooksIncluded = addedBooks.All(x => author.Colleague.Books.Any(y => y.Id == x.Id));
            bool areCoauthorsBooksIncluded = addedBooks.All(x => author.Colleague.Books.Any(y => y.Coauthors.Any(z => z.BookId == x.Id)));

            Assert.IsTrue(isColleagueIncluded);
            Assert.IsTrue(areBooksIncluded);
            Assert.IsTrue(areCoauthorsBooksIncluded);
        }

        [Test]
        public void IncludeColleagueAndAllHisBooksAndTheirCoauthorsComepletlyFilled_ReturnsAuthorEntityWithColleagueAndAllHisBooksAndTheirCoauthorsWithBothAuthorAndBookPropsPopulated()
        {
            var newAuthor = CreateNewAuthor(addColleague: true);
            var colleague = _authorRepository.Get(newAuthor.ColleagueId.Value);

            var colleagueBook1 = CreateNewBook(author: colleague);
            var colleagueBook2 = CreateNewBook(author: colleague);
            var addedBooks = new Book[2] { colleagueBook1, colleagueBook2 };

            var book1Coauthor1 = CreateNewCoauthor(colleagueBook1);
            var book1Coauthor2 = CreateNewCoauthor(colleagueBook1);
            var book2Coauthor1 = CreateNewCoauthor(colleagueBook2);
            var book2Coauthor2 = CreateNewCoauthor(colleagueBook2);
            var addedCoauthors = new Coauthor[4] { book1Coauthor1, book1Coauthor2, book2Coauthor1, book2Coauthor2 };

            var author = _authorRepository.Get(newAuthor.Id);
            var couathorsIncludableQuery =
                                author
                                    .Include(x => x.Colleague, _authorRepository)
                                    .ThenInclude(x => x.Books, _bookRepository)
                                    .ThenInclude(x => x.Coauthors, _coauthorRepository);
            couathorsIncludableQuery.ThenInclude(x => x.Author, _authorRepository);
            couathorsIncludableQuery.ThenInclude(x => x.Book, _bookRepository);


            Assert.IsNotNull(author.Colleague);
            Assert.IsNotNull(author.Colleague.Books);
            foreach (var book in author.Colleague.Books)
            {
                Assert.IsNotNull(book.Coauthors);
                foreach (var coauthor in book.Coauthors)
                {
                    Assert.IsNotNull(coauthor.Author);
                    Assert.IsNotNull(coauthor.Book);
                }
            }

            bool isColleagueIncluded = author.Colleague.Id.IsEqualTo(colleague.Id);
            bool areBooksIncluded = addedBooks.All(x => author.Colleague.Books.Any(y => y.Id == x.Id));
            bool areCoauthorsAuthorsIncluded = addedCoauthors.All(x => author.Colleague.Books.Any(y => y.Coauthors.Any(z => z.Id == x.Id)));
            bool areCoauthorsBooksIncluded = addedBooks.All(x => author.Colleague.Books.Any(y => y.Coauthors.Any(z => z.BookId == x.Id)));
            bool areCoauthorsPropsIncluded = areCoauthorsAuthorsIncluded && areCoauthorsBooksIncluded;

            Assert.IsTrue(isColleagueIncluded);
            Assert.IsTrue(areBooksIncluded);
            Assert.IsTrue(areCoauthorsPropsIncluded);
        }


        [Test]
        public void IncludeAuthorAndHisColleagueInBookEntity_ReturnsBookEntityWithItsAuthorAndHisColleaguePopulated()
        {
            var newAuthor = CreateNewAuthor(addColleague: true);
            var book = CreateNewBook(author: newAuthor);

            var entity = _bookRepository.Get(book.Id);

            entity
                .Include(x => x.Author, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository);

            Assert.IsNotNull(entity.Author);
            Assert.IsNotNull(entity.Author.Colleague);

            bool isAuthorIncluded = entity.Author.Id.IsEqualTo(newAuthor.Id);
            bool isColleagueIncluded = entity.Author.Colleague.Id.IsEqualTo(newAuthor.ColleagueId.Value);

            Assert.IsTrue(isAuthorIncluded);
            Assert.IsTrue(isColleagueIncluded);
        }

        [Test]
        public void IncludeAuthorAndAllHisBooksInBookEntity_ReturnsBookEntityWithAuthorAndAllOfHisBooksPopulated()
        {
            var newAuthor = CreateNewAuthor();
            var newBook1 = CreateNewBook(author: newAuthor);
            var newBook2 = CreateNewBook(author: newAuthor);
            var addedBooks = new Book[2] { newBook1, newBook2 };

            var allAuthorBooks = _bookRepository.GetAllBooksOfAuthor(newAuthor.Id);

            var book = _bookRepository.Get(newBook1.Id);
            book
                .Include(x => x.Author, _authorRepository)
                .ThenInclude(x => x.Books, _bookRepository);

            Assert.IsNotNull(book.Author);
            Assert.IsNotNull(book.Author.Books);

            bool isAuthorIncluded = book.AuthorId.IsEqualTo(newAuthor.Id);
            bool areAllAuthorsBooksIncluded = addedBooks.All(x => book.Author.Books.Any(y => y.Id == x.Id));

            Assert.IsTrue(isAuthorIncluded);
            Assert.IsTrue(areAllAuthorsBooksIncluded);
        }

        [Test]
        public void IncludeAuthorsColleagueThatHasOriginalAuthorAsColleagueAndThenExecuteIncludeThenIncludeACoupleOfTimes_ReturnsAuthorWithEachColleagueHavingItsColleaguePopulatedAsCoded()
        {
            var newAuthor = CreateNewAuthor(addColleague: true);
            var colleague = _authorRepository.Get(newAuthor.ColleagueId.Value);
            colleague.ColleagueId = newAuthor.Id;
            _authorRepository.Update(colleague);

            var author = _authorRepository.Get(newAuthor.Id);

            author
                .Include(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository)
                .ThenInclude(x => x.Colleague, _authorRepository);

            Assert.IsNotNull(author.Colleague);
            
            Assert.IsNotNull(author.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague);
            Assert.IsNotNull(author.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague);
            
            Assert.IsNull(author.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague.Colleague);
        }

        #endregion

        #region Raw Order-bys Tests

        [Test]
        public void OrderByBookTitleDescending_ReturnsBooksListOrderedByTitleInDescendingOrder()
        {
            var allBooks = _bookRepository.GetAllBooksSortedDescByTitle();

            var deepClonedCollection = allBooks.DeepCopyCollection();
            var allBooksCloned = deepClonedCollection.OrderByDescending(x => x.Title).ToList();

            bool areOrderedDescByTitle = true;

            for (int i = 0; i < allBooks.Count; i++)
            {
                var bookFromDb = allBooks[i];
                var clonedBook = allBooksCloned[i];

                if (bookFromDb.Id != clonedBook.Id)
                {
                    areOrderedDescByTitle = false;
                    break;
                }
            }

            Assert.IsTrue(areOrderedDescByTitle);
        }

        [Test]
        public void OrderByBookTitleAscending_ReturnsBooksListOrderedByTitleInAscendingOrder()
        {
            var allBooks = _bookRepository.GetAllBooksSortedAscByTitle();

            var deepClonedCollection = allBooks.DeepCopyCollection();
            var allBooksCloned = deepClonedCollection.OrderBy(x => x.Title).ToList();

            bool areOrderedAscByTitle = true;

            for (int i = 0; i < allBooks.Count; i++)
            {
                if (allBooks[i].Id != allBooksCloned[i].Id)
                {
                    areOrderedAscByTitle = false;
                    break;
                }
            }

            Assert.IsTrue(areOrderedAscByTitle);
        }

        #endregion
    }
}
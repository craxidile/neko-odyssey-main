using System.Linq;
using Database.Repository;
using NUnit.Framework;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Tests.Extension;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class QueryPatternTests : GRUTestsBase
    {
        #region Query Pattern Select Tests

        [Test]
        public void SelectAuthorId_ReturnsAuthorOnlyWithIdPropSet()
        {
            var newEntity = CreateNewAuthor();

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
            });

            var entity = _authorRepository.Get(newEntity.Id, query);

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

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Name = x.Name,
            });

            var entity = _authorRepository.Get(newEntity.Id, query);

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

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Birthday = x.Birthday,
            });

            var entity = _authorRepository.Get(newEntity.Id, query);

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

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
                Name = x.Name,
            });

            var entity = _authorRepository.Get(newEntity.Id, query);

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

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
                Name = x.Name,
                Birthday = x.Birthday,
            });

            var entity = _authorRepository.Get(newEntity.Id, query);

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

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
                Name = x.Name,
                City = x.City,
                Birthday = x.Birthday,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted
            });

            var entity = _authorRepository.Get(updatedEntity.Id, query);

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
        public void SelectDistinctAuthorDateOfBirth_ReturnsAuthorOnlyWithDateOfBirthPropDistinctSelected()
        {
            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Birthday = x.Birthday,
            }, shouldDistinct: true);

            var entities = _authorRepository.GetAll(query);

            bool allUnique = entities.GroupBy(x => x.Birthday).All(g => g.Count() == 1);

            Assert.IsTrue(allUnique);
        }

        [Test]
        public void SelectDistinctAuthordName_ReturnsAuthorOnlyWithNameDistinctSelected()
        {
            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Name = x.Name,
            }, shouldDistinct: true);

            var entities = _authorRepository.GetAll(query);
            bool allUnique = entities.GroupBy(x => x.Name).All(g => g.Count() == 1);
            Assert.IsTrue(allUnique);
        }

        #endregion

        #region Query Pattern Where Tests

        [Test]
        public void WhereAuthorId_GreaterThanX_ReturnsAllAuthorsWithIdGreaterThanX([Values(2)] int nAuthorsToCreateAfterFirst)
        {
            var newEntity = CreateNewAuthor();
            var entitiesPostAdded = CreateNewAuthors(nAuthorsToCreateAfterFirst);

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
            });
            query.AddWhereExpression(x => x.Id > newEntity.Id);

            var matches = _authorRepository.Find(query);

            Assert.AreEqual(matches.Count, nAuthorsToCreateAfterFirst);
        }

        [Test]
        public void WhereAuthorId_LessThanX_ReturnsAllAuthorsWithIdLessThanX()
        {
            var entities = _authorRepository.GetAll();

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
            });

            var idForCompare = entities.Last().Id;
            query.AddWhereExpression(x => x.Id < idForCompare);

            var matches = _authorRepository.Find(query);

            Assert.AreEqual(matches.Count, entities.Count - 1);
        }

        [Test]
        public void WhereAuthorId_Equals_To_X_ReturnsSingleAuthorWithIdBeingEqualTo_X()
        {
            var newEntity = CreateNewAuthor();

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
            });

            query.AddWhereExpression(x => x.Id.Equals(newEntity.Id));

            var match = _authorRepository.Find(query);

            Assert.IsTrue(match.Count == 1);
        }

        [Test]
        public void WhereAuthorId_Equals_To_X_OrLessThan_Y_ReturnsMathcingAuthor()
        {
            var entities = _authorRepository.GetAll();

            var newEntity = CreateNewAuthor();

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Id = x.Id,
            });

            var secondEntityId = entities.ToList()[1].Id;
            query.AddWhereExpression(x => x.Id.Equals(newEntity.Id) || x.Id < secondEntityId);

            var match = _authorRepository.Find(query);

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

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Name = x.Name,
            });
            query.AddWhereExpression(x => x.Name.StartsWith(nameToCheck));
            var matches = _authorRepository.Find(query);

            Assert.IsTrue(matches.Count == numOfEntitiesStartingWithTheSameName);
        }

        #endregion

        #region Query Pattern  Order-bys Tests

        [Test]
        public void OrderByBookTitleDescending_ReturnsBooksListOrderedByTitleInDescendingOrder()
        {
            var query = new QueryExpression<Book>();
            query.AddOrderExpression(x => new Book(x.Title), OrderDirection.Descending);
            var allBooks = _bookRepository.Find(query).ToList();

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
            var query = new QueryExpression<Book>();
            query.AddOrderExpression(x => new Book(x.Title), OrderDirection.Ascending);
            var allBooks = _bookRepository.Find(query).ToList();

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

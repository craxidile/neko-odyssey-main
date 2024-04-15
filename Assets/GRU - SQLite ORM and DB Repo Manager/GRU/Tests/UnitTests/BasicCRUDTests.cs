using NUnit.Framework;
using SpatiumInteractive.Libraries.Unity.GRU.Tests.Extension;
using SpatiumInteractive.Libraries.Unity.Platform.Helpers;
using SQLite4Unity3d;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class BasicCRUDTests: GRUTestsBase
    {
        #region General CRUD Tests

        [Test]
        public void AddNonValidAuthor_FailsToCreateAuthor()
        {
            var author = _authorMock.GetNonValidAuthorForCreate();

            Assert.Throws<SQLiteException>(() => _authorRepository.Add(author));

            int authorCountAfter = _authorRepository.GetAll().Count;

            Assert.AreEqual(authorCountAfter, _currentAuthorsCount);
        }

        [Test]
        public void AddValidAuthor_CreatesAuthor()
        {
            var newEntity = base.CreateNewAuthor();
            int authorCountAfter = _authorRepository.GetAll().Count;

            Assert.Greater(authorCountAfter, _currentAuthorsCount);
            Assert.NotNull(newEntity);
            Assert.Greater(newEntity.Id, default);
        }

        [Test]
        public void AddNValidAuthors_CreatesNAuthors([Values(1, 5, 10)] int n)
        {
            for (int i = 0; i < n; i++)
            {
                var newEntity = base.CreateNewAuthor();
                int authorCountAfter = _authorRepository.GetAll().Count;

                Assert.Greater(authorCountAfter, _currentAuthorsCount);
                Assert.NotNull(newEntity);
            }
        }

        [Test]
        public void GetAuthorByValidID_ReturnsAuthor()
        {
            var newEntity = base.CreateNewAuthor();
            var entity = _authorRepository.Get(newEntity.Id);
            Assert.NotNull(entity);
        }

        [Test]
        public void GetAuthorByNonValidID_FailsToReturnAuthor()
        {
            int id = _authorMock.GetNonExistingID();
            var entity = _authorRepository.Get(id);
            Assert.Null(entity);
        }

        [Test]
        public void UpdateAuthorProp_UpdatesUpdatedAtDateAsWell()
        {
            var newEntity = base.CreateNewAuthor();

            var entityToUpdate = _authorMock.DeepCloneAuthorAndUpdateName(newEntity);
            var updatedEntity = _authorRepository.Update(entityToUpdate);

            var entity = _authorRepository.Get(updatedEntity.Id);

            bool areEqual = DeepCompare.AreEquallyDumped(entity, updatedEntity);

            Assert.IsTrue(areEqual);

            bool hasSetUpdatedAt = updatedEntity.UpdatedAt.IsSelectedInDb();
            Assert.IsTrue(hasSetUpdatedAt);
        }

        [Test]
        public void UpdateAuthorName_UpdatesOnlyAuthorsName()
        {
            var newEntity = base.CreateNewAuthor();

            var entityToUpdate = _authorMock.DeepCloneAuthorAndUpdateName(newEntity);
            var updatedEntity = _authorRepository.Update(entityToUpdate);

            var entity = _authorRepository.Get(updatedEntity.Id);

            bool areEqual = DeepCompare.AreEquallyDumped(entity, updatedEntity);

            Assert.IsTrue(areEqual);
        }

        [Test]
        public void UpdateAuthorDateOfBirth_UpdatesOnlyAuthorsDateOfBirth()
        {
            var newEntity = base.CreateNewAuthor();

            var entityToUpdate = _authorMock.DeepCloneAuthorAndUpdateBirthday(newEntity);
            var updatedEntity = _authorRepository.Update(entityToUpdate);

            var entity = _authorRepository.Get(updatedEntity.Id);
            
            bool areEqual = DeepCompare.AreEquallyDumped(entity, updatedEntity);

            Assert.IsTrue(areEqual);
        }

        [Test]
        public void UpdateAuthorWithNonValidData_FailsToUpdateAuthor()
        {
            var newEntity = base.CreateNewAuthor();
            var invalidEntityToUpdate = _authorMock.DeepCloneAuthorAndMakeItHaveInvalidData(newEntity);
            Assert.Throws<SQLiteException>(() => _authorRepository.Update(invalidEntityToUpdate));
        }

        [Test]
        public void RemoveAuthorByID_DeletesAuthor()
        {
            var newEntity = base.CreateNewAuthor();
            int id = newEntity.Id;

            _authorRepository.Remove(id);

            int countAfterRemoval = _authorRepository.GetAll().Count;

            Assert.AreEqual(_currentAuthorsCount, countAfterRemoval);
        }

        [Test]
        public void RemoveAuthorByFetchedEntityReference_DeletesAuthor()
        {
            var newEntity = base.CreateNewAuthor();

            _authorRepository.Remove(newEntity);

            int countAfterRemoval = _authorRepository.GetAll().Count;

            Assert.AreEqual(_currentAuthorsCount, countAfterRemoval);
        }

        [Test]
        public void AuthorSetDeleted_UpdatesAuthorIsDeletedToTrueAndIsActiveToFalse()
        {
            var newEntity = base.CreateNewAuthor();

            newEntity.SetDeleted();

            _authorRepository.Update(newEntity);

            var entity = _authorRepository.Get(newEntity.Id);
            bool isSetAsDeleted = entity.IsDeleted && !entity.IsActive;

            Assert.IsTrue(isSetAsDeleted);
        }

        [Test]
        public void AuthorSetActive_UpdatesAuthorIsDeletedToFalseAndIsActiveToTrue()
        {
            var newEntity = base.CreateNewAuthor();
            newEntity.IsDeleted = true;
            newEntity.IsActive = false;

            newEntity.SetActive();

            _authorRepository.Update(newEntity);

            var entity = _authorRepository.Get(newEntity.Id);
            bool isSetAsActive = !entity.IsDeleted && entity.IsActive;

            Assert.IsTrue(isSetAsActive);
        }

        #endregion
    }
}

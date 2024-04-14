using System;
using System.Collections.Generic;
using Database.Repository;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class BookMock
    {
        #region Data for Mock

        private List<string> _mockTitles = new List<string>()
        {
            "Harry Potter",
            "Snow white",
            "Beauty and the beast",
            "Old man and the sea",
            "Some old book",
            "Really old book",
            "Just another SF story",
            "Drama and drama only"
        };

        #endregion

        #region Public Methods

        public Book GetValidBookForCreate(int authorId)
        {
            string bookTitle = GetRandomBookTitle();
            return new Book()
            {
                Title = bookTitle,
                AuthorId = authorId
            };
        }

        public Book GetBookWithNonExistingAuthor()
        {
            string bookTitle = null;
            return new Book()
            {
                Title = bookTitle,
                AuthorId = -1
            };
        }

        public Book DeepCloneBookAndUpdateTitle(Book entity)
        {
            string newTitle = GetRandomBookTitleOtherThan(entity.Title);
            var entityCloned = entity.DeepCopy();
            entityCloned.Title = newTitle;
            return entityCloned;
        }

        public Book DeepCloneBookAndMakeItHaveInvalidData(Book entity)
        {
            var entityCloned = entity.DeepCopy();
            entityCloned.Title = null;
            return entityCloned;
        }

        public int GetNonExistingID()
        {
            Random r = new Random();
            int id = r.Next(10000000, 90000000);
            return id;
        }

        #endregion

        #region Private Methods

        private string GetRandomBookTitle()
        {
            Random r = new Random();
            int randomIndex = r.Next(0, _mockTitles.Count - 1);
            return _mockTitles[randomIndex];
        }

        private string GetRandomBookTitleOtherThan(string currentTitle)
        {
            Random r = new Random();
            string newRandomTitle;

            while (true)
            {
                int randomIndex = r.Next(0, _mockTitles.Count - 1);
                newRandomTitle = _mockTitles[randomIndex];

                if (newRandomTitle != currentTitle)
                {
                    break;
                }
            }

            return newRandomTitle;
        }

        #endregion
    }
}

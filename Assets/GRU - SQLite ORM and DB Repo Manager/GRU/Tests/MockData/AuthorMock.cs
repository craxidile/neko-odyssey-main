using System;
using System.Collections.Generic;
using Database.Repository;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class AuthorMock
    {
        #region Data for Mock

        private List<string> _mockNames = new List<string>()
        {
            "Ana",
            "Ann",
            "Antonio",
            "Anthony",
            "Claudia",
            "Drake",
            "Ernest",
            "John",
            "Mark",
            "Luke",
            "Lucy",
            "Lucca",
            "Kishore",
            "Vineet",
            "Max",
            "Karl",
            "Stephen",
            "Marie",
            "Martin",
            "Patricia",
            "Philip",
            "Vlad",
            "Wiliam"
        };

        private List<string> _mockCities = new List<string>()
        {
            "Austin",
            "Berlin",
            "Bratislava",
            "Beograd",
            "Bruxeles",
            "Chicago",
            "Cairo",
            "Dubai",
            "Milan",
            "Lisabon",
            "Ljubljana",
            "New York",
            "Los Angeles",
            "Lyon",
            "San Francisco",
            "Paris",
            "Rome",
            "Sidney",
            "Viena",
            "Zagreb",
        };

        #endregion

        #region Public Methods

        public Author GetValidAuthorForCreate(int? colleagueId = null)
        {
            string authorName = GetRandomAuthorName();
            string authorCity = GetRandomAuthorCity();
            DateTime authorBirthday = DateTime.Now;
            var author = new Author()
            {
                Name = authorName,
                City = authorCity,
                Birthday = authorBirthday,
                ColleagueId = colleagueId
            };
            return author;
        }

        public Author GetNonValidAuthorForCreate()
        {
            string authorName = null;
            string authorCity = GetRandomAuthorCity();
            DateTime authorBirthday = DateTime.Now;
            return new Author()
            {
                Name = authorName,
                City = authorCity,
                Birthday = authorBirthday,
            };
        }

        public Author DeepCloneAuthorAndUpdateName(Author entity)
        {
            string newName = GetRandomAuthorNameOtherThan(entity.Name);
            var entityCloned = entity.DeepCopy();
            entityCloned.Name = newName;
            return entityCloned;
        }

        public Author DeepCloneAuthorAndUpdateBirthday(Author entity)
        {
            var newBirthday = GetRandomAuthorBirthdayOtherThan(entity.Birthday);
            var entityCloned = entity.DeepCopy();
            entityCloned.Birthday = newBirthday;
            return entityCloned;
        }

        public Author DeepCloneAuthorAndMakeItHaveInvalidData(Author entity)
        {
            var entityCloned = entity.DeepCopy();
            entityCloned.Name = null;
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

        private string GetRandomAuthorName()
        {
            Random r = new Random();
            int randomIndex = r.Next(0, _mockNames.Count - 1);
            return _mockNames[randomIndex];
        }

        private DateTime GetRandomAuthorBirthdayOtherThan(DateTime currentBirthday)
        {
            DateTime newBirthday = currentBirthday;
            newBirthday.AddYears(-2);
            return newBirthday;
        }

        private string GetRandomAuthorNameOtherThan(string currentName)
        {
            Random r = new Random();
            string newRandomName;

            while (true)
            {
                int randomIndex = r.Next(0, _mockNames.Count - 1);
                newRandomName = _mockNames[randomIndex];

                if (newRandomName != currentName)
                {
                    break;
                }
            }

            return newRandomName;
        }

        private string GetRandomAuthorCity()
        {
            Random r = new Random();
            int randomIndex = r.Next(0, _mockCities.Count - 1);
            return _mockCities[randomIndex];
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Database.Repository;
using SpatiumInteractive.Libraries.Unity.GRU.Base;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class CompilerTests : GRUTestsBase
    {
        [Test]
        public void String_StartsWith_CompilesToSQLiteStartsWith()
        {
            var newEntity = CreateNewAuthor();
            string nameToCheck = newEntity.Name;

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Name = x.Name,
            });
            query.AddWhereExpression(x => x.Name.StartsWith(nameToCheck));

            List<Author> authorList = new List<Author>();
            Assert.DoesNotThrow(() => authorList = _authorRepository.Find(query).ToList());

            bool allStartWith = authorList
                .All(x => x.Name.StartsWith(nameToCheck, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(allStartWith);
        }

        [Test]
        public void String_ToLower_CompilesToSqliteLower()
        {
            var newEntity = CreateNewAuthor(NameFormat.AllLower);
            string nameToCheck = newEntity.Name;

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Name = x.Name,
            });
            query.AddWhereExpression(x => x.Name.ToLower() == (nameToCheck));

            var matches = new List<Author>();
            Assert.DoesNotThrow(() => matches = _authorRepository.Find(query).ToList());

            bool doesAtLeastOneExist = matches.Any();
            Assert.IsTrue(doesAtLeastOneExist);
        }

        [Test]
        public void String_ToUpper_CompilesToSqliteUpper()
        {
            var newEntity = CreateNewAuthor(NameFormat.AllUpper);
            string nameToCheck = newEntity.Name;

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Name = x.Name,
            });
            query.AddWhereExpression(x => x.Name.ToUpper() == (nameToCheck));

            var matches = new List<Author>();
            Assert.DoesNotThrow(() => matches = _authorRepository.Find(query).ToList());

            bool doesAtLeastOneExist = matches.Any();
            Assert.IsTrue(doesAtLeastOneExist);
        }

        [Test]
        public void String_Trim_CompilesToSqliteTrim()
        {
            var newEntity = CreateNewAuthor(NameFormat.WrapedWithSpaces);
            string nameToCheck = newEntity.Name.Trim();

            var query = new QueryExpression<Author>();
            query.AddSelectExpression(x => new Author()
            {
                Name = x.Name,
            });
            query.AddWhereExpression(x => x.Name.Trim() == (nameToCheck));

            var matches = new List<Author>();
            Assert.DoesNotThrow(() => matches = _authorRepository.Find(query).ToList());

            bool doesAtLeastOneExist = matches.Any();
            Assert.IsTrue(doesAtLeastOneExist);
        }
    }
}

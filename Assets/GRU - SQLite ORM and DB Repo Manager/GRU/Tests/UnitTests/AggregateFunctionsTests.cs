using System;
using NUnit.Framework;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class AggregateFunctionsTests : GRUTestsBase
    {
        [Test]
        public void CountAllAuthors_ReturnsTotalEntityCountFromAuthorsTable()
        {
            var entities = _authorRepository.GetAll();

            int entitiesCount = entities.Count;
            int totalCount = _authorRepository.GetTotalCount();

            Assert.AreEqual(entitiesCount, totalCount);
        }

        //[Test]
        public void AvgIdOffAllAuthors_ReturnsAvgOfAllIdsOfAllAuthors()
        {
            //todo: avg aggregate is not yet implemented (will be in future versions of GRU)
            throw new NotImplementedException();
        }

        //[Test]
        public void MaxIdOffAllAuthors_ReturnsMaxOfAllIdsOfAllAuthors()
        {
            //todo: max aggregate is not yet implemented (will be in future versions of GRU)
            throw new NotImplementedException();
        }

        //[Test]
        public void MinIdOffAllAuthors_ReturnsMinOfAllIdsOfAllAuthors()
        {
            //todo: min aggregate is not yet implemented (will be in future versions of GRU)
            throw new NotImplementedException();
        }

    }
}

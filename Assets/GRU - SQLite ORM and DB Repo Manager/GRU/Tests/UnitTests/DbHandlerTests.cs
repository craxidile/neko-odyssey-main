using Assets.Scripts.Integration.GRU.DbContexts;
using NUnit.Framework;
using SpatiumInteractive.Libraries.Unity.GRU.Tests.Extension;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public class DbHandlerTests: GRUTestsBase
    {
        [Test]
        public void WhenExecutingTest_TestCachingSystemHasDbConnectionCachedAlready()
        {
            var cachedConnection = GRUTestsCache<BookshopexampledbDbContext>.GetDbConnection();
            bool isSet = 
                cachedConnection != null && 
                cachedConnection.DatabasePath.IsSelectedInDb() && 
                cachedConnection.DatabasePath.IsSelectedInDb();

            Assert.IsTrue(isSet);
        }
    }
}

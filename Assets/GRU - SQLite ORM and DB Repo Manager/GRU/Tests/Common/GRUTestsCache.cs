using Database.Repository;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace SpatiumInteractive.Libraries.Unity.GRU.Tests
{
    public static class GRUTestsCache<T> where T: SQLiteConnection
    {
        #region Private Fields

        private static DBHandler _dDbHandler;

        private static SQLiteConnection _dbConnection;

        private static IDbContext<T> _dbContext;

        #endregion

        #region Public Methods

        public static DBHandler GetDbHandler()
        {
            return _dDbHandler;
        }

        public static SQLiteConnection GetDbConnection()
        {
            return _dbConnection;
        }

        public static IDbContext<T> GetDbContext()
        {
            return _dbContext;
        }

        public static bool HasInitCache()
        {
            return
                _dDbHandler != null &&
                _dbConnection != null &&
                _dbContext != null;
        }

        public static void InitCache
        (
            DBHandler dbHandler,
            SQLiteConnection dbConnection,
            IDbContext<T> dbContext
        )
        {
            _dDbHandler = dbHandler;
            _dbConnection = dbConnection;
            _dbContext = dbContext;
        }

        #endregion
    }
}

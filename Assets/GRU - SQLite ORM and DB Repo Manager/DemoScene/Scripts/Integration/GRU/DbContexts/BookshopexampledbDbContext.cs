using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace Assets.Scripts.Integration.GRU.DbContexts
{
	public class BookshopexampledbDbContext : SQLiteConnection, IDbContext<BookshopexampledbDbContext>
	{
		public BookshopexampledbDbContext(SQLiteConnection connection) : base(connection.DatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, connection.StoreDateTimeAsTicks)
		{
		}

		public BookshopexampledbDbContext Context => this;
	}
}


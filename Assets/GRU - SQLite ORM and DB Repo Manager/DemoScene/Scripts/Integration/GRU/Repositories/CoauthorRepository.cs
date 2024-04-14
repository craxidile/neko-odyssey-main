using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using Assets.Scripts.Integration.GRU.DbContexts;
using Database.Repository;

namespace Assets.Scripts.Integration.GRU.Repositories
{
    public class CoauthorRepository : Repository<Coauthor, int, BookshopexampledbDbContext>, ICoauthorRepository
    {
        public CoauthorRepository(IDbContext<BookshopexampledbDbContext> dbContext) : base(dbContext)
        {
        }
    }
}

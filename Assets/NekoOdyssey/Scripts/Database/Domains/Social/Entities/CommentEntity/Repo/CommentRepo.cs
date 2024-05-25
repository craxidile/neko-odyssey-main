using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Social.Entities.CommentEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Social.Entities.CommentEntity.Repo
{
    public class CommentRepo : Repository<Comment, int, SQLiteConnection>
    {
        public CommentRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<Comment> List()
        {
            return _dbContext.Context.Table<Comment>()
                .OrderBy(i => i.Id)
                .ToList();
        }
    }
}
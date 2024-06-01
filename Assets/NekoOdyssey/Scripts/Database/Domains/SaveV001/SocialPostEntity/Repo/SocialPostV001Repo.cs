using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.SocialPostEntity.Repo
{
    public class SocialPostV001Repo : Repository<SocialPostV001, int, SQLiteConnection>
    {
        private readonly CatPhotoV001Repo _catPhotoV001Repo;
        
        public SocialPostV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
            _catPhotoV001Repo = new CatPhotoV001Repo(dbContext);
        }

        public ICollection<SocialPostV001> List()
        {
            var socialPosts = _dbContext.Context.Table<SocialPostV001>()
                .OrderByDescending(sp => sp.Id)
                .ToList();
            foreach (var socialPost in socialPosts)
            {
                socialPost.Photo = _catPhotoV001Repo.FindById(socialPost.CatPhotoV001Id);
            }
            return socialPosts;
        }

        public SocialPostV001 FindByCatPhotoId(int catPhotoId)
        {
            return _dbContext.Context.Table<SocialPostV001>()
                .FirstOrDefault(sp => sp.CatPhotoV001Id == catPhotoId);
        }
    }
}
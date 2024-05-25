using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Repo
{
    public class CatPhotoV001Repo : Repository<CatPhotoV001, int, SQLiteConnection>
    {
        public CatPhotoV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<CatPhotoV001> List()
        {
            return _dbContext.Context.Table<CatPhotoV001>()
                .OrderByDescending(ct => ct.Id)
                .ToList();
        }

        public CatPhotoV001 FindById(int id)
        {
            return _dbContext.Context
                .Table<CatPhotoV001>()
                .FirstOrDefault(cp => cp.Id == id);
        }

        public CatPhotoV001 FindByAssetBundleName(string assetBundleName)
        {
            return _dbContext.Context
                .Table<CatPhotoV001>()
                .FirstOrDefault(cp => cp.AssetBundleName == assetBundleName);
        }
    }
}
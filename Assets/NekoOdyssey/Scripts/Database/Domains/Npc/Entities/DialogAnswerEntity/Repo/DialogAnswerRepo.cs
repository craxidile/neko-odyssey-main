using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogAnswerEntity.Repo
{
    public class DialogAnswerRepo : Repository<DialogAnswer, int, SQLiteConnection>
    {
        public DialogAnswerRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<DialogAnswer> List()
        {
            return _dbContext.Context.Table<DialogAnswer>()
                .OrderBy(da => da.Id)
                .ToList();
        }

        public DialogAnswer FindById(int id)
        {
            return _dbContext.Context.Table<DialogAnswer>()
                .FirstOrDefault(da => da.Id == id);
        }

        public ICollection<DialogAnswer> ListByQuestionId(int questionId)
        {
            return _dbContext.Context.Table<DialogAnswer>()
                .Where(da => da.QuestionId == questionId)
                .OrderBy(da => da.Id)
                .ToList();
        }
    }
}
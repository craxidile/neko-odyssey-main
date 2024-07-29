using System.Collections;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Npc;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Repo;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.MasterData.Npc.QuestGroups
{
    public class QuestGroupsMasterData
    {
        public bool Ready { get; private set; }
        public ICollection<QuestGroup> QuestGroups { get; private set; }

        public Subject<Unit> OnReady { get; } = new();

        public void Bind()
        {
            InitializeDatabase();
        }

        public void Start()
        {
            ListAll();
            Ready = true;
            OnReady.OnNext(default);
        }

        public void Unbind()
        {
        }

        private void InitializeDatabase()
        {
            using (new NpcDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }

        private void ListAll()
        {
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var questGroupRepo = new QuestGroupRepo(npcDbContext);
                QuestGroups = questGroupRepo.List();
                foreach (var questGroup in QuestGroups)
                {
                    questGroup.Conditions = ListConditionsByQuestGroupId(questGroup.Id);
                    questGroup.Quests = ListQuestsByQuestGroupId(questGroup.Id);
                    foreach (var quest in questGroup.Quests)
                    {
                        quest.Conditions = ListConditionsByQuestId(quest.Id);
                    }
                }
            }
        }

        private static ICollection<QuestGroupCondition> ListConditionsByQuestGroupId(int questGroupId)
        {
            ICollection<QuestGroupCondition> conditions = null;
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var questGroupConditionRepo = new QuestGroupConditionRepo(npcDbContext);
                conditions = questGroupConditionRepo.ListByQuestGroupId(questGroupId);
            }

            return conditions;
        }

        private static ICollection<Quest> ListQuestsByQuestGroupId(int questGroupId)
        {
            ICollection<Quest> quests = null;
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var questRepo = new QuestRepo(npcDbContext);
                quests = questRepo.ListByQuestGroupId(questGroupId);
            }

            return quests;
        }

        private static ICollection<QuestCondition> ListConditionsByQuestId(int questId)
        {
            ICollection<QuestCondition> conditions = null;
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var questConditionRepo = new QuestConditionRepo(npcDbContext);
                conditions = questConditionRepo.ListByQuestId(questId);
            }

            return conditions;
        }
    }
}
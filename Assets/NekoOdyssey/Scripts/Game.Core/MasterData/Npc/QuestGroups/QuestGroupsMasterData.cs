using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        }

        public void Start()
        {
            if (GameRunner.Instance.Core.MasterData.NpcMasterData.DialogsMasterData.Ready)
            {
                ListAll();
                Ready = true;
                OnReady.OnNext(default);
            }
            else
            {
                GameRunner.Instance.Core.MasterData.NpcMasterData.DialogsMasterData.OnReady
                    .Subscribe(_ =>
                    {
                        ListAll();
                        Ready = true;
                        OnReady.OnNext(default);
                    })
                    .AddTo(GameRunner.Instance);
            }
        }

        public void Unbind()
        {
        }

        private void ListAll()
        {
            var dialogs = GameRunner.Instance.Core.MasterData.NpcMasterData.DialogsMasterData.Dialogs;
            
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var questGroupRepo = new QuestGroupRepo(npcDbContext);
                var questGroupConditionRepo = new QuestGroupConditionRepo(npcDbContext);
                var questRepo = new QuestRepo(npcDbContext);
                var questConditionRepo = new QuestConditionRepo(npcDbContext);

                QuestGroups = questGroupRepo.List();
                foreach (var questGroup in QuestGroups)
                {
                    questGroup.Conditions = questGroupConditionRepo.ListByQuestGroupId(questGroup.Id);
                    questGroup.Quests = questRepo.ListByQuestGroupId(questGroup.Id);
                    foreach (var quest in questGroup.Quests)
                    {
                        quest.Conditions = questConditionRepo.ListByQuestId(quest.Id);
                        if (quest.DialogId == null) continue;
                        quest.Dialog = dialogs.FirstOrDefault(d => d.Id == quest.DialogId.Value);
                    }
                }
            }
        }
    }
}
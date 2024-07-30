using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Npc;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.ChatGroupEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Repo;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Repo;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.MasterData.Npc.ChatGroups
{
    public class ChatGroupsMasterData
    {
        public bool Ready { get; private set; }
        public ICollection<ChatGroup> ChatGroups { get; private set; }

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
                var chatGroupRepo = new ChatGroupRepo(npcDbContext);
                var chatGroupConditionRepo = new ChatGroupConditionRepo(npcDbContext);
                var chatRepo = new ChatRepo(npcDbContext);
                var chatConditionRepo = new ChatConditionRepo(npcDbContext);

                ChatGroups = chatGroupRepo.List();
                foreach (var chatGroup in ChatGroups)
                {
                    chatGroup.Conditions = chatGroupConditionRepo.ListByChatGroupId(chatGroup.Id);
                    chatGroup.Chats = chatRepo.ListByChatGroupId(chatGroup.Id);
                    foreach (var chat in chatGroup.Chats)
                    {
                        chat.Conditions = chatConditionRepo.ListByChatId(chat.Id);
                        chat.Dialog = dialogs.FirstOrDefault(d => d.Id == chat.DialogId);
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;
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
                var chatGroupRepo = new ChatGroupRepo(npcDbContext);
                ChatGroups = chatGroupRepo.List();
                foreach (var chatGroup in ChatGroups)
                {
                    chatGroup.Conditions = ListConditionsByChatGroupId(chatGroup.Id);
                    chatGroup.Chats = ListChatsByChatGroupId(chatGroup.Id);
                    foreach (var chat in chatGroup.Chats)
                    {
                        chat.Conditions = ListConditionsByChatId(chat.Id);
                    }
                }
            }
        }

        private static ICollection<ChatGroupCondition> ListConditionsByChatGroupId(int chatGroupId)
        {
            ICollection<ChatGroupCondition> conditions = null;
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var chatGroupConditionRepo = new ChatGroupConditionRepo(npcDbContext);
                conditions = chatGroupConditionRepo.ListByChatGroupId(chatGroupId);
            }

            return conditions;
        }

        private static ICollection<Chat> ListChatsByChatGroupId(int chatGroupId)
        {
            ICollection<Chat> chats = null;
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var chatRepo = new ChatRepo(npcDbContext);
                chats = chatRepo.ListByChatGroupId(chatGroupId);
            }

            return chats;
        }

        private static ICollection<ChatCondition> ListConditionsByChatId(int questId)
        {
            ICollection<ChatCondition> conditions = null;
            using (var npcDbContext = new NpcDbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var chatConditionRepo = new ChatConditionRepo(npcDbContext);
                conditions = chatConditionRepo.ListByChatId(questId);
            }

            return conditions;
        }
    }
}
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Npc;
using NekoOdyssey.Scripts.Game.Core.MasterData.Npc.ChatGroups;
using NekoOdyssey.Scripts.Game.Core.MasterData.Npc.Dialogs;
using NekoOdyssey.Scripts.Game.Core.MasterData.Npc.QuestGroups;
using NekoOdyssey.Scripts.Game.Core.MasterData.Npc.Routines;

namespace NekoOdyssey.Scripts.Game.Core.MasterData.Npc
{
    public class NpcMasterData
    {
        public DialogsMasterData DialogsMasterData { get; } = new();
        public QuestGroupsMasterData QuestGroupsMasterData { get; } = new();
        public ChatGroupsMasterData ChatGroupsMasterData { get; } = new();
        public RoutinesMasterData RoutinesMasterData { get; } = new();
        
        public void Bind()
        {
            InitializeDatabase();
            DialogsMasterData.Bind();
            QuestGroupsMasterData.Bind();
            ChatGroupsMasterData.Bind();
            RoutinesMasterData.Bind();
        }
        
        public void Start()
        {
            DialogsMasterData.Start();
            QuestGroupsMasterData.Start();
            ChatGroupsMasterData.Start();
            RoutinesMasterData.Start();
        }

        public void Unbind()
        {
            DialogsMasterData.Unbind();
            QuestGroupsMasterData.Unbind();
            ChatGroupsMasterData.Unbind();
            RoutinesMasterData.Unbind();
        }

        private void InitializeDatabase()
        {
            using (new NpcDbContext(new() { CopyMode = DbCopyMode.ForceCopy, ReadOnly = false })) ;
        }
    }
}
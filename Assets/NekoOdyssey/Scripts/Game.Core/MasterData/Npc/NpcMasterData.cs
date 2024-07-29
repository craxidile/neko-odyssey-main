using NekoOdyssey.Scripts.Game.Core.MasterData.Npc.ChatGroups;
using NekoOdyssey.Scripts.Game.Core.MasterData.Npc.Dialogs;
using NekoOdyssey.Scripts.Game.Core.MasterData.Npc.QuestGroups;

namespace NekoOdyssey.Scripts.Game.Core.MasterData.Npc
{
    public class NpcMasterData
    {
        public QuestGroupsMasterData QuestGroupsMasterData { get; } = new();
        public ChatGroupsMasterData ChatGroupsMasterData { get; } = new();
        public DialogsMasterData DialogsMasterData { get; } = new();
        
        public void Bind()
        {
            QuestGroupsMasterData.Bind();
            ChatGroupsMasterData.Bind();
            DialogsMasterData.Bind();
        }
        
        public void Start()
        {
            QuestGroupsMasterData.Start();
            ChatGroupsMasterData.Start();
            DialogsMasterData.Start();
        }

        public void Unbind()
        {
            QuestGroupsMasterData.Unbind();
            ChatGroupsMasterData.Unbind();
            DialogsMasterData.Unbind();
        }
    }
}
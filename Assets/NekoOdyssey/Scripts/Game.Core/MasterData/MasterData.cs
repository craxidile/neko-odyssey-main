using NekoOdyssey.Scripts.Game.Core.MasterData.Cats;
using NekoOdyssey.Scripts.Game.Core.MasterData.Items;
using NekoOdyssey.Scripts.Game.Core.MasterData.Npc;

namespace NekoOdyssey.Scripts.Game.Core.MasterData
{
    public class MasterData
    {
        public CatsMasterData CatsMasterData { get; } = new();
        public ItemsMasterData ItemsMasterData { get; } = new();
        public NpcMasterData NpcMasterData { get; } = new();
        
        public void Bind()
        {
            CatsMasterData.Bind();
            ItemsMasterData.Bind();
            NpcMasterData.Bind();
        }

        public void Start()
        {
            CatsMasterData.Start();
            ItemsMasterData.Start();
            NpcMasterData.Start();
        }

        public void Unbind()
        {
            CatsMasterData.Unbind();
            ItemsMasterData.Unbind();
            NpcMasterData.Unbind();
        }
    }
}
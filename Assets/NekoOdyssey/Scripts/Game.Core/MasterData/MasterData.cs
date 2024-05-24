using NekoOdyssey.Scripts.Game.Core.MasterData.Items;

namespace NekoOdyssey.Scripts.Game.Core.MasterData
{
    public class MasterData
    {
        public CatsMasterData CatsMasterData { get; } = new();
        public ItemsMasterData ItemsMasterData { get; } = new();
        
        public void Bind()
        {
            CatsMasterData.Bind();
            ItemsMasterData.Bind();
        }

        public void Start()
        {
            CatsMasterData.Start();
            ItemsMasterData.Start();
        }

        public void Unbind()
        {
            CatsMasterData.Unbind();
            ItemsMasterData.Unbind();
        }
    }
}
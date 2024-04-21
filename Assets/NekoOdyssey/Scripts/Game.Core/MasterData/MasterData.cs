using NekoOdyssey.Scripts.Game.Core.MasterData.Items;

namespace NekoOdyssey.Scripts.Game.Core.MasterData
{
    public class MasterData
    {
        public ItemsMasterData ItemsMasterData { get; } = new();
        
        public void Bind()
        {
            ItemsMasterData.Bind();
        }

        public void Start()
        {
            ItemsMasterData.Start();
        }

        public void Unbind()
        {
            ItemsMasterData.Unbind();
        }
    }
}
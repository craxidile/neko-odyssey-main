using NekoOdyssey.Scripts.MiniGame.Core.PlayerItems;
using NekoOdyssey.Scripts.MiniGame.Core.Site;

namespace NekoOdyssey.Scripts.MiniGame.Core
{
    public class MiniGameCore
    {
        public MiniGameSite Site { get; } = new();
        public MiniGamePlayerItems PlayerItems { get; } = new();
        
        public void Bind()
        {
            Site.Bind();
            PlayerItems.Bind();
        }

        public void Start()
        {
            Site.Start();
            PlayerItems.Start();
        }

        public void Unbind()
        {
            Site.Unbind();
            PlayerItems.Unbind();
        }
    }
}
using NekoOdyssey.Scripts.Game.Unity.Game.Core;

namespace NekoOdyssey.Scripts.Game.Core.Player.Bag
{
    public class PlayerBag
    {
        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void Finish()
        {
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
        }
    }
}
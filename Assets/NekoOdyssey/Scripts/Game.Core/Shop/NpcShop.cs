using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Shop
{
    public class NpcShop
    {
        public bool Selling { get; private set; }
        
        public Subject<bool> OnSell { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
            GameRunner.Instance.PlayerInputHandler.OnShopTriggerred
                .Subscribe(_ => SwitchSelling())
                .AddTo(GameRunner.Instance);
        }

        public void Unbind()
        {
        }

        public void SetSelling(bool selling)
        {
            Selling = selling;
            OnSell.OnNext(Selling);
        }

        private void SwitchSelling()
        {
            SetSelling(!Selling);
        }
    }
}
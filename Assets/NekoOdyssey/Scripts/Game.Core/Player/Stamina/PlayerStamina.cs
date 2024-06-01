using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Game.Core.Player.Stamina
{
    public class PlayerStamina
    {
        private static bool _isEnable = true;
        
        private TimeHrMin _startDayTime;

        public int Stamina { get; private set; }

        public Subject<int> OnChangeStamina { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
            _startDayTime = new TimeHrMin(AppConstants.Time.StartDayTime);
            
            GameRunner.Instance.TimeRoutine.OnTimeUpdate
                .Subscribe(UpdateStamina)
                .AddTo(GameRunner.Instance);
            GameRunner.Instance.Core.Player.Bag.OnUseBagItem
                .Subscribe(HandleFoodItemUsage)
                .AddTo(GameRunner.Instance);

            OnChangeStamina
                .Subscribe(CheckStaminaDisable)
                .AddTo(GameRunner.Instance);

            GameRunner.Instance.TimeRoutine.OnChangeDay
                .Subscribe(HandleChangeDay)
                .AddTo(GameRunner.Instance);
        }

        public void Unbind()
        {
        }

        public void ResetStamina()
        {
            Stamina = AppConstants.Stamina.NewDay;
            OnChangeStamina.OnNext(Stamina);
        }

        void UpdateStamina(int deltaTime)
        {
            if (deltaTime < 0) return;
            if (!_isEnable) return;
            var timeRoutine = GameRunner.Instance.TimeRoutine;
            if (timeRoutine.currentTime <= _startDayTime) return;

            var previousStamina = Stamina;
            const int staminaDrainPerSecond = AppConstants.Stamina.NewDay / AppConstants.Stamina.LiveTime;

            var staminaDecrease = staminaDrainPerSecond * timeRoutine.hungryOverTimeMultiplier;
            Debug.Log($"drain stamina : {staminaDecrease}");

            var newStamina = Stamina - Mathf.RoundToInt(staminaDecrease * deltaTime);
            if (newStamina == previousStamina) return;
            
            SetStamina(newStamina);
        }

        public void SetStamina(int newStamina)
        {
            Stamina = Mathf.Min(AppConstants.Stamina.MaxTotal, newStamina);
            OnChangeStamina.OnNext(Stamina);
        }

        public void AddStamina(int value)
        {
            Stamina = Mathf.Min(AppConstants.Stamina.MaxTotal, Stamina + value);
            OnChangeStamina.OnNext(Stamina);
        }

        public void ConsumeStamina(int value)
        {
            Stamina = Mathf.Max(0, Stamina - value);
            OnChangeStamina.OnNext(Stamina);
        }

        private void HandleFoodItemUsage(BagItemV001 usedItem)
        {
            // if (!usedItem.Item.Type.Code.ToLower().Equals($"playerfood")) return;
            Debug.Log($"Food Item Used Add stamina {usedItem.Item.Stamina}");
            AddStamina(usedItem.Item.Stamina);
        }

        private void CheckStaminaDisable(int stamina)
        {
            if (stamina > 0) return;
            _isEnable = false;
        }

        //public void StaminaOutFinish()
        //{
        //    OnStaminaOutFinish.OnNext(Unit.Default);
        //    GameRunner.Instance.Core.Player.SetMode(PlayerMode.Stop);
        //}

        void HandleChangeDay(int dayTotal)
        {
            ResetStamina();
            _isEnable = true;
        }
    }
}
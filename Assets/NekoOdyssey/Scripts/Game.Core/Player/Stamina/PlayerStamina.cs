using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using NekoOdyssey.Scripts.Game.Core.Routine;
using UniRx;
using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Game.Core.Player.Stamina
{
    public class PlayerStamina
    {
        bool _isEnable = true;
        public int Stamina { get; private set; }

        public Subject<int> OnChangeStamina { get; } = new();

        public void Bind()
        {

        }

        public void Start()
        {
            TimeRoutine.OnTimeUpdate.Subscribe(UpdateStamina).AddTo(GameRunner.Instance);
            GameRunner.Instance.Core.Player.Bag.OnUseBagItem.Subscribe(HandleFoodItemUsed).AddTo(GameRunner.Instance);

            OnChangeStamina.Subscribe(CheckStaminaOut).AddTo(GameRunner.Instance);
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
            if (!_isEnable) return;

            int previousStamina = Stamina;
            var staminaDrainPerSecond = AppConstants.Stamina.NewDay / AppConstants.Stamina.LiveTime;

            var staminaDecrease = staminaDrainPerSecond * TimeRoutine.s_hungryOverTimeMultiplier;

            Debug.Log($"drain stamina : {staminaDecrease}");

            var newStamina = Stamina - Mathf.RoundToInt(staminaDecrease * deltaTime);

            if (newStamina != previousStamina)
            {
                SetStamina(newStamina);
            }
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

        void HandleFoodItemUsed(BagItemV001 usedItem)
        {
            //if (usedItem.Item.Type.Code.ToLower().Equals("playerfood"))
            {
                Debug.Log($"Food Item Used Add stamina {usedItem.Item.Stamina}");

                AddStamina(usedItem.Item.Stamina);
            }
        }


        void CheckStaminaOut(int stamina)
        {
            if (stamina > 0) return;
            if (!_isEnable) return;
            _isEnable = false;

            Debug.Log($"player stamina out");

            DOVirtual.DelayedCall(1, () =>
            {
                //play unconsios animation
                //

                //move player to home
                //var homeSite = "MikiHome";
                var homeSite = "GamePlayZone6_01";
                SiteRunner.Instance.Core.Site.SetSite(homeSite);
                //UnityEngine.SceneManagement.SceneManager.LoadScene("SceneLoader");



                _isEnable = true;

                TimeRoutine.NextDay();
                SetStamina(AppConstants.Stamina.NewDay);
            });




        }
    }
}
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using DG.Tweening;

namespace NekoOdyssey.Scripts.Game.Core.EndDay
{
    public class EndDayController
    {
        public const float HomeAnimationDelay = 3f;

        public static EndDayMode endDayMode = EndDayMode.None;
        public static EndDayStep endDayStep = EndDayStep.None;

        public void Bind()
        {

        }

        public void Start()
        {
            GameRunner.Instance.Core.Player.Stamina.OnStaminaOutFinish
                .Subscribe(EndDay_StaminaOut)
                .AddTo(GameRunner.Instance);

            Debug.Log("EndDayController Start");

            if (endDayStep != EndDayStep.None)
            {
                NextEndDayStep();
            }
        }

        public void Unbind()
        {
        }



        void EndDay_StaminaOut(Unit _)
        {
            if (endDayMode != EndDayMode.None) return;

            endDayMode = EndDayMode.StaminaOut;

            //var homeSite = "NekoInside03MikiHouse";

            //SiteRunner.Instance.Core.Site.SetSite(homeSite);

            endDayStep = EndDayStep.MikiHome;


            GameRunner.Instance.Core.GameScene.CloseScene();
            DOVirtual.DelayedCall(4, () =>
            {
                var homeSite = "NekoInside03MikiHouse";
                SiteRunner.Instance.Core.Site.SetSite(homeSite);
            });
        }


        void NextEndDayStep()
        {
            Debug.Log($"NextEndDayStep = {endDayStep} ");
            switch (endDayStep)
            {
                case EndDayStep.None:
                    break;
                case EndDayStep.PlayerAnimation:
                    break;
                case EndDayStep.MikiHome:
                    EndDayStep_MikiHome();
                    break;
                case EndDayStep.Result:
                    break;
                case EndDayStep.NewDay:
                    break;
                default:
                    break;
            }
        }

        void EndDayStep_MikiHome()
        {
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.EndDay_StaminaOut);
            GameRunner.Instance.Core.Player.GameObject.SetActive(false);

            DOVirtual.DelayedCall(HomeAnimationDelay, () =>
            {
                Debug.Log($"EndDayStep_MikiHome delay complete");
                GameRunner.Instance.Core.GameScene.CloseScene();

                DOVirtual.DelayedCall(4, () =>
                {
                    Debug.Log($"EndDayStep_MikiHome delay complete 2");
                    endDayStep = EndDayStep.Result;

                    //var homeSite = "NekoInside03MikiHouse";
                    //SiteRunner.Instance.Core.Site.SetSite(homeSite);
                }, false);
            }, false);
        }



    }

    public enum EndDayMode
    {
        None, StaminaOut, TimeOut,
    }
    public enum EndDayStep
    {
        None, PlayerAnimation, MikiHome, Result, NewDay,
    }

}
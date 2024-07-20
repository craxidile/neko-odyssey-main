using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Linq;

using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using DG.Tweening;
using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Game.Core.EndDay
{
    public class EndDayController
    {
        public const float HomeAnimationDelay = 4f;

        public static EndDayMode endDayMode = EndDayMode.None;
        public static EndDayStep endDayStep = EndDayStep.None;



        #region remove this part after linked with database
        static List<string> temp_visitedEndDayCutscene = new List<string>();
        static Queue<EndDayCutsceneDetail> EndDayCutSceneQuene = new Queue<EndDayCutsceneDetail>();
        static bool isPlayCutsceneToday = false;
        List<EndDayCutsceneDetail> temp_avaliableEndDayCutscene = new List<EndDayCutsceneDetail> { new EndDayCutsceneDetail("NekoInside28BedroomFinal", 200) };
        class EndDayCutsceneDetail
        {
            public string siteName;
            public int followerNeeded;
            public EndDayCutsceneDetail(string siteName, int followerNeeded)
            {
                this.siteName = siteName;
                this.followerNeeded = followerNeeded;
            }
        }
        #endregion


        public Subject<Unit> OnStaminaOutFinish { get; } = new();
        public Subject<Unit> OnTimeOutFinish { get; } = new();

        public void Bind()
        {

        }

        public void Start()
        {
            //GameRunner.Instance.Core.Player.Stamina.OnStaminaOutFinish
            //    .Subscribe(EndDay_StaminaOut)
            //    .AddTo(GameRunner.Instance);

            //GameRunner.Instance.Core.Player.Stamina.OnStaminaOutFinish
            //    .Subscribe(EndDay_StaminaOut)
            //    .AddTo(GameRunner.Instance);

            Debug.Log("EndDayController Start");

            if (endDayStep != EndDayStep.None)
            {
                ProcessEndDayStep();
            }
        }

        public void Unbind()
        {
        }

        public void StaminaOutFinish()
        {
            OnStaminaOutFinish.OnNext(Unit.Default);

            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Stop);

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
        public void TimeOutFinish()
        {
            OnTimeOutFinish.OnNext(Unit.Default);

            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Stop);

            if (endDayMode != EndDayMode.None) return;

            endDayMode = EndDayMode.TimeOut;

            //var homeSite = "NekoInside03MikiHouse";

            //SiteRunner.Instance.Core.Site.SetSite(homeSite);

            endDayStep = EndDayStep.MikiHome;


            GameRunner.Instance.Core.GameScene.CloseScene();
            DOVirtual.DelayedCall(4, () =>
            {
                var homeSite = "NekoInside28Bedroom";
                SiteRunner.Instance.Core.Site.SetSite(homeSite);
            });
        }

        //void EndDay_StaminaOut(Unit _)
        //{

        //}
        //void EndDay_TimeOut(Unit _)
        //{

        //}


        void ProcessEndDayStep()
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
                    ShowResultPanel();
                    break;
                case EndDayStep.EndDayCutscene:
                    PlayEndDayCutscene();
                    break;
                case EndDayStep.NewDay:
                    LoadNewDay();
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

                DOVirtual.DelayedCall(Unity.Uis.SceneFadeCanvas.SceneFadeCanvasController.FadeDuration, () =>
                {
                    Debug.Log($"EndDayStep_MikiHome delay complete 2");
                    endDayStep = EndDayStep.Result;

                    ProcessEndDayStep();

                    //var homeSite = "NekoInside03MikiHouse";
                    //SiteRunner.Instance.Core.Site.SetSite(homeSite);
                }, false);
            }, false);
        }

        void ShowResultPanel()
        {
            //var homeSite = "NekoInside03MikiHouse";
            //SiteRunner.Instance.Core.Site.SetSite(homeSite);

            SceneManager.LoadSceneAsync("EndDayResult", LoadSceneMode.Additive).completed += _ =>
            {
                var endDayResultCanvas = Object.FindFirstObjectByType<EndDayResult_CanvasController>();
                endDayResultCanvas.OnEndDayResultFinish.Subscribe(_ =>
                {
                    CheckEndDayCutscene();
                    //endDayStep = EndDayStep.NewDay;
                    //ProcessEndDayStep();
                })
                .AddTo(endDayResultCanvas);
            };
        }

        void CheckEndDayCutscene()
        {
            //bool isCutsceneAvaliable = false;
            var follower = GameRunner.Instance.Core.Player.FollowerCount;
            //var DemoFinished = GameRunner.Instance.Core.Player.DemoFinished;
            //follower = 200; //ignore real value for now

            foreach (var endDayCutscene in temp_avaliableEndDayCutscene)
            {
                Debug.Log($"check endDay cutscene : {endDayCutscene.siteName} / {endDayCutscene.followerNeeded}");
                if (temp_visitedEndDayCutscene.Contains(endDayCutscene.siteName)) continue;
                if (EndDayCutSceneQuene.Any(cutscne => cutscne.siteName.Equals(endDayCutscene.siteName))) continue;

                if (follower >= endDayCutscene.followerNeeded)
                {
                    Debug.Log($"load end day cutscene : {endDayCutscene.siteName}");

                    //isCutsceneAvaliable = true;

                    EndDayCutSceneQuene.Enqueue(endDayCutscene);
                }
            }


            //if (!_finalSceneLoaded && !DemoFinished && follower >= 200)
            //{
            //    _finalSceneLoaded = true;
            //    Debug.Log($">>load_final<<");

            //    GameRunner.Instance.Core.SaveDbWriter.Add(dbContext =>
            //    {
            //        var repo = new PlayerPropertiesV001Repo(dbContext);
            //        var properties = repo.Load();
            //        properties.DemoFinished = true;
            //        OnFinishDemo.OnNext(default);
            //        repo.Update(properties);
            //    });

            //    DOVirtual.DelayedCall(2f, () =>
            //    {
            //        GameRunner.Instance.Core.GameScene.CloseScene();
            //        DOVirtual.DelayedCall(4f,
            //            () => { SiteRunner.Instance.Core.Site.SetSite("NekoInside28BedroomFinal"); });
            //    });
            //}


            //if (isCutsceneAvaliable)
            if (EndDayCutSceneQuene.Count > 0)
            {
                endDayStep = EndDayStep.EndDayCutscene;
            }
            else
            {
                endDayStep = EndDayStep.NewDay;
            }
            ProcessEndDayStep();

            //PlayEndDayCutscene();
        }

        void PlayEndDayCutscene()
        {
            if (!isPlayCutsceneToday)
            {
                Debug.Log("PlayEndDayCutscene");
                var cutsceneDetail = EndDayCutSceneQuene.Dequeue();

                isPlayCutsceneToday = true;

                temp_visitedEndDayCutscene.Add(cutsceneDetail.siteName);


                GameRunner.Instance.Core.GameScene.CloseScene();
                GameRunner.Instance.Core.GameScene.OnChangeSceneFinish.Subscribe(_ =>
                {
                    SiteRunner.Instance.Core.Site.SetSite(cutsceneDetail.siteName);
                });

            }
            else
            {
                endDayStep = EndDayStep.NewDay;
                ProcessEndDayStep();
            }




        }


        void LoadNewDay()
        {
            endDayStep = EndDayStep.None;
            endDayMode = EndDayMode.None;

            var newDaySite = "GamePlayZone3_02";
            SiteRunner.Instance.Core.Site.SetSite(newDaySite);

            GameRunner.Instance.TimeRoutine.SetTime(AppConstants.Time.StartDayTime);
            GameRunner.Instance.TimeRoutine.NextDay();

            GameRunner.Instance.TimeRoutine.ContinueTime();

            isPlayCutsceneToday = false;
        }




    }

    public enum EndDayMode
    {
        None, StaminaOut, TimeOut,
    }
    public enum EndDayStep
    {
        None, PlayerAnimation, MikiHome, Result, NewDay,
        EndDayCutscene,
    }

}
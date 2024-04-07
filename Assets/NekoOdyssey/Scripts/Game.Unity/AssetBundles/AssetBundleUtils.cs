using System;
using DG.Tweening;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Unity.AssetBundles
{
    public class AssetBundleUtils
    {
        public static void OnReady(Action action)
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                if (GameRunner.Instance.Ready)
                {
                    action();
                }
                else
                {
                    GameRunner.Instance.OnReady.Subscribe(ready =>
                    {
                        if (!ready) return;
                        action();
                    }).AddTo(GameRunner.Instance);
                }
            });
        }
    }
}
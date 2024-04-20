using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NekoOdyssey.Scripts.Game.Core.Routine
{
    public class Routine
    {
        public Routine()
        {

        }

        public void Bind()
        {
        }

        public void Start()
        {
            AssetBundleUtils.OnReady(() =>
            {
                //var a = GameRunner.Instance.AssetMap["daytime"] as ScriptableObject;
                var CSVHolder = GameRunner.Instance.AssetMap["CSVHolder".ToLower()] as CSVHolderScriptable;
                var currentTime  = GameRunner.Instance.AssetMap["CurrentTime".ToLower()] as TimeScriptable;
                Debug.Log($"test load scriptable asset bundle {currentTime.currentTimeText}");
            });
        }

        public void Unbind()
        {
        }

    }
}

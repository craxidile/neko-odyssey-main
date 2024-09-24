using System;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts
{
    public class MiniGameTest : MonoBehaviour
    {
        public Button lv1Button;
        public Button lv2Button;
        public Button lv3Button;
        public Button lv4Button;

        private void Start()
        {
            lv1Button.onClick.AddListener(() => MoveToGameSite(1));
            lv2Button.onClick.AddListener(() => MoveToGameSite(2));
            lv3Button.onClick.AddListener(() => MoveToGameSite(3));
            lv4Button.onClick.AddListener(() => MoveToGameSite(4));
        }

        private void MoveToGameSite(int level)
        {
            SiteRunner.Instance.Core.Site.SetSite("MiniGameFishing", true, level);
        }
    }
}
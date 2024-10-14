using NekoOdyssey.Scripts.Game.Unity.Uis.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Scenes
{
    public class DemoTitleManager : MonoBehaviour
    {
        public Text newGameText;

        private void Start()
        {
            var localiser = newGameText.GetComponent<UiTextLocaliser>();
            if (!localiser) return;
            if (SiteRunner.Instance.Core.Site.GameStarted) localiser.OriginalText = "Continue";
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

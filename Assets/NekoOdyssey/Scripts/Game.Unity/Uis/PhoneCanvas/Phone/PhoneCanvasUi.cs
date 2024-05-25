using System;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone
{
    [Serializable]
    public class PhoneCanvasUi
    {
        public PlayerPhoneApp phoneApp;
        public CanvasGroup canvasGroup;
        public GameObject grid;
        public GameObject gridCell;
        public ScrollRect scrollRect;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas
{
    public class DialogCanvasController : MonoBehaviour
    {
        public Text messageBox;
        public Animator animator;


        public void SetText(string text)
        {
            messageBox.text = text;

            Canvas.ForceUpdateCanvases();
            messageBox.gameObject.SetActive(false);
            messageBox.gameObject.SetActive(true);



        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Unity.Uis.Utils;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas
{
    public class DialogCanvasController : MonoBehaviour
    {
        public Text messageBox;
        public GameObject dialogBalloon;
        [SerializeField] Animator animator;
        public Image dialogImageFrame;
        // For Cutscene
        [HideInInspector] public bool lastLineId;
        [HideInInspector] public bool goNextLineId;
        [HideInInspector] public bool endDialogue;
        public void SetText(string text)
        {
            messageBox.text = ThaiGlyphAdjuster.Adjust(text);

            Canvas.ForceUpdateCanvases();
            messageBox.gameObject.SetActive(false);
            messageBox.gameObject.SetActive(true);
        }

        public void SetOpened(bool isOpen)
        {
            dialogBalloon.SetActive(isOpen);

            //gameObject.transform.localScale = Vector3.zero; //change back later;
        }
    }
}
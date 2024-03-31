using System;
using NekoOdyssey.Scripts.Game.Core.Cat;
using UniRx;
using UnityEditor.Animations;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Cat
{
    public class CatController : MonoBehaviour
    {
        private Animator _emojiAnimator;
        private Core.Cat.Cat _cat;

        public string code;
        public GameObject emoji;

        private void Awake()
        {
            emoji.SetActive(false);
            _emojiAnimator = emoji.GetComponent<Animator>();

            _cat = GameRunner.Instance.Core.Cats.RegisterCat(code);
        }

        private void Start()
        {
            _cat.OnChangeEmotion
                .Subscribe(HandleEmotion)
                .AddTo(this);
        }

        private void HandleEmotion(CatEmotion emotion)
        {
            AnimatorController animatorController = null;
            switch (emotion)
            {
                case CatEmotion.Love:
                    animatorController = GameRunner.Instance.AssetMap["LoveLoveAnimator".ToLower()]
                        as AnimatorController;
                    break;
                case CatEmotion.BadTempered:
                    animatorController = GameRunner.Instance.AssetMap["AngryAnimator".ToLower()]
                        as AnimatorController;
                    break;
                case CatEmotion.Hungry:
                    break;
                case CatEmotion.Happy:
                    break;
                case CatEmotion.None:
                default:
                    break;
            }

            if (animatorController != null)
                _emojiAnimator.runtimeAnimatorController = animatorController;


            emoji.SetActive(emotion != CatEmotion.None);
        }
    }
}
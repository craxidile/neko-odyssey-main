using NekoOdyssey.Scripts.Game.Core.Cat;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Cats
{
    public class CatController : MonoBehaviour
    {
        private Animator _emojiAnimator;
        private Animator _catAnimator;
        private Cat _cat;

        public string code;
        public GameObject emoji;

        private void Awake()
        {
            emoji.SetActive(false);
            _catAnimator = GetComponent<Animator>();
            _emojiAnimator = emoji.GetComponent<Animator>();

        }

        private void Start()
        {
            _cat = GameRunner.Instance.Core.Cats.RegisterCat(code);
            _cat.OnChangeEmotion
                .Subscribe(HandleEmotion)
                .AddTo(this);
            _cat.OnEat
                .Subscribe(HandleEat)
                .AddTo(this);
        }

        private void HandleEmotion(CatEmotion emotion)
        {
            RuntimeAnimatorController animatorController = null;
            switch (emotion)
            {
                case CatEmotion.Love:
                    animatorController = GameRunner.Instance.AssetMap["LoveLoveAnimator".ToLower()]
                        as RuntimeAnimatorController;
                    break;
                case CatEmotion.BadTempered:
                    animatorController = GameRunner.Instance.AssetMap["AngryAnimator".ToLower()]
                        as RuntimeAnimatorController;
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

        private void HandleEat(bool eating)
        {
            _catAnimator.SetBool($"Eat", eating);
        }
    }
}
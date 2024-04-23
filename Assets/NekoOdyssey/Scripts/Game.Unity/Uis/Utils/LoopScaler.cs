using DG.Tweening;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.Utils
{
    public class LoopScaler : MonoBehaviour
    {
        public float startValue = 1f, endValue = 1.2f;
        public float duration = 1f;
    
        public bool enable = true;
        bool _tempEnable = false;
    
        void Update()
        {
            if (_tempEnable != enable)
            {
                _tempEnable = enable;
    
                if (enable)
                {
                    StartScaler();
                }
                else
                {
                    transform.DOKill();
                    transform.DOScale(startValue, duration);
                }
            }
        }
    
        void StartScaler()
        {
            transform.DOScale(startValue, 0);
            transform.DOScale(endValue, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    
        public void SetEnabled(bool condition)
        {
            enable = condition;
            Update();
        }
    }
}
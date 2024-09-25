using UnityEngine;

namespace NekoOdyssey.Scripts.MiniGame.Core.Site
{
    public class MiniGameSite
    {
        private static int _siteValue;
        private static Vector3 _previousPlayerPosition;
        
        public int SiteValue => _siteValue;
        public Vector3 PreviousPlayerPosition => _previousPlayerPosition;
        
        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void SetPreviousSite(int value, Vector3 position)
        {
            _siteValue = value;
            _previousPlayerPosition = position;
        }
    }
}
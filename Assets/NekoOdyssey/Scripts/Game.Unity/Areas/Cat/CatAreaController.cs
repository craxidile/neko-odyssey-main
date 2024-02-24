using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Areas.Cat
{
    public class CatAreaController : MonoBehaviour
    {
        private void Awake()
        {
            var areaCollider = GetComponent<Collider>();
            var bounds = areaCollider.bounds;
            var catArea = GameRunner.Instance.Core.Areas.RegisterCatArea(bounds);
            catArea.GameObject = gameObject;
        }
    }
}
using System.Collections;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Ais.Cat.Behaviours.Move
{
    public class MoveController : MonoBehaviour
    {
        private bool _executing;

        private CatAi _catAi;
        private CatBehaviourController _behaviourController;

        private void Awake()
        {
            var behaviourController = GetComponent<CatBehaviourController>();
            _catAi = behaviourController.CatAi;
        }

        private void Start()
        {
            _catAi.OnCatMove
                .Subscribe(HandleFollowPlayer)
                .AddTo(this);
        }

        private void HandleFollowPlayer(Vector3 position)
        {
            StartCoroutine(MoveToPosition(position));
        }

        private IEnumerator MoveToPosition(Vector3 position)
        {
            transform.Translate(position, Space.World);
            yield return null;
            _catAi.SetCatPosition(transform.position);
        }
    }
}
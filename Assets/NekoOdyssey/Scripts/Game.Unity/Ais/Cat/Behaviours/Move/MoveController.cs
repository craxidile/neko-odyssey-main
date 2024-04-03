using System.Collections;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Ais.Cat.Behaviours.Move
{
    public class MoveController : MonoBehaviour
    {
        private bool _executing;

        private CatAi _catAi;
        private CatBehaviourController _behaviourController;
        private Animator _animator;
        private SpriteRenderer _renderer;
        private bool _running;
        private bool _moving;
        private bool _flipped;
        private Coroutine _currentCoroutine;

        private void Awake()
        {
            var behaviourController = GetComponent<CatBehaviourController>();
            _catAi = behaviourController.CatAi;
            _renderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _catAi.OnFlip
                .Subscribe(HandleFlip)
                .AddTo(this);
            _catAi.OnChangeMode
                .Subscribe(HandleModeChange)
                .AddTo(this);
            _catAi.OnCatMove
                .Subscribe(HandleMove)
                .AddTo(this);
        }

        private void HandleFlip(bool flipped)
        {
            _flipped = flipped;
        }

        private void HandleModeChange(CatBehaviourMode mode)
        {
            _running = _catAi.Mode == CatBehaviourMode.FollowPlayer;
            if (!_running && _currentCoroutine != null) StopCoroutine(_currentCoroutine);
            if (_running) _catAi.WaitingToWalk = true;
            if (!_running) _animator.SetBool($"Move", false);
        }

        private void HandleMove(Vector3 position)
        {
            _animator.SetBool($"Move", true);
            _currentCoroutine = StartCoroutine(MoveToPosition(position));
        }

        private IEnumerator MoveToPosition(Vector3 position)
        {
            if (!_running) yield break;
            if (_catAi.WaitingToWalk)
            {
                yield return null;
                _catAi.SetCatPosition(transform.position);
                yield break;
            }

            _renderer.flipX = _flipped;
            transform.Translate(position, Space.World);
            yield return null;
            _catAi.SetCatPosition(transform.position);
        }
    }
}
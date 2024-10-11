using System.Collections;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Player.Feed;
using NekoOdyssey.Scripts.Game.Unity.Player.Petting;
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
            _catAi.OnReadyToWalk
                .Subscribe(HandleReadyToWalk)
                .AddTo(this);
            _catAi.OnChangeMode
                .Subscribe(HandleModeChange)
                .AddTo(this);
            _catAi.OnCatStartMoving
                .Subscribe(HandleStartMoving)
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
            if (!_running) _animator.SetBool($"Move", false);
        }

        private void HandleStartMoving(Unit _)
        {
            if (!_running) return;
            _animator.SetBool($"Sit", false);
            _animator.SetBool($"Eat", false);
            _animator.SetBool($"Move", true);
        }

        private void HandleReadyToWalk(bool ready)
        {
            if (!_running || !ready) return;
            _renderer.flipX = _flipped;
        }

        private void HandleMove(Vector3 position)
        {
            if (!_running) return;

            if (GameRunner.Instance.Core.Player.Mode == PlayerMode.Feed)
            {
                _animator.SetBool($"Move", false);
                _animator.SetBool($"Eat", true);
                DOVirtual.DelayedCall(PlayerFeedController.FeedDelay, () =>
                {
                    _animator.SetBool($"Eat", false);
                    _animator.SetBool($"Move", true);
                    _currentCoroutine = StartCoroutine(MoveToPosition(Vector3.zero));
                });
                return;
            }

            if (GameRunner.Instance.Core.Player.Mode == PlayerMode.Pet)
            {
                _animator.SetBool($"Move", false);
                _animator.SetBool($"Sit", true);
                DOVirtual.DelayedCall(PlayerPettingController.PettingDelay, () =>
                {
                    _animator.SetBool($"Sit", false);
                    _animator.SetBool($"Move", true);
                    _currentCoroutine = StartCoroutine(MoveToPosition(Vector3.zero));
                });
                return;
            }

            _animator.SetBool($"Move", true);
            _currentCoroutine = StartCoroutine(MoveToPosition(position));
        }

        private IEnumerator MoveToPosition(Vector3 position)
        {
            transform.Translate(_catAi.Walkable ? position : Vector3.zero, Space.World);
            yield return null;
            _catAi.SetCatPosition(transform.position);

            //Debug.Log($"MoveToPosition {position} ({Time.time})");
        }
    }
}
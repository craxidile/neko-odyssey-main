using System;
using System.Net;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours.FollowPlayer
{
    public class FollowPlayerBehaviour : BaseCatBehaviour
    {
        private const float MinDistance = .2f;
        private const float MaxDistance = .6f;

        private float _coolDownDelay = 0f;
        private Vector3? _readyToWalkPosition;
        private Vector3? _targetPositionOnArea;

        private IDisposable _catStartPositionSubscription;
        private IDisposable _catPositionSubscription;
        private IDisposable _readyToWalkSubscription;

        public FollowPlayerBehaviour(CatAi catAi) : base(catAi)
        {
        }

        public override void Start()
        {
            CatAi.SetReadyToWalk(false);
            _targetPositionOnArea = null;

            _catStartPositionSubscription = CatAi.OnChangeCatStartPosition
                .Subscribe(HandleCatStartPosition)
                .AddTo(GameRunner.Instance);
            _catPositionSubscription = CatAi.OnChangeCatPosition
                .Subscribe(HandleCatPosition)
                .AddTo(GameRunner.Instance);
            _readyToWalkSubscription = CatAi.OnReadyToWalk
                .Subscribe(HandleReadyToWalk)
                .AddTo(GameRunner.Instance);
        }

        private void HandleCatStartPosition(Vector3 catPosition)
        {
            if (CatAi.Mode != CatBehaviourMode.FollowPlayer) return;
            
            _readyToWalkPosition = null;

            var playerPosition = GameRunner.Instance.Core.Player.Position;
            var playerRefPosition = playerPosition;
            playerRefPosition.y = catPosition.y;
            if (Vector3.Distance(catPosition, playerRefPosition) <= 0.65f)
            {
                End();
                return;
            }

            var randStandDistance = Random.Range(MinDistance, MaxDistance);
            var targetPosition = playerPosition + (catPosition - playerPosition).normalized * randStandDistance;
            targetPosition.y = catPosition.y;

            var catAreas = GameRunner.Instance.Core.Areas;
            var closestPoint = catAreas.CalculateClosestPoint(targetPosition);
            closestPoint.y = catPosition.y;
            _targetPositionOnArea = closestPoint;

            CatAi.OnFlip.OnNext(CatAi.DeltaXFromPlayer > 0);

            var nextPosition = CalculateNextPosition(catPosition, _targetPositionOnArea.Value);
            if (nextPosition == null)
            {
                End();
                return;
            }

            _readyToWalkPosition = nextPosition;
            CatAi.OnCatStartMoving.OnNext(default);
        }

        private void HandleCatPosition(Vector3 catPosition)
        {
            if (CatAi.Mode != CatBehaviourMode.FollowPlayer) return;

            if (_targetPositionOnArea == null)
            {
                End();
                return;
            }

            var nextPosition = CalculateNextPosition(catPosition, _targetPositionOnArea.Value);
            Debug.Log($">>change_mode<< next_position {nextPosition}");
            if (nextPosition == null)
            {
                End();
                return;
            }

            if (_targetPositionOnArea != null) CatAi.OnCatMove.OnNext(nextPosition.Value);
        }

        private void HandleReadyToWalk(bool ready)
        {
            if (CatAi.Mode != CatBehaviourMode.FollowPlayer || _readyToWalkPosition == null) return;
            CatAi.OnCatMove.OnNext(_readyToWalkPosition.Value);
        }

        private Vector3? CalculateNextPosition(Vector3 catPosition, Vector3 targetPosition)
        {
            var distanceToTarget = Vector3.Distance(catPosition, targetPosition);
            if (distanceToTarget <= 0.1f) return null;

            var moveDirection = (targetPosition - catPosition).normalized;
            var moveRange = Mathf.Min(
                CatAi.Profile.MoveSpeed * Time.deltaTime,
                Vector3.Distance(catPosition, targetPosition)
            );

            var nextPosition = moveDirection * moveRange;
            nextPosition.y = 0;

            return nextPosition;
        }

        private void End()
        {
            Debug.Log($">>change_mode<< end");
            _catStartPositionSubscription?.Dispose();
            _catPositionSubscription?.Dispose();
            _readyToWalkSubscription?.Dispose();
            _targetPositionOnArea = null;
            _readyToWalkPosition = null;
            CatAi.OnFinishBehaviour.OnNext(CatBehaviourMode.FollowPlayer);
        }
    }
}
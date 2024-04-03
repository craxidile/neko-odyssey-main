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
        private IDisposable _catPositionSubscription;

        public FollowPlayerBehaviour(CatAi catAi) : base(catAi)
        {
        }

        public override void Start()
        {
            //Debug.Log($">>modes<< follow_player start");
            CatAi.SetReadyToWalk(false);
            _targetPositionOnArea = null;

            CatAi.OnChangeCatPosition
                .Subscribe(HandleCatPosition)
                .AddTo(GameRunner.Instance);
            CatAi.OnReadyToWalk
                .Subscribe(HandleReadyToWalk)
                .AddTo(GameRunner.Instance);
        }

        private void HandleReadyToWalk(bool ready)
        {
            if (_readyToWalkPosition == null);
            CatAi.OnCatMove.OnNext(_readyToWalkPosition.Value);
        }

        private void HandleCatPosition(Vector3 catPosition)
        {
            if (CatAi.Mode != CatBehaviourMode.FollowPlayer) return;

            var playerPosition = GameRunner.Instance.Core.Player.Position;

            if (_targetPositionOnArea == null)
            {
                Debug.Log($">>change_mode<< target_position {_targetPositionOnArea}");
                var playerRefPosition = playerPosition;
                playerRefPosition.y = catPosition.y;
                Debug.Log($">>change_mode<< >>distance<< {Vector3.Distance(catPosition, playerRefPosition)}");
                if (Vector3.Distance(catPosition, playerRefPosition) <= 0.65f)
                {
                    Debug.Log($">>chane_mode<< >>distance<< exit");
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
            }

            var distanceToTarget = Vector3.Distance(catPosition, _targetPositionOnArea.Value);

            if (distanceToTarget <= 0.1f)
            {
                End();
                return;
            }

            var moveDirection = (_targetPositionOnArea.Value - catPosition).normalized;
            var moveRange = Mathf.Min(
                CatAi.Profile.MoveSpeed * Time.deltaTime,
                Vector3.Distance(catPosition, _targetPositionOnArea.Value)
            );

            var nextPosition = moveDirection * moveRange;
            nextPosition.y = 0;

            if (CatAi.ReadyToWalk)
            {
                CatAi.OnCatMove.OnNext(nextPosition);
            }
            else if (_readyToWalkPosition == null)
            {
                Debug.Log($">>change_mode<< start_moving");
                _readyToWalkPosition = nextPosition;
                CatAi.OnCatStartMoving.OnNext(default);
            }
        }


        private void End()
        {
            Debug.Log($">>change_mode<< end");
            _catPositionSubscription?.Dispose();
            _targetPositionOnArea = null;
            _readyToWalkPosition = null;
            CatAi.OnFinishBehaviour.OnNext(CatBehaviourMode.FollowPlayer);
        }
    }
}
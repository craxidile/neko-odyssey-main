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
        private Vector3? _targetPositionOnArea;
        private IDisposable _catPositionSubscription;

        public FollowPlayerBehaviour(CatAi catAi) : base(catAi)
        {
        }

        public override void Start()
        {
            //Debug.Log($">>modes<< follow_player start");
            _targetPositionOnArea = null;
            _catPositionSubscription = CatAi
                .OnChangeCatPosition
                .Subscribe(HandleCatPosition);
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
                Debug.Log($">>distance<< {Vector3.Distance(catPosition, playerRefPosition)}");
                if (Vector3.Distance(catPosition, playerRefPosition) <= 0.65f)
                {
                    Debug.Log($">>distance<< exit");
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

            var moveDuration = distanceToTarget / CatAi.Profile.MoveSpeed;

            // catAi.SetFlipToTarget(player.transform.position);
            // catAi.animator.SetBool("Move", true);

            // CoolDown(moveDuration);

            var moveDirection = (_targetPositionOnArea.Value - catPosition).normalized;
            var moveRange = Mathf.Min(
                CatAi.Profile.MoveSpeed * Time.deltaTime,
                Vector3.Distance(catPosition, _targetPositionOnArea.Value)
            );

            var nextPosition = moveDirection * moveRange;
            nextPosition.y = 0; // catPosition.y;

            CatAi.OnCatMove.OnNext(nextPosition);
        }


        private void End()
        {
            Debug.Log($">>move_end<<");
            _catPositionSubscription.Dispose();
            _targetPositionOnArea = null;
            CatAi.OnFinishBehaviour.OnNext(CatBehaviourMode.FollowPlayer);
        }
    }
}
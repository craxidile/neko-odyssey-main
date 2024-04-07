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
        private Vector3 _targetPositionOnArea = Vector3.zero;
        private IDisposable _catPositionSubscription;

        public FollowPlayerBehaviour(CatAi catAi) : base(catAi)
        {
        }

        public override void Start()
        {
            //Debug.Log($">>modes<< follow_player start");
            _targetPositionOnArea = Vector3.zero;
            _catPositionSubscription = CatAi.OnChangeCatPosition
                .Subscribe(HandleCatPosition);
        }


        private void HandleCatPosition(Vector3 catPosition)
        {
            if (CatAi.Mode != CatBehaviourMode.FollowPlayer) return;

            var playerPosition = GameRunner.Instance.Core.Player.Position;

            if (_targetPositionOnArea == Vector3.zero)
            {
                //Debug.Log($">>modes<< call_to_feed initialize");
                var randStandDistance = Random.Range(MinDistance, MaxDistance);

                var targetPosition = playerPosition + (catPosition - playerPosition).normalized * randStandDistance;
                targetPosition.y = catPosition.y;

                var catAreas = GameRunner.Instance.Core.Areas;
                _targetPositionOnArea = catAreas.CalculateClosestPoint(targetPosition);
                _targetPositionOnArea.y = catPosition.y;
            }

            if (Vector3.Distance(_targetPositionOnArea, catPosition) <= 0.01f)
            {
                //Debug.Log($">>modes<< call_to_feed end");
                End();
                return;
            }

            var distanceToTarget = Vector3.Distance(catPosition, _targetPositionOnArea);

            var moveDuration = distanceToTarget / CatAi.Profile.MoveSpeed;

            CatAi.OnFlip.OnNext(CatAi.DeltaXFromPlayer > 0);
            // catAi.SetFlipToTarget(player.transform.position);
            // catAi.animator.SetBool("Move", true);

            CoolDown(moveDuration);

            var moveDirection = (_targetPositionOnArea - catPosition).normalized;
            var moveRange = Mathf.Min(
                CatAi.Profile.MoveSpeed * Time.deltaTime,
                Vector3.Distance(catPosition, _targetPositionOnArea)
            );

            //Debug.Log($"move range {moveRange} ({Time.time})");

            var nextPosition = moveDirection * moveRange;
            nextPosition.y = 0; // catPosition.y;

            CatAi.OnCatMove.OnNext(nextPosition);
        }


        private void End()
        {
            _catPositionSubscription.Dispose();
            _targetPositionOnArea = Vector3.zero;
            CatAi.OnFinishBehaviour.OnNext(Unit.Default);
        }
    }
}
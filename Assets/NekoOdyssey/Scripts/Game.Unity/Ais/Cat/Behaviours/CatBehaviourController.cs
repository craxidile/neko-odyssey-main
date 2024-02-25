using System;
using System.Collections;
using System.Linq;
using UniRx;
using DG.Tweening;
using NekoOdyssey.Scripts.DataSerializers.Csv.CatProfile;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat;
using NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours;
using NekoOdyssey.Scripts.Game.Unity.Ais.Cat.Behaviours.CallToFeed;
using NekoOdyssey.Scripts.Game.Unity.Ais.Cat.Behaviours.Move;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Models;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Ais.Cat.Behaviours
{
    public class CatBehaviourController : MonoBehaviour
    {
        private readonly string _catProfilesAssetName = $"cat_profiles";

        private SpriteRenderer _spriteRenderer;
        
        public string catCode;

        public CatAi CatAi { get; private set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
           AssetBundleUtils.OnReady(CreateCatAi); 
        }

        private void Update()
        {
            if (CatAi == null) return;
            var playerPosition = GameRunner.Instance.Core.Player.GameObject.transform.position;
            var catPosition = transform.position;
            var distance = Vector3.Distance(playerPosition,catPosition);
            var mainCamera = GameRunner.Instance.cameras.mainCamera;
            var delta = mainCamera.WorldToScreenPoint(playerPosition) -
                         mainCamera.WorldToScreenPoint(catPosition);
            CatAi.SetPlayerDistance(distance, delta.x);
        }

        private void CreateCatAi()
        {
            var ais = GameRunner.Instance.Core.Ais;
            var serializer = new CatProfileSerializer();
            
            var assetMap = GameRunner.Instance.AssetMap;
            if (!assetMap.ContainsKey(_catProfilesAssetName)) return;

            var text = assetMap[_catProfilesAssetName].ToString();
            var columns = serializer.DeserializeHeadColumns(text);
            
            var index = columns.ToList().IndexOf(catCode);
            if (index == -1) return;

            var catProfile = serializer.DeserializeLines(text, index);
            
            CatAi = ais.RegisterCatAi(catProfile);
            CatAi.Start();
            
            gameObject.AddComponent<CallToFeedController>();
            gameObject.AddComponent<MoveController>();

            CatAi.OnFlip.Subscribe(HandleCatFlip);
            CatAi.OnChangeMode.Subscribe(HandleCatBehaviourModeChange);
        }

        private void HandleCatFlip(bool flipped)
        {
            _spriteRenderer.flipX = flipped;
        }

        private void HandleCatBehaviourModeChange(CatBehaviourMode _)
        {
            StartCoroutine(DelayedSetCatPosition());
        }

        private IEnumerator DelayedSetCatPosition()
        {
            yield return null;
            CatAi.SetCatPosition(transform.position);
        }
        
    }
}
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Player;
using UnityEngine;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Unity.Petting
{
    public class CentralPlayerPettingHandler : MonoBehaviour
    {
        private Core.Player.Player _player;
        private global::NekoOdyssey.Scripts.Game.Core.PlayerMenu.PlayerMenu _playerMenu;
        private Core.Player.Petting.PlayerPetting _petting;
        
        private void Awake()
        {
            _player = GameRunner.Instance.Core.Player;
            _playerMenu = GameRunner.Instance.Core.PlayerMenu;
            _petting = GameRunner.Instance.Core.Player.Petting;
        }


        private void Start()
        {
            _playerMenu.OnCommitAction
                .Subscribe(HandlePlayerMenuAction)
                .AddTo(this);
        }

        private void HandlePlayerMenuAction(PlayerMenuAction action)
        {
            if (action != PlayerMenuAction.Pet) return;
            _playerMenu.SetActive(false);
            HandlePetAction();
        }

        private void HandlePetAction()
        {
            var menuGameObject = _playerMenu.GameObject;
            var attributes = menuGameObject.GetComponent<PettingAttributes>();
            if (attributes == null) return;

            _petting.Mode = attributes.pettingMode;
            _petting.TargetPosition = attributes.pettingAnchor.position;
            GameRunner.Instance.Core.Cats.CurrentCatCode = attributes.catCode;
            
            _player.SetMode(PlayerMode.Pet);
        }
    }
}
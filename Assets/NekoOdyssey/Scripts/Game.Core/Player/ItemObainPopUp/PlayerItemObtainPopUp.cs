using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Uis.ItemObtain;
using NekoOdyssey.Scripts.Game.Unity.Inputs;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;

namespace NekoOdyssey.Scripts.Game.Core.Player.ItemObain
{

    public class PlayerItemObtainPopUp
    {
        public GameObject GameObject { get; set; }


        public Subject<ItemObtainPopUpDetail> OnShowPopUp { get; } = new();

        public void Bind()
        {

        }

        public void Start()
        {
            var playerInputHandler = GameRunner.Instance.PlayerInputHandler;

            GameRunner.Instance.PlayerInputHandler.OnBagTriggerred
                .Subscribe(_ => ShowPopUp(new Item()))
                .AddTo(GameRunner.Instance);
        }

        public void Unbind()
        {

        }



        public void ShowPopUp(Item item)
        {
            var popUpDetail = new ItemObtainPopUpDetail();

            OnShowPopUp.OnNext(popUpDetail);
        }
    }
}

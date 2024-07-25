using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Uis.ItemObtain;
using NekoOdyssey.Scripts.Game.Unity.Inputs;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using System.Linq;

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
            //var playerInputHandler = GameRunner.Instance.PlayerInputHandler;

            //GameRunner.Instance.PlayerInputHandler.OnBagTriggerred
            //    .Subscribe(_ => ShowPopUp())
            //    .AddTo(GameRunner.Instance);


            //GameRunner.Instance.Core.Player.Bag.AddBagItem(item);

            //GameRunner.Instance.Core.Player.ItemObtainPopUp.ShowPopUp("PlayerFood001");
        }

        public void Unbind()
        {

        }



        public void ShowPopUp(string itemCode)
        {
            var masterItems = GameRunner.Instance.Core.MasterData.ItemsMasterData.Items.ToList();

            var item = masterItems.FirstOrDefault(i => i.Code == itemCode);

            var popUpDetail = new ItemObtainPopUpDetail(item , 1);

            OnShowPopUp.OnNext(popUpDetail);
        }
    }
}

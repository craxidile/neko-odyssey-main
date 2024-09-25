using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Uis.ConfirmationPanel;

namespace NekoOdyssey.Scripts.Game.Core.Player.ConfirmationPanel
{

    public class PlayerConfirmationPanel
    {
        public ConfirmationPanelController canvasPanel { get; set; }


        public Subject<Unit> OnShowPanel { get; } = new();

        public void Bind()
        {

        }

        public void Start()
        {

        }

        public void Unbind()
        {

        }


        public void ShowConfirmation(string titleName, string description, UnityAction confirmCallback = null, UnityAction cancelCallback = null)
        {
            canvasPanel.SetDescription(titleName, description);
            canvasPanel.OnConfirmation_OneTime += confirmCallback;
            canvasPanel.OnCancellation_OneTime += cancelCallback;

            canvasPanel.SetVisible(true);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NekoOdyssey.Scripts.Game.Core.Routine;

public class PlayerStamina : MonoBehaviour
{
    public const float LiveTime = 360; //how long (in-game minute) player can stay idle with 100 stamina

    public float stamina { get; set; }

    private void Start()
    {
        ResetStamina();

        //TimeRoutine.OnTimeUpdate.Subscribe(Update).AddTo(this);
    }
    public void ResetStamina()
    {
        stamina = 100;
    }
    //private void Update()
    //{
    //    var staminaDrainPerSecond =
    //}
}

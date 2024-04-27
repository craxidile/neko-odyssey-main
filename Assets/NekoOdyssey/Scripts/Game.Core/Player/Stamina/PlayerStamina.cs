using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NekoOdyssey.Scripts.Game.Core.Routine;
using UniRx;
using NekoOdyssey.Scripts;

public class PlayerStamina
{
    public const float LiveTime = 360; //how long (in-game minute) player can stay idle with 100 stamina

    float _stamina;
    public int Stamina => Mathf.RoundToInt(_stamina);

    public Subject<float> OnStaminaUpdate { get; } = new();

    public void Bind()
    {

    }

    public void Start()
    {
        ResetStamina();

        TimeRoutine.OnTimeUpdate.Subscribe(UpdateStamina).AddTo(GameRunner.Instance);

    }
    public void Unbind()
    {
    }




    public void ResetStamina()
    {
        _stamina = 100;
    }


    public void UpdateStamina(int deltaTime)
    {
        var staminaDrainPerSecond = 100f / LiveTime;

        _stamina -= staminaDrainPerSecond * deltaTime;

    }
}

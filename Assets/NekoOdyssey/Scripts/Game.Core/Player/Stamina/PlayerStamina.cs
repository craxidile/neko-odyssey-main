using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NekoOdyssey.Scripts.Game.Core.Routine;
using UniRx;
using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;

public class PlayerStamina
{
    public const float MaxStamina = 200;
    public const float LiveTime = 360; //how long (in-game minute) player can stay idle with 100 stamina

    float _stamina;
    public int Stamina => Mathf.RoundToInt(_stamina);

    public Subject<float> OnStaminaChange { get; } = new();

    public void Bind()
    {

    }

    public void Start()
    {
        ResetStamina();

        TimeRoutine.OnTimeUpdate.Subscribe(UpdateStamina).AddTo(GameRunner.Instance);
        GameRunner.Instance.Core.Player.Bag.OnUseBagItem.Subscribe(HandleFoodItemUsed).AddTo(GameRunner.Instance);

    }
    public void Unbind()
    {
    }




    public void ResetStamina()
    {
        _stamina = 100;
    }


    void UpdateStamina(int deltaTime)
    {
        int previousStamina = Stamina;
        var staminaDrainPerSecond = 100f / LiveTime;

        _stamina -= staminaDrainPerSecond * deltaTime;

        if (Stamina != previousStamina)
        {
            OnStaminaChange.OnNext(Stamina - previousStamina);
        }
    }

    public void IncreaseStamina(float staminaValue)
    {
        var previousStamina = _stamina;
        _stamina = Mathf.Min(_stamina + staminaValue, MaxStamina);
        OnStaminaChange.OnNext(_stamina - previousStamina);
    }
    public void ConsumeStamina(float staminaValue)
    {
        var previousStamina = _stamina;
        _stamina = Mathf.Max(_stamina - staminaValue, 0);
        OnStaminaChange.OnNext(_stamina - previousStamina);
    }

    void HandleFoodItemUsed(BagItemV001 usedItem)
    {
        if (usedItem.Item.Type.Code.ToLower().Equals("playerfood"))
        {
            Debug.Log($"Food Item Used");

            IncreaseStamina(30);
        }
    }
}

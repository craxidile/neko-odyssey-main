using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Game.Audio;
using Sound = Unity.Game.Audio.Sound;

public class UiButtonSound : MonoBehaviour
{
    [SerializeField] public Sound onHoverSound = Sound.UI_Rollover;
    [SerializeField] public Sound onClickSound = Sound.UI_ClickOk;

    ButtonHover _hoverButton;

    private void Awake()
    {
        _hoverButton = GetComponent<ButtonHover>();
        Initialized();
    }


    void Initialized()
    {
        Initialized_OnHover();
        Initialized_OnClick();
    }
    public void Initialized_OnHover()
    {
        _hoverButton.onHover.AddListener(OnHover);
    }
    public void Initialized_OnClick()
    {
        _hoverButton.onClick.AddListener(OnClick);
    }

    void OnHover(bool isHover)
    {
        if (isHover)
        {
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayAudio(onHoverSound);
            }
        }
    }

    public void OnClick()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayAudio(onClickSound);
        }

    }
}


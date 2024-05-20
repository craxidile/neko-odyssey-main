using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonSound : MonoBehaviour
{
    [SerializeField] public SoundType onHoverSound = SoundType.SFX_Rollover;
    [SerializeField] public SoundType onClickSound = SoundType.SFX_ClickOk;


    ButtonHover _hoverButton;

    private void Awake()
    {
        _hoverButton = GetComponent<ButtonHover>();
        //Initialized();
    }


    //void Initialized()
    //{
    //    Initialized_OnHover();
    //    Initialized_OnClick();
    //}
    //public void Initialized_OnHover()
    //{
    //    if (onHoverSound != AudioType.None)
    //    {
    //        _hoverButton.onHover.AddListener(OnHover);
    //    }
    //}
    //public void Initialized_OnClick()
    //{
    //    if (onClickSound != AudioType.None)
    //    {
    //        _hoverButton.onClick.AddListener(OnClick);
    //    }
    //}

    void OnHover(bool isHover)
    {
        if (isHover)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayAudio(SoundType.SFX_Rollover, true, 1.0f);
            }
        }
    }

    public void OnClick()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayAudio(SoundType.SFX_ClickOk, true, 1.0f); ;
        }

    }
}


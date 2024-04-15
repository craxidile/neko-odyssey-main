using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetBookByNameBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetBookByNameBtnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetBookByNameBtnClick != null)
        {
            OnGetBookByNameBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}


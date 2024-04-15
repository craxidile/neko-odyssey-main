using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetBookByIdBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetBookByIdBtnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetBookByIdBtnClick != null)
        {
            OnGetBookByIdBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}


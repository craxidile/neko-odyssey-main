using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnCreateBookBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnCreateBookBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnCreateBookBtnClick != null)
        {
            OnCreateBookBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}

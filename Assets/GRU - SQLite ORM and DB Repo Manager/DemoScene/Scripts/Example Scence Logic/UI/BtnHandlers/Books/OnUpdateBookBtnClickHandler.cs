using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnUpdateBookBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnUpdateBookBtnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnUpdateBookBtnClick != null)
        {
            OnUpdateBookBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDeleteBookBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnDeleteBookBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnDeleteBookBtnClick != null)
        {
            OnDeleteBookBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDeleteAuthorBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnDeleteAuthorBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnDeleteAuthorBtnClick != null)
        {
            OnDeleteAuthorBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}

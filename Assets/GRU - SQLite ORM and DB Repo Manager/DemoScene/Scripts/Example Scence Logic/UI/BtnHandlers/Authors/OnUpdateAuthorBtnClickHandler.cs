using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnUpdateAuthorBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnUpdateAuthorBtnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnUpdateAuthorBtnClick != null)
        {
            OnUpdateAuthorBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

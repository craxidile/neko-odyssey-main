using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnCreateAuthorBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnCreateAuthorBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnCreateAuthorBtnClick != null)
        {
            OnCreateAuthorBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}

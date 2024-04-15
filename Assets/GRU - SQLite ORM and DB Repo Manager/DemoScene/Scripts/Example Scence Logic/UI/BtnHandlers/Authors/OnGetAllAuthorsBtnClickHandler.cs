using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetAllAuthorsBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetAllAuthorsBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetAllAuthorsBtnClick != null)
        {
            OnGetAllAuthorsBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetAllBooksBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetAllBooksBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetAllBooksBtnClick != null)
        {
            OnGetAllBooksBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

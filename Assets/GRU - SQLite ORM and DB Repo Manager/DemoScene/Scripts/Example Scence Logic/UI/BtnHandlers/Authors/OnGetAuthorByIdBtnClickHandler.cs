using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetAuthorByIdBtnClickHandler :  MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetAuthorByIdBtnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetAuthorByIdBtnClick != null)
        {
            OnGetAuthorByIdBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}


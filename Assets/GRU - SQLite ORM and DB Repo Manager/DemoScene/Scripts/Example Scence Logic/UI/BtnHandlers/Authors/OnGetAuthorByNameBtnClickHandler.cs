using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetAuthorByNameBtnClickHandler :  MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetAuthorByNameBtnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetAuthorByNameBtnClick != null)
        {
            OnGetAuthorByNameBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}


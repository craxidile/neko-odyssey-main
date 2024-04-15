using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetAllAuthorsForDeleteBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetAllAuthorsForDeleteBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetAllAuthorsForDeleteBtnClick != null)
        {
            OnGetAllAuthorsForDeleteBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

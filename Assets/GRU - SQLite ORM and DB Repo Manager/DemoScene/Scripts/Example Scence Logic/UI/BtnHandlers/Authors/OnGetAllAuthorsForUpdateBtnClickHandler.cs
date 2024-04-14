using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetAllAuthorsForUpdateBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetAllAuthorsForUpdateBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetAllAuthorsForUpdateBtnClick != null)
        {
            OnGetAllAuthorsForUpdateBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

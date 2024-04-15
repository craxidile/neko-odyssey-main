using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetAllBooksForUpdateBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetAllBooksForUpdateBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetAllBooksForUpdateBtnClick != null)
        {
            OnGetAllBooksForUpdateBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

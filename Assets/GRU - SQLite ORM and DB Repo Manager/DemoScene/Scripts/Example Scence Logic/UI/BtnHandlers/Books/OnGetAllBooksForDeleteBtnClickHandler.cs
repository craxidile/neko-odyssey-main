using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGetAllBooksForDeleteBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGetAllBooksForDeleteBtnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnGetAllBooksForDeleteBtnClick != null)
        {
            OnGetAllBooksForDeleteBtnClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}

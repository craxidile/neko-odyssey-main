using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnBookInUpdateListBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public IdentifyBookDto BookDto { get; set; }

    public static event Action<IdentifyBookDto> OnBookInUpdateListBtnClick;

    public void InitHandler(IdentifyBookDto authorDto)
    {
        BookDto = authorDto;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnBookInUpdateListBtnClick != null)
        {
            OnBookInUpdateListBtnClick(this.BookDto);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}

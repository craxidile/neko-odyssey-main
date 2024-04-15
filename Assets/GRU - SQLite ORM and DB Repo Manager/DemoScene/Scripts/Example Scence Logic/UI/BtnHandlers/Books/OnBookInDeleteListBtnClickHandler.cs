using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnBookInDeleteListBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public IdentifyBookDto BookDto { get; set; }

    public static event Action<IdentifyBookDto> OnBookInDeleteListBtnClick;

    public void InitHandler(IdentifyBookDto authorDto)
    {
        BookDto = authorDto;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnBookInDeleteListBtnClick != null)
        {
            OnBookInDeleteListBtnClick(this.BookDto);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}

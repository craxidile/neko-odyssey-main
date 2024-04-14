using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnAuthorInDeleteListBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public IdentifyAuthorDto AuthorDto { get; set; }

    public static event Action<IdentifyAuthorDto> OnAuthorInDeleteListBtnClick;

    public void InitHandler(IdentifyAuthorDto authorDto)
    {
        AuthorDto = authorDto;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnAuthorInDeleteListBtnClick != null)
        {
            OnAuthorInDeleteListBtnClick(this.AuthorDto);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}

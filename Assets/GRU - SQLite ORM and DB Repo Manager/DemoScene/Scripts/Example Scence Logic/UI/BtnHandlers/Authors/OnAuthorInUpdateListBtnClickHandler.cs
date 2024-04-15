using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnAuthorInUpdateListBtnClickHandler : MonoBehaviour, IPointerClickHandler
{
    public IdentifyAuthorDto AuthorDto { get; set; }

    public static event Action<IdentifyAuthorDto> OnAuthorInUpdateListBtnClick;

    public void InitHandler(IdentifyAuthorDto authorDto)
    {
        AuthorDto = authorDto;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnAuthorInUpdateListBtnClick != null)
        {
            OnAuthorInUpdateListBtnClick(this.AuthorDto);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

}

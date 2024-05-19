using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConfirmPopUpPanel : UiStackPanelHandler
{
    public CanvasGroup canvasGroup;
    public ButtonHover ConfirmButton, CancelButton;

    bool _recursiveCheck = false;

    private void Awake()
    {
        OnClosePanel += ClickCloseButton;
        CancelButton.onClick.AddListener(CloseStackPanel);
        ConfirmButton.onClick.AddListener(CloseStackPanel);

        OnOpenPanel += OpenPanel;
    }
    void ClickCloseButton() //Esc
    {
        if (!_recursiveCheck)
        {
            CancelButton.onClick?.Invoke();
            _recursiveCheck = true;
        }
    }
    void CloseStackPanel() //click button
    {
        if (!_recursiveCheck)
        {
            InputControls.StackPanelGOList.Remove(gameObject);
            InputControls.StackPanelList.Remove(this);
            _recursiveCheck = true;
        }
    }

    void OpenPanel()
    {
        _recursiveCheck = false;
    }
}

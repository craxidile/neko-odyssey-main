using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static TimeMachineBehaviour;

public class UiStackPanelHandler : MonoBehaviour, IStackPanel
{
    public Action OnOpenPanel { get; set; }
    public Action OnClosePanel { get; set; }

    public bool closeCondition { get; set; } = true;

    public void ShowPanel()
    {
        InputControls.StackPanelList.Add(this);
        InputControls.StackPanelGOList.Add(gameObject);
        OnOpenPanel?.Invoke();
    }
    public void ClosePanel()
    {
        //Remove StackPanelList is already inside InputControls function
        if (closeCondition)
        {
            OnClosePanel?.Invoke();

            if (TryGetComponent(out KeyboardControlNavigation keyboardControl))
            {
                keyboardControl.OnDisable();
            }
        }
    }
}

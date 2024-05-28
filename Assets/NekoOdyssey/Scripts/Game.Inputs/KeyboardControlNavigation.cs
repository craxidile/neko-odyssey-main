using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class KeyboardControlNavigation : MonoBehaviour
{
    public bool focusOnEnable = true;
    public bool resetFocusEveryTime = false;
    Button onEnableFocusObject;

    CanvasGroup canvasGroup;
    bool isActive = false;

    static List<KeyboardControlNavigation> keyboardControlLayer = new List<KeyboardControlNavigation>();
    static UnityAction onKeyboardLayerClosed;

    Button[] allButtons;
    private void Awake()
    {
        GetButtonComponents();

        TryGetComponent(out canvasGroup);

        onKeyboardLayerClosed += OnKeyboardLayerClosed;
    }
    public void OnEnable()
    {
        isActive = true;

        if (keyboardControlLayer.Count == 0 || keyboardControlLayer.Last() != this)
        {
            InputControls.onKeyboardEnable += FocusButton;

            keyboardControlLayer.Add(this);
        }

        if (focusOnEnable)
        {
            FocusButton(true);
        }
    }

    public void GetButtonComponents()
    {
        allButtons = GetComponentsInChildren<Button>(true);
        //Debug.Log(name + " GetButtonComponents : " + allButtons.Length);
    }


    void FocusButton() => FocusButton(false);
    void FocusButton(bool isFlexing = false)
    {
        if (!InputControls.isKeyboardControlling || keyboardControlLayer.Last() != this) return;

        //if (SoundEffectManager.Instance != null) SoundEffectManager.Instance.MakeSoundCheckDelay(0.1f);

        if (isFlexing)
        {
            EventSystem.current?.SetSelectedGameObject(null);
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(waitEndFrme());
        }
    }

    IEnumerator waitEndFrme()
    {
        yield return new WaitForEndOfFrame();

        if (!resetFocusEveryTime && allButtons.Contains(onEnableFocusObject) && CheckFocusObjectCondition(onEnableFocusObject))
        {

        }
        else
        {
            foreach (var button in allButtons)
            {
                //if (button == null) continue;
                if (CheckFocusObjectCondition(button))
                {
                    onEnableFocusObject = button;
                    break;
                }
            }

        }

        //Debug.Log($"KeyboardControlNavigation {gameObject.name} focus on {onEnableFocusObject.name}".SetColor(Color.yellow));
        if (onEnableFocusObject != null) onEnableFocusObject.Select();
    }

    bool CheckFocusObjectCondition(Button button)
    {
        return button != null
            && button.gameObject.activeInHierarchy
            && button.navigation.mode != Navigation.Mode.None
            && button.enabled;
    }


    public void OnDisable()
    {
        isActive = false;
        EventSystem.current?.SetSelectedGameObject(null);

        InputControls.onKeyboardEnable -= FocusButton;


        keyboardControlLayer.Remove(this);
        onKeyboardLayerClosed?.Invoke();

    }

    void OnKeyboardLayerClosed()
    {
        if (keyboardControlLayer.Count <= 0) return;
        if (keyboardControlLayer.Last() == this)
        {
            OnEnable();
        }
    }

    private void Update()
    {
        if (canvasGroup != null)
        {
            if (canvasGroup.interactable && !isActive)
            {
                OnEnable();
            }
            else
            if (!canvasGroup.interactable && isActive)
            {
                OnDisable();
            }
        }
        if (InputControls.isKeyboardControlling && isActive /*&& InputControls.input.Player.AnyKeyKeyboardAndGamepad.triggered*/)
        {
            if (keyboardControlLayer.Last() == this)
            {
                if (EventSystem.current?.currentSelectedGameObject == null)
                {
                    FocusButton();

                }
                else
                {
                    if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out onEnableFocusObject))
                    {
                        //Debug.Log($"checkKeyFocus >>> {gameObject.name} focus on {onEnableFocusObject.name}".SetColor(Color.cyan));

                    }
                }

            }
        }


    }

}

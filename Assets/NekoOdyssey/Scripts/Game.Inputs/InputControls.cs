using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;

public class InputControls : MonoBehaviour
{
    public static PlayerInputActions input { get; set; }

    public static bool isKeyboardControlling { get; set; } = false;

    public static UnityAction onKeyboardEnable;

    public static List<IStackPanel> StackPanelList = new List<IStackPanel>();
    public static List<GameObject> StackPanelGOList = new List<GameObject>();

    public enum InputDevice
    {
        MouseKeyboard,
        DualShock,
        Xbox,
        Switch,
    }

    [HideInInspector] public static InputDevice currentInputDevice { get; set; }
    public static UnityAction OnDeviceChange { get; set; }

    private void Awake()
    {
        if (input != null)
        {
            Destroy(this);
            return;
        }

        var newInput = new PlayerInputActions();
        newInput.Enable();
        input = newInput;

        currentInputDevice = InputDevice.MouseKeyboard;
    }

    private void Update()
    {
        if (!isKeyboardControlling)
        {
            if (isKeyboardChecking())
            {
                isKeyboardControlling = true;
                onKeyboardEnable?.Invoke();
                Debug.Log("Enable Keyboard & Controller : " + currentInputDevice);
            }
        }

        if (isKeyboardControlling)
        {
            if (input.AnyKey.MouseAny.triggered)
            {
                isKeyboardControlling = false;
                Debug.Log("Enable Mouse");
            }
        }

        if (input.UI.Cancel.triggered)
        {
            CloseStackPanel();
        }

        AnyKeyDown();

        DeviceChanged();


        DebugCheck();
    }

    bool isKeyboardChecking()
    {
        return AnyKeyDown(false) &&
               !(input.Player.Movement.triggered || Keyboard.current.escapeKey.wasPressedThisFrame);
        ;
    }

    public static bool AnyKeyDown(bool IncludeMouse = true)
    {
        bool mouseDown = IncludeMouse ? input.AnyKey.MouseAny.triggered : false;

        bool keyboardDown = input.AnyKey.KeyboardAny.triggered;

        bool gamepadDown = input.AnyKey.GamePadAny.triggered;


        if (mouseDown || keyboardDown)
        {
            if (currentInputDevice != InputDevice.MouseKeyboard)
            {
                currentInputDevice = InputDevice.MouseKeyboard;
                OnDeviceChange?.Invoke();
            }
        }


        if (gamepadDown)
        {
            if (input.AnyKey.GamePadAny.triggered)
            {
                if (Gamepad.current is XInputController && currentInputDevice != InputDevice.Xbox)
                {
                    currentInputDevice = InputDevice.Xbox;
                    OnDeviceChange?.Invoke();
                }
                else if (Gamepad.current is DualShockGamepad && currentInputDevice != InputDevice.DualShock)
                {
                    currentInputDevice = InputDevice.DualShock;
                    OnDeviceChange?.Invoke();
                }
                else if (Gamepad.current is SwitchProControllerHID && currentInputDevice != InputDevice.Switch)
                {
                    currentInputDevice = InputDevice.Switch;
                    OnDeviceChange?.Invoke();
                }
            }
        }

        return mouseDown || keyboardDown || gamepadDown;
    }


    public static void CloseStackPanel()
    {
        //Debug.Log($"CloseStackPanel Check)");
        if (StackPanelList.Count > 0)
        {
            var targetPanel = StackPanelList.Last();
            if (targetPanel != null)
            {
                StackPanelList.Remove(targetPanel);

                //targetPanel.ClosePanel();

                //Debug.Log($"CloseStackPanel {targetPanel}");

            }
            else
            {
                StackPanelList.Remove(targetPanel);
            }
        }

        if (StackPanelGOList.Count > 0)
        {
            var targetPanel = StackPanelGOList.Last();
            if (targetPanel != null)
            {
                StackPanelGOList.Remove(targetPanel);

                var stackPanel = targetPanel.GetComponent<IStackPanel>();

                stackPanel.ClosePanel();

                //Debug.Log($"CloseStackPanel {targetPanel}");

            }
            else
            {
                StackPanelGOList.Remove(targetPanel);
            }
        }
    }


    void DeviceChanged()
    {
        switch (currentInputDevice)
        {
            case InputDevice.MouseKeyboard:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
            case InputDevice.DualShock:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case InputDevice.Xbox:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case InputDevice.Switch:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            default:
                break;
        }
    }
    // void OnGUI()
    // {
    //     var targetSelect = EventSystem.current.currentSelectedGameObject;
    //     if (targetSelect != null)
    //     {
    //         GUI.Label(new Rect(10, 10, 100, 20), targetSelect.name);
    //
    //     }
    //
    //
    // }

    void DebugCheck()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            string allUiStackPanel = "";

            allUiStackPanel += "Check UiStackPanelList \n".SetColor(Color.red);
            int maxIndex = Mathf.Max(StackPanelList.Count, StackPanelGOList.Count);
            for (int i = 0; i < maxIndex; i++)
            {
                if (StackPanelList.Count > i)
                {
                    var stackPanelMono = StackPanelList[i] as MonoBehaviour;
                    if (stackPanelMono != null)
                    {
                        allUiStackPanel += stackPanelMono.name;

                    }
                }

                allUiStackPanel += "   /   ".SetColor(Color.red);

                if (StackPanelGOList.Count > i)
                {
                    var stackPanelGO = StackPanelGOList[i];
                    if (stackPanelGO != null)
                    {
                        allUiStackPanel += stackPanelGO.name;

                    }
                }

                allUiStackPanel += "\n";
            }
            Debug.Log($"{allUiStackPanel}");
        }
    }
}
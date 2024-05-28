using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_keyboardAddition : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Button leftButton, rightButton;

    float _inputValue;
    float _inputDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        //{
        //    _inputValue -= Time.deltaTime;
        //    if (_inputValue > 0)
        //    {
        //        return;
        //    }

        //    Vector2 input = InputControls.input.UI.Navigate.ReadValue<Vector2>();

        //    if (input.x != 0)
        //    {
        //        _inputValue = _inputDelay;

        //        if (slider != null)
        //        {
        //            slider.value += input.x * (slider.maxValue / 10);
        //        }
        //        if (leftButton != null && rightButton != null)
        //        {
        //            if (input.x > 0) // +1
        //            {
        //                rightButton.onClick?.Invoke();
        //            }
        //            if (input.x < 0) // -1
        //            {
        //                leftButton.onClick?.Invoke();
        //            }
        //        }
        //    }
        //}
    }
}

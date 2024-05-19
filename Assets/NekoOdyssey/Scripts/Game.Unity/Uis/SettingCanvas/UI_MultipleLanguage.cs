using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting;

public class UI_MultipleLanguage : MonoBehaviour
{
    TextMeshProUGUI text;

    string initialText = "";

    //bool isOverided;

    [HideInInspector] public Action<TextMeshProUGUI> onUpdateLanguage;

    public void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        if (initialText == "")
        {
            initialText = text.text;
            ChangeText();
        }
    }

    public void OverideInitialText(string newText, Action<TextMeshProUGUI> onUpdateLanguage = null)
    {
        //isOverided = true;
        initialText = newText;

        if (onUpdateLanguage != null)
        {
            this.onUpdateLanguage += onUpdateLanguage;
        }
    }

    void ChangeText()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }


        text.text = initialText;
        LoadUiLanguageFromCSV.GetUiLanguage(ref text);

        onUpdateLanguage?.Invoke(text);
        
    }


    public static void UpdateLanguage()
    {
        var textList = FindObjectsOfType<UI_MultipleLanguage>(true);

        foreach (var texts in textList)
        {
            //if (texts.isOverided) continue;

            texts.ChangeText();
        }
    }
}
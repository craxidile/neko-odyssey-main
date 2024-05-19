using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Dropdown languageDropdown;
    [SerializeField] GameObject _loadingCanvas;

    private void Awake()
    {
        if (languageDropdown != null)
        {
            List<Dropdown.OptionData> optionLanguage = new List<Dropdown.OptionData>();
            string[] PieceTypeNames = System.Enum.GetNames(typeof(languageType));
            for (int i = 0; i < PieceTypeNames.Length; i++)
            {
                optionLanguage.Add(new Dropdown.OptionData(PieceTypeNames[i]));
            }
            languageDropdown.AddOptions(optionLanguage);
            //LanguageManager.globalLanguage = LanguageManager.languageType.English;

        }

    }
    private void Start()
    {
        if (languageDropdown != null)
            languageDropdown.captionText.text = LanguageManager.globalLanguage.ToString();
    }
    public void ChangeLanguage()
    {
        //var language = (languageType)languageDropdown.value;
        ChangeLanguage(languageDropdown.value);


    }
    public void ChangeLanguage(int languageIndex)
    {
        var language = (languageType)languageIndex;
        Debug.Log($"ChangeLanguage : {language}");
        LanguageManager.globalLanguage = language;

        GetComponent<LoadUiLanguageFromCSV>().ReloadUiLanguage();
    }

    public void ShowLoadingCanvas()
    {
        _loadingCanvas.SetActive(true);
    }

    public void ExitGame() => Application.Quit();

    //private void Update()
    //{
    //    var input = InputControls.input.Player.ZoomOutCamera.ReadValue<Vector2>();

    //    Debug.Log("CameraZoom_OnMapsMenu : "+input);
    //}
}

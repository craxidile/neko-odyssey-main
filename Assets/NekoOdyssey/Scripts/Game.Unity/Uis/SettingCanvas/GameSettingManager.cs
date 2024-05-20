using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using DataPersistence;
using UnityEngine.SceneManagement;

public class GameSettingManager : MonoBehaviour, IStackPanel
{
    [SerializeField] RectTransform _settingPanel;
    [SerializeField] Button _backButton;
    [SerializeField] Button _resetSettingButton, _saveSettingButton;
    [SerializeField] Button _languageLeftButton, _languageRightButton;
    [SerializeField] Image _languageNameImage;
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] Slider _masterVolumeSlider, _bgmVolumeSlider, _effectVolumeSlider;
    [SerializeField] Button _resolutionLeftButton, _resolutionRightButton;
    [SerializeField] TextMeshProUGUI _resolutionText;
    [SerializeField] Button _fullscreenLeftButton, _fullscreenRightButton;
    [SerializeField] TextMeshProUGUI _fullscreenText;

    [SerializeField] ConfirmPopUpPanel changingLanguageConfirmPopUpPanel;


    CanvasGroup canvasGroup;

    public bool IsActivation { get; set; }

    int LanguageIndex;
    int _starterLanguageIndex;
    int WindowMode;

    List<Resolution> activeResolutions;
    int currentResolutionIndex;

    bool _oldFullscreen;
    int _oldResolutionIndex;

    static GameSettingData settingData;

    public Action OnClosePanel { get; set; }
    public Action OnOpenPanel { get; set; }

    public static GameSettingManager Instance;
    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        //_backButton.onClick.AddListener(() => _saveSettingButton.onClick?.Invoke());
        _backButton.onClick.AddListener(() => ResetAndClose()); //
        //_backButton.onClick.AddListener(() => SaveSetting(true)); //
        //_backButton.onClick.AddListener(() => ShowPanel(false)); //


        _resetSettingButton.onClick.AddListener(() => ResetSetting());

        //_saveSettingButton.onClick.AddListener(() => SaveSetting());
        //_saveSettingButton.onClick.AddListener(() => ShowPanel(false));
        //_saveSettingButton.onClick.AddListener(() => _backButton.onClick?.Invoke());
        _saveSettingButton.onClick.AddListener(() => SaveSetting(true));


        _languageLeftButton.onClick.AddListener(() => ChangeLanguage(-1));
        _languageRightButton.onClick.AddListener(() => ChangeLanguage(1));

        _masterVolumeSlider.onValueChanged.AddListener(value => ChangeMasterVolume(value));
        _masterVolumeSlider.value = 0.3f;
        _bgmVolumeSlider.onValueChanged.AddListener(value => ChangeBGMVolume(value));
        _effectVolumeSlider.onValueChanged.AddListener(value => ChangeEffectVolume(value));

        _resolutionLeftButton.onClick.AddListener(() => ChangeScreenResolution(-1));
        _resolutionRightButton.onClick.AddListener(() => ChangeScreenResolution(1));

        _fullscreenLeftButton.onClick.AddListener(() => ChangeWindowMode(-1));
        _fullscreenRightButton.onClick.AddListener(() => ChangeWindowMode(1));

        LoadSetting();
        SetupLanguage();
        SetupVolume();
        SetupScreen();

        SetUpChangingLanguagePopUpPanel();
        changingLanguageConfirmPopUpPanel.canvasGroup.LerpAlpha(0, 0);

        //ClosePanel();
        //SaveSetting(false);
        ShowPanel(false);

#if UNITY_SWITCH

        SwitchSetting();
#endif

    }

    void LoadSetting()
    {
        if (settingData == null) settingData = DataPersistenceManager.instance.GetSettingData();
    }
    void SetupLanguage()
    {
        _starterLanguageIndex = settingData.LanguageIndex;

        if (!_languageRightButton.transform.parent.gameObject.activeSelf)
        {
            changingLanguageConfirmPopUpPanel.gameObject.SetActive(false);
            LanguageIndex = settingData.LanguageIndex;
            return;
        }

        LanguageIndex = settingData.LanguageIndex - 1;

        ChangeLanguage(1);
    }
    void SetupVolume()
    {
        _masterVolumeSlider.value = settingData.MasterVolume;
        _bgmVolumeSlider.value = settingData.BgmVolume;
        _effectVolumeSlider.value = settingData.EffectVolume;
    }
    void SetupScreen()
    {
        var resolutions = Screen.resolutions;
        var currentRefrestRate = Screen.currentResolution.refreshRateRatio;

        activeResolutions = resolutions
            .Where(res => res.refreshRateRatio.Equals(currentRefrestRate))
            .Where(res => res.width / res.height >= 16 / 9)
            .ToList();

        for (int i = 0; i < activeResolutions.Count; i++)
        {
            var res = activeResolutions[i];
            if (res.width == Screen.width && res.height == Screen.height)
            {
                currentResolutionIndex = i;
                _oldResolutionIndex = i;
            }
        }

        WindowMode = Screen.fullScreen ? 0 : 1;
        _oldFullscreen = Screen.fullScreen;

        _resolutionText.text = $"{Screen.width}x{Screen.height}";

        var fullscreenModeText = Screen.fullScreen ? "Fullscreen" : "Windowed";
        //_fullscreenText.text = Screen.fullScreen ? "Fullscreen" : "Windowed";
        _fullscreenText.text = LoadUiLanguageFromCSV.GetUiLanguageText(fullscreenModeText);

        var fullscreenTextMultiLanguageUi = _fullscreenText.GetComponent<UI_MultipleLanguage>();
        fullscreenTextMultiLanguageUi.OverideInitialText(fullscreenModeText);
    }

    public void SaveSetting(bool askingConfirm)
    {
        //show changing language comfirmation panel
        if (settingData.LanguageIndex != LanguageIndex && askingConfirm)
        {
            ShowChangingLanguageComfirmation(true);
            return;
        }

        //language
        settingData.LanguageIndex = LanguageIndex;
        //UpdateMultilanguageUi();

        //sound
        settingData.MasterVolume = _masterVolumeSlider.value;
        settingData.BgmVolume = _bgmVolumeSlider.value;
        settingData.EffectVolume = _effectVolumeSlider.value;



        DataPersistenceManager.instance.SaveSetting();

        //override temp
        _oldResolutionIndex = currentResolutionIndex;
        _oldFullscreen = Screen.fullScreen;


        //close panel
        ShowPanel(false);


    }
    public void ResetSetting()
    {
        SetupLanguage();
        SetupVolume();

        currentResolutionIndex = _oldResolutionIndex;
        ChangeScreenResolution(0);

        WindowMode = _oldFullscreen ? 0 : 1;
        ChangeWindowMode(0);

    }


    public void ChangeLanguage(int value)
    {
        int maxLanguageIndex = Enum.GetValues(typeof(languageType)).Length - 1;
        int languageIndex = Mathf.Clamp(LanguageIndex + value, 0, maxLanguageIndex);
        if (languageIndex != LanguageIndex)
        {
            LanguageIndex = languageIndex;
            var language = (languageType)languageIndex;
            Debug.Log($"ChangeLanguage : {language}");
            LanguageManager.globalLanguage = language;

            FindFirstObjectByType<LoadUiLanguageFromCSV>().ReloadUiLanguage(); 
            _languageNameImage.sprite = LanguagePresetProvider.Instance.GetLanguageComponent().languageNameImage;
            _languageNameImage.SetNativeSize();

        }


        _languageLeftButton.gameObject.SetActive(!(languageIndex == 0));
        _languageRightButton.gameObject.SetActive(!(languageIndex == maxLanguageIndex));

        UpdateMultilanguageUi();

    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
        //_audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
    }
    public void ChangeBGMVolume(float value)
    {
        float targetValue = Mathf.Log10(value) * 20;
        if (value == 0)
        {
            targetValue = -80;
        }
        _audioMixer.SetFloat("BGM", targetValue);
    }
    public void ChangeEffectVolume(float value)
    {
        var targetValue = Mathf.Log10(value) * 20;
        if (value == 0)
        {
            targetValue = -80;
        }
        _audioMixer.SetFloat("Effect", targetValue);
    }

    public void ChangeScreenResolution(int value)
    {
        currentResolutionIndex = Mathf.Clamp(currentResolutionIndex + value, 0, activeResolutions.Count - 1);

        var res = activeResolutions[currentResolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        _resolutionText.text = $"{res.width}x{res.height}";
    }
    public void ChangeWindowMode(int value)
    {
        WindowMode = (WindowMode + value) % 2;
        Screen.fullScreen = WindowMode == 0;

        string textOption = WindowMode == 0 ? "Fullscreen" : "Windowed";
        //_fullscreenText.text = WindowMode == 0 ? "Fullscreen" : "Windowed";
        _fullscreenText.text = LoadUiLanguageFromCSV.GetUiLanguageText(textOption);

        var fullscreenTextMultiLanguageUi = _fullscreenText.GetComponent<UI_MultipleLanguage>();
        fullscreenTextMultiLanguageUi.OverideInitialText(textOption);

    }

    void UpdateMultilanguageUi()
    {
        UI_MultipleLanguage.UpdateLanguage();
    }


    void SetUpChangingLanguagePopUpPanel()
    {
        changingLanguageConfirmPopUpPanel.ConfirmButton.onClick.AddListener(() =>
        {
            SaveSetting(false);
            //SceneManager.LoadScene("Demo Cutscene Menu");
            ShowChangingLanguageComfirmation(false);
        });

        changingLanguageConfirmPopUpPanel.CancelButton.onClick.AddListener(() =>
        {
            ShowChangingLanguageComfirmation(false);
            ShowPanel(true);
            ResetSetting();
        });
    }

    void ShowChangingLanguageComfirmation(bool condition)
    {
        if (condition)
        {
            changingLanguageConfirmPopUpPanel.ShowPanel();
        }
        changingLanguageConfirmPopUpPanel.canvasGroup.LerpAlpha(condition ? 1f : 0f, 0.2f);

    }


    public void ShowPanel() => ShowPanel(!IsActivation);
    public void ShowPanel(bool condition)
    {
        IsActivation = condition;

        if (condition)
        {
            InputControls.StackPanelList.Add(this);
            InputControls.StackPanelGOList.Add(gameObject);
            OnOpenPanel?.Invoke();
            //canvasGroup.SetAlpha(1f);
        }
        else
        {
            OnClosePanel?.Invoke();
            //canvasGroup.SetAlpha(0f);
        }
        canvasGroup.LerpAlpha(condition ? 1f : 0f, 0.2f);
    }

    public void ClosePanel()
    {
        _backButton.onClick?.Invoke();
    }

    void ResetAndClose()
    {
        //ShowPanel(true);
        ResetSetting();
        SaveSetting(false);
    }


    void SwitchSetting()
    {
        float sizeReduced = 0;
        var resolutionGrp = _resolutionText.transform.parent;
        var windowModeGrp = _fullscreenText.transform.parent;

        resolutionGrp.gameObject.SetActive(false);
        sizeReduced += resolutionGrp.GetComponent<RectTransform>().sizeDelta.y;
        windowModeGrp.gameObject.SetActive(false);
        sizeReduced += windowModeGrp.GetComponent<RectTransform>().sizeDelta.y;

        Debug.Log($"sizeReduced : {sizeReduced}");

        var newSize = _settingPanel.sizeDelta;
        newSize.y -= sizeReduced;
        _settingPanel.sizeDelta = newSize;
    }
}

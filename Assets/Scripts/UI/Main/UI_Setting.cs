using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    private enum GameObjects
    {
        GameSetting,
        GraphicSetting,
        SoundSetting,
        ResolutionDropdown,
        WindowedToggle,
        BgmSlider,
        EffectSlider
    }

    private enum Texts
    {
        BgmValueText,
        EffectValueText
    }

    private enum Buttons
    {
        GameButton,
        GraphicButton,
        SoundButton,
        ExitButton,
        SaveButton,
        LoadButton,
        LobbyButton,
    }

    private UserDataManager userDataManager => Managers.Data.UserDataManager;
    private List<Resolution> _resolutions;
    private Toggle _screenToggle;
    private Slider _bgmSlider;
    private TextMeshProUGUI _bgmValueText;
    private Slider _effectSlider;
    private TextMeshProUGUI _effectValueText;
    private TMP_Dropdown _resolutionDropdown;
    private GameObject _gameSetting;
    private GameObject _graphicSetting;
    private GameObject _soundSetting;
    private int _currentSettingSlot = 0;

    private void OnSettingButtonClicked(PointerEventData data)
    {
        string buttonName = data.pointerClick.name;

        if(!Enum.TryParse(buttonName, out Buttons btn))
        {
            return;
        }

        if(_currentSettingSlot == (int)btn)
        {
            return;
        }

        _currentSettingSlot = (int)btn;
        InitializeMainMenu();
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void OnSaveButtonClicked(PointerEventData data)
    {
        int saveNum = userDataManager.CurrentSaveData.saveNumber;
        
        userDataManager.SaveDataToJson(saveNum);

        UI_MessageBox messageBox = Managers.UI.ShowUI<UI_MessageBox>();
        string msg = "저장이 완료되었습니다.";
        messageBox.SetMessageBox(msg, null, true);
    }

    private void OnLoadButtonClicked(PointerEventData data)
    {
        UI_SaveLoad saveLoadUI = Managers.UI.ShowUI<UI_SaveLoad>();
        saveLoadUI.SetCurrentMode(1);
    }

    private void OnCompleted()
    {
        Managers.Scene.LoadNextScene(Define.SceneType.Lobby);
    }

    private void OnLobbyButtonClicked(PointerEventData data)
    {
        UI_MessageBox messageBox = Managers.UI.ShowUI<UI_MessageBox>();
        string msg = "저장되지 않은 데이터는 사라집니다. 로비로 나가시겠습니까?";
        messageBox.SetMessageBox(msg, OnCompleted);
    }

    private void OnResolutionChanged(int index)
    {
        Resolution res = _resolutions[index];
        bool isFullScreen = Screen.fullScreen;

        Screen.SetResolution(res.width, res.height, isFullScreen);
    }

    private void InitDropdown()
    {
        _resolutions = new List<Resolution>(Screen.resolutions);
        _resolutionDropdown.ClearOptions();

        int currentIndex = 0;
        List<string> options = new List<string>();

        for(int i = 0; i < _resolutions.Count; i++)
        {
            Resolution res = _resolutions[i];
            string option = $"{res.width} x {res.height} @ {res.refreshRate}Hz";
            
            options.Add(option);

            if(res.width == Screen.width &&
                res.height == Screen.height
                && res.refreshRate == Screen.currentResolution.refreshRate)
            {
                currentIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentIndex;
        _resolutionDropdown.RefreshShownValue();

        _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    private void OnToggleChanged(bool isFullScreen)
    {
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = isFullScreen;
    }

    private void InitScreenToggle()
    {
        if(_screenToggle == null)
        {
            return;
        }
        
        _screenToggle.isOn = Screen.fullScreen;
        _screenToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnBgmSliderChanged(float sliderValue)
    {
        Managers.Sound.SetBgmVolume(sliderValue);
        _bgmValueText.text = $"{sliderValue}";
    }

    private void OnEffectSliderChanged(float sliderValue)
    {
        Managers.Sound.EffectVolume = sliderValue;
        _effectValueText.text = $"{sliderValue}";
    }

    private void InitSlider()
    {
        _bgmSlider.wholeNumbers = _effectSlider.wholeNumbers = true;
        _bgmSlider.minValue = _effectSlider.minValue = 0;
        _bgmSlider.maxValue = _effectSlider.maxValue = 100;

        _bgmSlider.value = Managers.Sound.BgmVolume * 100.0f;
        _effectSlider.value = Managers.Sound.EffectVolume * 100.0f;

        _bgmValueText.text = $"{_bgmSlider.value}";
        _effectValueText.text = $"{_effectSlider.value}";

        _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        _effectSlider.onValueChanged.AddListener(OnEffectSliderChanged);
    }

    private void InitializeMainMenu()
    {
        switch(_currentSettingSlot)
        {
            case 0:
                _gameSetting.SetActive(true);
                _graphicSetting.SetActive(false);
                _soundSetting.SetActive(false);
                break;
            case 1:
                _gameSetting.SetActive(false);
                _graphicSetting.SetActive(true);
                _soundSetting.SetActive(false);
                break;
            case 2:
                _gameSetting.SetActive(false);
                _graphicSetting.SetActive(false);
                _soundSetting.SetActive(true);
                break;
            default:
                return;
        }
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
    }

    private void GetUIElements()
    {
        _resolutionDropdown = GetObject((int)GameObjects.ResolutionDropdown).GetComponent<TMP_Dropdown>();
        _gameSetting = GetObject((int)GameObjects.GameSetting);
        _graphicSetting = GetObject((int)GameObjects.GraphicSetting);
        _soundSetting = GetObject((int)GameObjects.SoundSetting);
        _screenToggle = GetObject((int)GameObjects.WindowedToggle).GetComponent<Toggle>();
        _bgmSlider = GetObject((int)GameObjects.BgmSlider).GetComponent<Slider>();
        _bgmValueText = GetText((int)Texts.BgmValueText);
        _effectSlider = GetObject((int)GameObjects.EffectSlider).GetComponent<Slider>();
        _effectValueText = GetText((int)Texts.EffectValueText);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.GameButton).gameObject.BindEvent(OnSettingButtonClicked);
        GetButton((int)Buttons.GraphicButton).gameObject.BindEvent(OnSettingButtonClicked);
        GetButton((int)Buttons.SoundButton).gameObject.BindEvent(OnSettingButtonClicked);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
        GetButton((int)Buttons.SaveButton).gameObject.BindEvent(OnSaveButtonClicked);
        GetButton((int)Buttons.LoadButton).gameObject.BindEvent(OnLoadButtonClicked);
        GetButton((int)Buttons.LobbyButton).gameObject.BindEvent(OnLobbyButtonClicked);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        InitDropdown();
        InitScreenToggle();
        InitSlider();
        InitializeMainMenu(); // 반드시 제일 마지막
    }
}

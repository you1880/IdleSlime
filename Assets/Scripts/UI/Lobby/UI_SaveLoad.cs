using System;
using System.Collections;
using System.Collections.Generic;
using Data.Save;
using Data.Slime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveLoadSlot
{
    private const string BACKGROUND_IMAGE_NAME = "SaveSlotBackground";
    private const string LOCK_IMAGE_NAME = "SaveSlotLock";
    private const string LAST_PLAY_TEXT_NAME = "LastPlayTimeText";
    private const string TRASH_BIN_BUTTON_NAME = "TrashBinButton";
    private const string EMPTY_SLOT_COLOR_HEX = "#8C8C8C";
    private const string OCCUPIED_SLOT_COLOR_HEX = "#BDDFFF";

    public Image backgroundImage;
    public Image lockImage;
    public TextMeshProUGUI lastSaveTimeText;
    public Button trashBinButton;
    public bool isSaveSlotUsed;

    public SaveLoadSlot(Image bgImg, Image img, TextMeshProUGUI txt, Button btn, bool used = false)
    {
        backgroundImage = bgImg;
        lockImage = img;
        lastSaveTimeText = txt;
        trashBinButton = btn;
        isSaveSlotUsed = used;
    }

    public static SaveLoadSlot CreateSlot(GameObject slotObject)
    {
        Image backgroundImage = Util.FindChild<Image>(slotObject, BACKGROUND_IMAGE_NAME, true);
        Image lockImage = Util.FindChild<Image>(slotObject, LOCK_IMAGE_NAME, true);
        TextMeshProUGUI playText = Util.FindChild<TextMeshProUGUI>(slotObject, LAST_PLAY_TEXT_NAME, true);
        Button trashBinButton = Util.FindChild<Button>(slotObject, TRASH_BIN_BUTTON_NAME, true);

        return new SaveLoadSlot(backgroundImage, lockImage, playText, trashBinButton);
    }

    public void SetEmptySlot()
    {
        backgroundImage.color = Util.GetColorFromHex(EMPTY_SLOT_COLOR_HEX);
        lockImage.gameObject.SetActive(true);
        lastSaveTimeText.text = "";
        trashBinButton.gameObject.SetActive(false);
        isSaveSlotUsed = false;
    }

    public void SetOccupiedSlot(string time)
    {
        backgroundImage.color = Util.GetColorFromHex(OCCUPIED_SLOT_COLOR_HEX);
        lockImage.gameObject.SetActive(false);
        lastSaveTimeText.text = $"마지막 세이브 시간 :\n{time}";
        trashBinButton.gameObject.SetActive(true);
        isSaveSlotUsed = true;
    }
}

public class UI_SaveLoad : UI_Base
{
    private enum GameObjects
    {
        SaveSlot0,
        SaveSlot1,
        SaveSlot2,
        SaveSlot3,
        SaveSlot4
    }

    private enum Texts {}

    private enum Images {}

    private enum Buttons
    {
        ExitButton
    }
    

    private const string SAVE_SLOT_PREFIX = "SaveSlot";
    private enum SaveLoadMode { Save, Load }
    private List<SaveLoadSlot> _saveSlots = new List<SaveLoadSlot>();
    private SaveLoadMode _currentMode;
    private int _clickedSlotNumber = 0;

    public void SetCurrentMode(int mode)
    {
        if(Enum.IsDefined(typeof(SaveLoadMode), mode))
        {
            _currentMode = (SaveLoadMode)mode;
        }
        else
        {
            _currentMode = SaveLoadMode.Save;
        }
    }

    private int GetSlotNumber(string name)
    {
        int number = 0;

        if(name.StartsWith(SAVE_SLOT_PREFIX))
        {
            string numberStr = name.Substring(SAVE_SLOT_PREFIX.Length);

            if(int.TryParse(numberStr, out number))
            {
                return number;
            }
        }

        return number;
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.Resource.Destroy(this.gameObject);
    }

    private void OnSlotClicked(PointerEventData data)
    {
        Managers.Sound.PlayButtonSound(); // Slot은 GameObject에 OnSlotClicked를 BindEvent해서 PlayButtonSound가 있어야됨

        _clickedSlotNumber = GetSlotNumber(data.pointerClick.name);
        if(_currentMode == SaveLoadMode.Save)
        {
            string msg;

            if(_saveSlots[_clickedSlotNumber].isSaveSlotUsed)
            {
                msg = "기존 세이브를 삭제하고 재생성하시겠습니까?";
            }
            else
            {
                msg = "세이브 파일을 생성하시겠습니까?";
            }

            UI_MessageBox box = Managers.UI.ShowUI<UI_MessageBox>("UI_MessageBox");
            box.SetMessageBox(msg, CreateSaveFile);
        }
        else if(_currentMode == SaveLoadMode.Load)
        {
            if(!_saveSlots[_clickedSlotNumber].isSaveSlotUsed)
            {
                return;
            }

            StartGame();
        }
    }

    private void CreateSaveFile()
    {
        string timeString = Util.GetCurrentDataTime();
        List<OwnedSlime> ownedSlimes = new List<OwnedSlime>
        {
            new OwnedSlime(1, 1)
        };

        SaveData saveData = new SaveData(_clickedSlotNumber, timeString, 500, ownedSlimes);
        Managers.Data.SaveDataToJson(_clickedSlotNumber, saveData); 

        _saveSlots[_clickedSlotNumber].SetOccupiedSlot(timeString);

        StartGame();
    }

    private void StartGame()
    {
        Managers.Data.SetCurrentSaveData(_clickedSlotNumber);
        Managers.Scene.LoadNextScene();
    }

    private void OnTrashBinButtonClicked(PointerEventData data)
    {
        Managers.Sound.PlayButtonSound(); // GetButton이 아닌 Util.FindChild로 찾아 BindEvent 하는거라 PlayButtonSound 해줘야함

        _clickedSlotNumber = GetSlotNumber(data.pointerClick.transform.parent.parent.name);
        string msg = "해당 세이브를 삭제하시겠습니까?";
        
        UI_MessageBox box = Managers.UI.ShowUI<UI_MessageBox>("UI_MessageBox");
        box.SetMessageBox(msg, DeleteSaveFile);
    }

    private void DeleteSaveFile()
    {
        Managers.Data.DeleteSaveData(_clickedSlotNumber);

        _saveSlots[_clickedSlotNumber].SetEmptySlot();
    }

    private void BindUIElements()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
    }

    private void GetSlotElements()
    {
        foreach(GameObjects slot in Enum.GetValues(typeof(GameObjects)))
        {
            GameObject slotObject = GetObject((int)slot);
            SaveLoadSlot saveLoadSlot = SaveLoadSlot.CreateSlot(slotObject);
            int slotNumber = GetSlotNumber(slotObject.name);

            InitSlotElements(slotNumber, saveLoadSlot);
            slotObject.BindEvent(OnSlotClicked);
            _saveSlots.Add(saveLoadSlot);
        }
    }

    private void InitSlotElements(int slotNumber, SaveLoadSlot slot)
    {
        SaveData data = Managers.Data.GetSaveDataWithIndex(slotNumber);

        if(data != null)
        {
            slot.SetOccupiedSlot(data.saveTime);
            slot.trashBinButton.gameObject.BindEvent(OnTrashBinButtonClicked);
        }
        else
        {
            slot.SetEmptySlot();
        }
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }
    
    public override void Init()
    {
        BindUIElements();
        GetSlotElements();
        BindButtonEvent();
    }
}

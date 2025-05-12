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
    public Image backgroundImage;
    public Image lockImage;
    public TextMeshProUGUI lastPlayTimeText;
    public Button trashBinButton;
    public bool isSaveSlotUsed;

    public SaveLoadSlot(Image bgImg, Image img, TextMeshProUGUI txt, Button btn, bool used = false)
    {
        backgroundImage = bgImg;
        lockImage = img;
        lastPlayTimeText = txt;
        trashBinButton = btn;
        isSaveSlotUsed = used;
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
    
    private const string BACKGROUND_IMAGE_NAME = "SaveSlotBackground";
    private const string LOCK_IMAGE_NAME = "SaveSlotLock";
    private const string LAST_PLAY_TEXT_NAME = "LastPlayTimeText";
    private const string TRASH_BIN_BUTTON_NAME = "TrashBinButton";
    private const string SAVE_SLOT_PREFIX = "SaveSlot";
    private const string EMPTY_SLOT_COLOR_HEX = "#8C8C8C";
    private const string OCCUPIED_SLOT_COLOR_HEX = "#BDDFFF";
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
            //TODO
            //CurrentSaveData를 클릭한 number로 변경 후 Scene 변경
            if(!_saveSlots[_clickedSlotNumber].isSaveSlotUsed)
            {
                return;
            }

            Managers.Data.SetCurrentSaveData(_clickedSlotNumber);
            Managers.Scene.LoadNextScene();
            StartCoroutine(Managers.Scene.Fade(0.0f, 1.0f));
        }
    }

    private void CreateSaveFile()
    {
        string timeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        List<OwnedSlime> ownedSlimes = new List<OwnedSlime>
        {
            new OwnedSlime(1, 1)
        };

        SaveData saveData = new SaveData(_clickedSlotNumber, timeString, 500, ownedSlimes);
        Managers.Data.SaveDataToJson(_clickedSlotNumber, saveData); 

        SetOccupiedSlot(timeString, _saveSlots[_clickedSlotNumber]);
    }

    private void OnTrashBinButtonClicked(PointerEventData data)
    {
        _clickedSlotNumber = GetSlotNumber(data.pointerClick.transform.parent.parent.name);
        string msg = "해당 세이브를 삭제하시겠습니까?";
        
        UI_MessageBox box = Managers.UI.ShowUI<UI_MessageBox>("UI_MessageBox");
        box.SetMessageBox(msg, DeleteSaveFile);

    }

    private void DeleteSaveFile()
    {
        Managers.Data.DeleteSaveData(_clickedSlotNumber);
        SetEmptySlot(_saveSlots[_clickedSlotNumber]);
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
            Image backgroundImage = Util.FindChild<Image>(slotObject, BACKGROUND_IMAGE_NAME, true);
            Image lockImage = Util.FindChild<Image>(slotObject, LOCK_IMAGE_NAME, true);
            TextMeshProUGUI playText = Util.FindChild<TextMeshProUGUI>(slotObject, LAST_PLAY_TEXT_NAME, true);
            Button trashBinButton = Util.FindChild<Button>(slotObject, TRASH_BIN_BUTTON_NAME, true);
            SaveLoadSlot saveLoadSlot = new SaveLoadSlot(backgroundImage, lockImage, playText, trashBinButton);
            int slotNumber = GetSlotNumber(slotObject.name);

            InitSlotElements(slotNumber, saveLoadSlot);
            slotObject.BindEvent(OnSlotClicked);
            _saveSlots.Add(saveLoadSlot);
        }
    }

    private void SetEmptySlot(SaveLoadSlot slot)
    {
        slot.backgroundImage.color = Util.GetColorFromHex(EMPTY_SLOT_COLOR_HEX);
        slot.lockImage.gameObject.SetActive(true);
        slot.lastPlayTimeText.text = "";
        slot.trashBinButton.gameObject.SetActive(false);
        slot.isSaveSlotUsed = false;
    }

    private void SetOccupiedSlot(string time, SaveLoadSlot slot)
    {
        slot.backgroundImage.color = Util.GetColorFromHex(OCCUPIED_SLOT_COLOR_HEX);
        slot.lockImage.gameObject.SetActive(false);
        slot.lastPlayTimeText.text = time;
        slot.trashBinButton.gameObject.SetActive(true);
        slot.isSaveSlotUsed = true;
        slot.trashBinButton.gameObject.BindEvent(OnTrashBinButtonClicked);
    }

    private void InitSlotElements(int slotNumber, SaveLoadSlot slot)
    {
        SaveData data = Managers.Data.GetSaveDataWithIndex(slotNumber);

        if(data != null)
        {
            SetOccupiedSlot(data.saveTime, slot);
        }
        else
        {
            SetEmptySlot(slot);
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

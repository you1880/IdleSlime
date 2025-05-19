using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Data.Save;
using System;
using Data.Game;

public class DataManager
{
    private UserDataManager _userData = new UserDataManager();
    private GameDataManager _gameData = new GameDataManager();

    public UserDataManager UserDataManager { get { return this._userData; } }
    public GameDataManager GameDataManager { get { return this._gameData; } }

    public void Init()
    {
        _userData.Init();
        _gameData.Init();
    }
}

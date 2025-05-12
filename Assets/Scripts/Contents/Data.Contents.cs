using System.Collections;
using System.Collections.Generic;
using Data.Slime;
using UnityEngine;

namespace Data.Save
{
    [System.Serializable]
    public class SaveData
    {
        public int saveNumber;
        public string saveTime;
        public int money;
        public List<OwnedSlime> ownedSlimes;

        public SaveData(int num, string time, int m, List<OwnedSlime> slimes)
        {
            saveNumber = num;
            saveTime = time;
            money = m;
            ownedSlimes = slimes;
        }
    }
}

namespace Data.Slime
{
    [System.Serializable]
    public class OwnedSlime
    {
        public int slimeType;
        public int slimeCount;

        public OwnedSlime(int sType, int count)
        {
            slimeType = sType;
            slimeCount = count;
        }
    }

    [System.Serializable]
    public class Slime
    {
        int slimeType;
    }
}
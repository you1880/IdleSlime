using System.Collections;
using System.Collections.Generic;
using Data.Save;
using UnityEngine;

public class MainScene : BaseScene
{
    private List<OwnedSlime> _ownedSlimes => Managers.Data.UserDataManager.CurrentSaveData.ownedSlimes;
    private HashSet<int> _spawnedSlimeTypes = new HashSet<int>();

    private void SetRandomSpawnPosition(GameObject slimeObject)
    {
        IReadOnlyList<float> canMoveRanges = Managers.Game.GetMoveRanges();

        float randomPosX = Random.Range(canMoveRanges[0], canMoveRanges[1]);
        float randomPosY = Random.Range(canMoveRanges[2], canMoveRanges[3]);

        slimeObject.transform.position = new Vector3(randomPosX, randomPosY, 0.0f);
    }

    private void SpawnSlimes()
    {
        foreach (OwnedSlime ownedSlime in _ownedSlimes)
        {
            if (_spawnedSlimeTypes.Contains(ownedSlime.slimeType))
            {
                continue;
            }

            GameObject slimeObj = Managers.Resource.Instantiate("Slime");

            if (slimeObj != null)
            {
                Slime slime = slimeObj.GetComponent<Slime>();
                slime.SetSlimeType(ownedSlime.slimeType);

                SetRandomSpawnPosition(slimeObj);
                _spawnedSlimeTypes.Add(ownedSlime.slimeType);
            }
        }
    }

    public override void Clear() { }
    public override void Init()
    {
        CurrentScene = Define.SceneType.Main;

        SpawnSlimes();
    }

    private void OnEnable()
    {
        Managers.Data.UserDataManager.OnUserDataChanged -= SpawnSlimes;
        Managers.Data.UserDataManager.OnUserDataChanged += SpawnSlimes;

        Managers.Game.CollectIdleIncome();
    }

    private void OnDisable()
    {
        Managers.Data.UserDataManager.OnUserDataChanged -= SpawnSlimes;
        Managers.Data.UserDataManager.InitCurrentSavePlayTime();
        Managers.Game.StopIdleIncomeRoutine();
    }
}

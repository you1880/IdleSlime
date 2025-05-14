using System.Collections;
using System.Collections.Generic;
using Data.Slime;
using UnityEngine;

public class MainScene : BaseScene
{
    private const float MIN_X = -7.0f;
    private const float MAX_X = 7.0f;
    private const float MIN_Y = -2.5f;
    private const float MAX_Y = 1.5f;

    private List<OwnedSlime> ownedSlimes;
    
    private void SetRandomSpawnPosition(GameObject slimeObject)
    {
        float randomPosX = Random.Range(MIN_X, MAX_X);
        float randomPosY = Random.Range(MIN_Y, MAX_Y);

        slimeObject.transform.position = new Vector3(randomPosX, randomPosY, 0.0f);
    }
    
    private void SpawnSlimes()
    {
        foreach(OwnedSlime ownedSlime in ownedSlimes)
        {
            GameObject slimeObj = Managers.Resource.Instantiate("Slime");
            
            if(slimeObj != null)
            {
                Slime slime = slimeObj.GetComponent<Slime>();
                slime.SetSlimeType(ownedSlime.slimeType);

                SetRandomSpawnPosition(slimeObj);
            }
        }
    }

    public override void Clear() {}
    public override void Init()
    {
        CurrentScene = Define.SceneType.Main;
        
        ownedSlimes = Managers.Data.CurrentSaveData.ownedSlimes;

        SpawnSlimes();
    }
}

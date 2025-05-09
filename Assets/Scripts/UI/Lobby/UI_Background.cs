using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud
{
    public RectTransform cloudRect;
    public float cloudSpeed;

    public Cloud(RectTransform rect, float speed)
    {
        cloudRect = rect;
        cloudSpeed = speed;
    }
}

public class UI_Background : UI_Base
{
    public enum GameObjects
    {
        CloudA,
        CloudB
    }

    private const int START_MIN_X = -775;
    private const int START_MAX_X = -765;
    private const int END_X = 765;
    private const int MIN_Y_RANGE = 175;
    private const int MAX_Y_RANGE = 360;
    private const float CLOUD_MOVE_SPEED = 15.0f;
    private const float CLOUD_MAX_SPEED_RANGE = 30.0f;
    private List<Cloud> clouds = new List<Cloud>();

    private void InitializeCloudLocation(Cloud cloud)
    {
        int posX = Random.Range(START_MIN_X, START_MAX_X + 1);
        int posY = Random.Range(MIN_Y_RANGE, MAX_Y_RANGE + 1);
        float weight = Random.Range(0.0f, CLOUD_MAX_SPEED_RANGE);

        cloud.cloudRect.anchoredPosition = new Vector3(posX, posY, 0.0f);
        cloud.cloudSpeed = CLOUD_MOVE_SPEED + weight;
    }

    private void MoveCloud(Cloud cloud)
    {;
        Vector3 pos = cloud.cloudRect.anchoredPosition;
        pos.x += cloud.cloudSpeed * Time.deltaTime;
        cloud.cloudRect.anchoredPosition = pos;

        if(pos.x >= END_X)
        {
            InitializeCloudLocation(cloud);
        }
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        RectTransform _cloudA = GetObject((int)GameObjects.CloudA).GetComponent<RectTransform>();
        RectTransform _cloudB = GetObject((int)GameObjects.CloudB).GetComponent<RectTransform>();

        clouds.Add(new Cloud(_cloudA, CLOUD_MOVE_SPEED));
        clouds.Add(new Cloud(_cloudB, CLOUD_MOVE_SPEED));

        foreach(Cloud cloud in clouds)
        {
            InitializeCloudLocation(cloud);
        }
    }

    private void Update()
    {
        foreach(Cloud cloud in clouds)
        {
            MoveCloud(cloud);
        }
    }
}

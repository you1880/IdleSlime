using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');

            if(index >= 0)
            {
                name = name.Substring(index + 1);
            }
        }

        return Resources.Load<T>(path);
    }    

    public Sprite LoadSlimeSprite(int slimeType)
    {
        string path = $"Arts/Slime/Slime{slimeType}";

        return Resources.Load<Sprite>(path);
    }

    public Sprite LoadSlimeGradeSprite(int slimeGrade)
    {
        string path = $"Arts/Grade/Grade{slimeGrade}";

        return Resources.Load<Sprite>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");

        if (original == null)
        {
            Debug.Log($"Failed to Load Prefab : {path}");
            return null;
        }

        GameObject obj = Object.Instantiate(original, parent);
        obj.name = original.name;

        return obj;
    }

    public void Destroy(GameObject obj, float time = 0.0f)
    {
        if(obj == null)
        {
            return;
        }

        Object.Destroy(obj, time);
    }
}

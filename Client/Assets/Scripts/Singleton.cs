using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected void Init()
    {
        if(Instance != null) Destroy(gameObject);

        Instance = GetComponent<T>();
    }

    protected void DeInit()
    {
        Instance = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if(Instance != null) Destroy(gameObject);

        Instance = GetComponent<T>();
    }

    public virtual void OnDestroy()
    {
        Instance = null;
    }
}

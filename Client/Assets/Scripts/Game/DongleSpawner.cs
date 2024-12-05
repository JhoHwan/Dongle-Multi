using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DongleSpawner : DongleSpawnerBase
{
    private TransformSyncReceiver _transformSyncReceiver;
    public virtual void Awake()
    {
        _transformSyncReceiver = GetComponent<TransformSyncReceiver>();
    }

    public void UpdatePosition(float x)
    {
        Vector3 v = transform.localPosition;
        v.x = x;

        _transformSyncReceiver.UpdatePosition(v);
    }

    private void Update()
    {
        _transformSyncReceiver.InterPolationObject();
    }
}

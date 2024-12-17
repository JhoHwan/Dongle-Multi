using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

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

    public void UpdateDongle(List<DongleInfo> dongles)
    {
        foreach(DongleInfo dongleInfo in dongles)
        {
            Vector3 vec = new Vector3(dongleInfo.x, dongleInfo.y, 0);
            Dongle dongle;
            if (!_dongleMap.ContainsKey(dongleInfo.id))
            {
                dongle = (Dongle)SpawnDongle(dongleInfo.level, vec, dongleInfo.rotation, _pool.transform);
                _dongleMap.Add(dongleInfo.id, dongle);
            }
            else
            {
                dongle = (Dongle)_dongleMap[dongleInfo.id];
            }

            dongle.SetDongleInfo(dongleInfo);
        }
    }

    public override void DeSpawn(DongleBase dongle)
    {
        _pool.DespawnDongle(dongle);
        _dongleMap.Remove(dongle.GetDongleInfo().id);
    }

    private void Update()
    {
        _transformSyncReceiver.InterPolationObject();
    }
}

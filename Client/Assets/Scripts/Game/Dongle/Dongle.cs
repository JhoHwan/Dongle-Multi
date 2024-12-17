using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : DongleBase
{
    private TransformSyncReceiver _receiver;

    public override void Awake()
    {
        base.Awake();
        _receiver = GetComponent<TransformSyncReceiver>();
        _receiver.SetInterPolationTime(NetWorkManager.Instance.SendInterval);
    }

    public override void SetDongleInfo(DongleInfo info)
    {
        _info = info;
        Vector3 vec = new Vector3(info.x, info.y);
        _receiver.UpdatePosition(vec);
        _receiver.UpdateRotation(info.rotation);

        Level = info.level;
        if (info.isEnable == 0)
        {
            _owner.DeSpawn(this);
        }
    }

    private void Update()
    {
        _receiver.InterPolationObject();
    }
}

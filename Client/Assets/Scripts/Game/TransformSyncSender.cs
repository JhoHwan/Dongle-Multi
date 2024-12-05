using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransformSyncSender : MonoBehaviour
{
    private Vector3 _previousPosition;

    private const float SERVER_UPDATE_TIME = 0.1f;
    private WaitForSeconds _sleep = new WaitForSeconds(SERVER_UPDATE_TIME);

    private Action SendEvent;
    public void Init(Action Send)
    {
        SendEvent += Send;
        _previousPosition = transform.localPosition;
        StartCoroutine(SendPositionToServerCoroutine());
    }

    private bool HasPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        return Vector3.Distance(oldPosition, newPosition) > 0.01f; // 0.01 허용 오차
    }

    private IEnumerator SendPositionToServerCoroutine()
    {
        while (true)
        {
            yield return _sleep; // 0.1초 대기

            // 위치가 변경되었는지 확인
            if (HasPositionChanged(_previousPosition, transform.localPosition))
            {
                SendEvent();
                _previousPosition = transform.localPosition;
            }
        }
    }


}

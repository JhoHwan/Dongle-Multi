using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransformSyncReceiver : MonoBehaviour
{
    private Vector3 _previousPosition;
    private Vector3 _targetPosition;
    private bool _isUpadatePosition = false;
    private float _interpolationFactor = 0;

    public void UpdatePosition(Vector3 position)
    {
        _previousPosition = transform.transform.localPosition;
        _targetPosition = position;
        _isUpadatePosition = true;
        _interpolationFactor = 0;
    }

    public void InterPolationObject()
    {
        if (!_isUpadatePosition) return;

        _interpolationFactor += Time.deltaTime / (1.0f / 10.0f); // 속도 조절
        _interpolationFactor = Mathf.Clamp01(_interpolationFactor);

        transform.localPosition = Vector3.Lerp(_previousPosition, _targetPosition, _interpolationFactor);
        if (_interpolationFactor == 1.0f)
        {
            transform.localPosition = _targetPosition;
            _interpolationFactor = 0;
            _isUpadatePosition = false;
        }
    }
}

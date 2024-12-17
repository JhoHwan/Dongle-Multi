using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TransformSyncReceiver : MonoBehaviour
{
    private Vector3 _previousPosition;
    private Vector3 _targetPosition;
    private float _previousRotation;
    private float _targetRotation;
    private bool _isUpadatePosition = false;
    private float _interpolationFactor = 0;
    private float _interpolationTime = 0.1f;

    public void SetInterPolationTime(float interPolationTime)
    {
        _interpolationTime = interPolationTime;
    }

    public void UpdatePosition(Vector3 position)
    {
        _previousPosition = transform.transform.localPosition;
        _targetPosition = position;

        if (!HasPositionChanged(position, _previousPosition)) return;

        _isUpadatePosition = true;
        _interpolationFactor = 0;
    }

    private bool HasPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        return Vector3.Distance(oldPosition, newPosition) > 0.01f; // 0.01 허용 오차
    }

    private bool HasRotationChanged(float oldRotation, float newRotation)
    {
        return MathF.Abs(oldRotation - newRotation) > 0.01f; // 0.01 허용 오차
    }

    public void UpdateRotation(float rotation)
    {
        _previousRotation = transform.transform.rotation.eulerAngles.z;
        _targetRotation = rotation;

        if (!HasRotationChanged(_previousRotation, rotation)) return;

        _isUpadatePosition = true;
        _interpolationFactor = 0;
    }

    public void InterPolationObject()
    {
        if (!_isUpadatePosition) return;

        _interpolationFactor += Time.deltaTime / _interpolationTime; // 속도 조절
        _interpolationFactor = Mathf.Clamp01(_interpolationFactor);

        transform.localPosition = Vector3.Lerp(_previousPosition, _targetPosition, _interpolationFactor);
        float interpolationRotate = Mathf.LerpAngle(_previousRotation, _targetRotation, _interpolationFactor);
        transform.rotation = Quaternion.Euler(0, 0, interpolationRotate);

        if (_interpolationFactor == 1.0f)
        {
            transform.localPosition = _targetPosition;
            transform.rotation = Quaternion.Euler(0, 0, _targetRotation);
            _interpolationFactor = 0;
            _isUpadatePosition = false;
        }
    }
}

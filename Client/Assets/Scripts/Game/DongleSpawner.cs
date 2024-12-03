using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DongleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _donglePrefab;
    [SerializeField] private GameObject _line;

    protected float _dongleRadius;

    WaitForSeconds _sleepTime = new WaitForSeconds(0.01f);

    float _cacheX = 0;

    public void TurnOnLine(bool turnOn)
    {
        _line.SetActive(turnOn);
    }

    public void Start()
    {
        TurnOnLine(true);
        SpawnDongle(0);
    }

    public Dongle SpawnDongle(int level)
    {                                                                        
        GameObject newDongle = Instantiate(_donglePrefab, transform);
        Dongle dongle = newDongle.GetComponent<Dongle>();

        dongle.Init(level);
        _dongleRadius = dongle.GetRadius();
        return dongle;
    }

    public void UpdatePosition(float x)
    {
        _cacheX = x;
        StartCoroutine(LerpPosition());
    }

    IEnumerator LerpPosition()
    {
        int currentFrame = 0;
        int totalFrames = 6;
        float x = transform.localPosition.x;
        Vector3 end = transform.localPosition;
        end.x = _cacheX;

        while (currentFrame < totalFrames)
        {
            float t = (float)currentFrame / totalFrames;
            Vector3 p = transform.localPosition;
            p.x = Mathf.Lerp(x, _cacheX, t);
            transform.localPosition = p;
            currentFrame++;

            yield return _sleepTime; // 한 프레임 대기
        }
        transform.localPosition = end;
    }
}

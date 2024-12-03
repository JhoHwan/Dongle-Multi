using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DongleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _donglePrefab;
    [SerializeField] private GameObject _line;

    protected float _dongleRadius;

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
}

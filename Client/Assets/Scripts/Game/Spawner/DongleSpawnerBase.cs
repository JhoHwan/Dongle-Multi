using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DongleSpawnerBase : MonoBehaviour
{
    [SerializeField] private GameObject _donglePrefab;
    [SerializeField] private GameObject _line;
    protected float _dongleRadius;

    protected PlayerDongle _curDongle;
    

    public void TurnOnLine(bool turnOn)
    {
        _line.SetActive(turnOn);
    }

    public virtual void Start()
    {
        TurnOnLine(true);
        _curDongle = SpawnDongle(0);
    }

    public PlayerDongle SpawnDongle(int level)
    {                                                                        
        GameObject newDongle = Instantiate(_donglePrefab, transform);
        PlayerDongle dongle = newDongle.GetComponent<PlayerDongle>();

        dongle.Init(level);
        _dongleRadius = dongle.GetRadius(); 
        return dongle;
    }
}

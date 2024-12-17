using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DongleSpawnerBase : MonoBehaviour
{
    [SerializeField] protected DonglePool _pool;
    [SerializeField] private GameObject _donglePrefab;
    [SerializeField] private GameObject _line;
    protected float _dongleRadius;
    protected Dictionary<ushort, DongleBase> _dongleMap = new Dictionary<ushort, DongleBase>();


    protected DongleBase _curDongle;

    protected ushort _curID = 0;

    public void TurnOnLine(bool turnOn)
    {
        _line.SetActive(turnOn);
    }

    public virtual void Start()
    {
        TurnOnLine(true);
    }

    public virtual DongleBase SpawnDongle(ushort level, Vector3 position, float rotation, Transform parent)
    {
        DongleBase dongle = _pool.SpawnDongle(position, parent);
        dongle.SetPool(this);
        dongle.Init(_curID++, level);
        
        _dongleRadius = dongle.GetRadius(); 

        return dongle;
    }

    public virtual void DeSpawn(DongleBase dongle)
    {
        _pool.DespawnDongle(dongle);
    }
}

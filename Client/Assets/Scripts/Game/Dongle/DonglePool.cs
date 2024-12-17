using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DonglePool : MonoBehaviour
{
    [SerializeField] private GameObject _donglePrefab;
    private ObjectPool<DongleBase> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<DongleBase>(
            createFunc: CreateDongle,
            actionOnGet: OnGetDongle,
            actionOnRelease: OnReleaseDongle,
            actionOnDestroy: OnDestroyDongle,
            collectionCheck: false,
            defaultCapacity: 25,
            maxSize: 50
        );
    }

    private DongleBase CreateDongle()
    {
        DongleBase dongle = Instantiate(_donglePrefab).GetComponent<DongleBase>();
        return dongle;
    }

    private void OnGetDongle(DongleBase dongle)
    { 
        dongle.gameObject.SetActive(true);
    }

    private void OnReleaseDongle(DongleBase dongle)
    {
        dongle.gameObject.SetActive(false);
        dongle.SetPool(null);
    }

    private void OnDestroyDongle(DongleBase dongle)
    {
        Destroy(dongle);
    }    

    public DongleBase SpawnDongle(Vector3 spawnPos, Transform parent = null)
    {
        DongleBase dongle = _pool.Get();
        dongle.transform.parent = parent;
        dongle.transform.localPosition = spawnPos;   
        return dongle;
    }

    public void DespawnDongle(DongleBase dongle)
    {
        _pool.Release(dongle);
    }
}

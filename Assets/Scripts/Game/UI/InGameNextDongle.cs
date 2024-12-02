using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameNextDongle : MonoBehaviour
{
    [SerializeField] private List<GameObject> _images;

    public void SetDongleImage(int num)
    {
        foreach (GameObject image in _images)
        {
            image.SetActive(false);
        }

        (_images[num])?.SetActive(true);
    }
}

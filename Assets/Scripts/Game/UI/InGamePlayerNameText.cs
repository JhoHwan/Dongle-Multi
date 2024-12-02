using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePlayerNameText : MonoBehaviour
{
    public int PlayerNum;

    private Text _nameText;

    private void Awake()
    {
        _nameText = GetComponent<Text>();
    }

    public void SetName(string name)
    {
        _nameText.text = name;
    }

    public void SetColor(Color color)
    {
        _nameText.color = color;
    }
}

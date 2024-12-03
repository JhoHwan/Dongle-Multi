using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] Text _serverIP;
    
    void Start()
    {
        SetServerIP();
    }


    void SetServerIP()
    {
        string text = "Connect : ";
        if(!NetWorkManager.Instance._session.IsConnected())
        {
            text += "Fail";
            return;
        }
        text += NetWorkManager.Instance.serverIP;
        text += ":";
        text += NetWorkManager.Instance.serverPort.ToString();

        _serverIP.text = text;
    }
}

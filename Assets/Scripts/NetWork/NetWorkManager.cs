using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using NetWork;

public class NetWorkManager : MonoBehaviourSingleton<NetWorkManager>
{
    public ServerSession _session = new ServerSession();
    public Connector _connector = new Connector();
    public string serverIP = "127.0.0.1";
    public int serverPort = 7777;

    private void Awake()
    {
        Init();
        Connect();
    }

    public void Connect()
    {
        IPAddress serverAddress = IPAddress.Parse(serverIP);
        IPEndPoint clientEP = new IPEndPoint(serverAddress, 7777);
        _connector.Connect(clientEP, () => { return _session; });
    }

    private void OnDestroy()
    {
        DeInit();
        _session.Disconnect();
        _session = null;
    }
}

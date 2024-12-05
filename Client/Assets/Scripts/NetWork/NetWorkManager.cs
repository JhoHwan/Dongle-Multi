using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using NetWork;
using System.Collections.Generic;

public class NetWorkManager : MonoBehaviourSingleton<NetWorkManager>
{
    public ServerSession _session = new ServerSession();
    public Connector _connector = new Connector();
    public string serverIP = "127.0.0.1";
    public int serverPort = 7777;

    private ClientPacketHandler _handler = new ClientPacketHandler();

    public override void Awake()
    {
        base.Awake();

        Connect();
    }

    public void Connect()
    {
        IPAddress serverAddress = IPAddress.Parse(serverIP);
        IPEndPoint clientEP = new IPEndPoint(serverAddress, 7777);
        _connector.Connect(clientEP, () => { return _session; });
    }

    public void Send(IPacket packet)
    {
        ArraySegment<byte> data;
        packet.Serialize(out data);

        Send(data);
    }

    public void Send(ArraySegment<byte> data)
    {
        _session.Send(data);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        _session.Disconnect();
        _session = null;
        _handler = null;
    }
}

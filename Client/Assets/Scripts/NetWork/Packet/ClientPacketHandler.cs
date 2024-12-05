using NetWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPacketHandler : ClientPacketHandler_Generated
{
    public static ClientPacketHandler Instance;

    public ClientPacketHandler()
    {
        Instance = this;
    }

    public override void Dispatch_GC_BroadCastMoveSpawner(ArraySegment<byte> _buffer)
    {
        Debug.Log("Dispatch_GC_BroadCastMoveSpawner");

        GC_BroadCastMoveSpawner packet = new GC_BroadCastMoveSpawner();
        packet.DeSerialize(_buffer);
        GameManager.Instance.CreateJob(() => { GameManager.Instance.SpawnerMove(packet); });
    }

    public override void Dispatch_GC_CheckKeepAlive(ArraySegment<byte> _buffer)
    {
        Debug.Log("Dispatch_GC_CheckKeepAlive");
    }
    

    public override void Dispatch_GC_ResponseEnterRoom(ArraySegment<byte> _buffer)
    {
        Debug.Log("Dispatch_GC_ResponseEnterRoom");

        GC_ResponseEnterRoom packet = new GC_ResponseEnterRoom();
        packet.DeSerialize(_buffer);

        if(packet.bSuccess == 0)
        {
            Debug.LogError("Fail Enter Room");
            return;
        }
        Debug.Log($"Sucess Enter Room : {packet.roomID}");
    }

    public override void Dispatch_GC_SendPlayerInfo(ArraySegment<byte> _buffer)
    {
        GC_SendPlayerInfo p = new GC_SendPlayerInfo();
        p.DeSerialize(_buffer);
        GameManager.Instance.PlayerID = p.playerID;

        CG_RequestEnterRoom packet = new CG_RequestEnterRoom();
        packet.playerID = GameManager.Instance.PlayerID;
        packet.roomID = 0;
        Send_CG_RequestEnterRoom(packet);
    }

    public override void Send_CG_RequestEnterRoom(CG_RequestEnterRoom packet)
    {
        SendPacket(packet);
    }

    public override void Send_CG_ResponseKeepAlive(CG_ResponseKeepAlive packet)
    {
        SendPacket(packet);
    }

    public override void Send_CG_SendMoveSpawner(CG_SendMoveSpawner packet)
    {
        SendPacket(packet);
    }

    private void SendPacket(IPacket packet)
    {
        ArraySegment<byte> buffer;
        packet.Serialize(out buffer);
        NetWorkManager.Instance.Send(buffer);
    }
}
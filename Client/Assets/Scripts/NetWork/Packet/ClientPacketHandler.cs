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

    public override void Dispatch_GC_BroadCastDonglePool(ArraySegment<byte> _buffer)
    {
        //Debug.Log("Dispatch_GC_BroadCastDonglePool");

        GC_BroadCastDonglePool packet = new GC_BroadCastDonglePool();
        packet.DeSerialize( _buffer );
        if (packet.playerID == GameManager.Instance.PlayerID) return;
        GameManager.Instance.CreateJob(() => { GameManager.Instance.Room.UpdateDongle(packet.dongleInfos); });
    }

    public override void Dispatch_GC_BroadCastGameOver(ArraySegment<byte> _buffer)
    {
        GC_BroadCastGameOver packet = new GC_BroadCastGameOver();
        packet.DeSerialize( _buffer );
        GameManager.Instance.CreateJob(GameManager.Instance.Room.GameOver);
    }

    public override void Dispatch_GC_BroadCastGameStart(ArraySegment<byte> _buffer)
    {
        GC_BroadCastGameStart packet = new GC_BroadCastGameStart();
        packet.DeSerialize( _buffer );
        GameManager.Instance.CreateJob(GameManager.Instance.Room.GameStart);
    }

    public override void Dispatch_GC_BroadCastMergeDongle(ArraySegment<byte> _buffer)
    {
        GC_BroadCastMergeDongle packet = new GC_BroadCastMergeDongle();
        packet.DeSerialize( _buffer );
        if (packet.playerID == GameManager.Instance.PlayerID) return;

        GameManager.Instance.CreateJob(() => { GameManager.Instance.Room.DeleteDongle(packet.dongleID); });
    }

    public override void Dispatch_GC_CheckKeepAlive(ArraySegment<byte> _buffer)
    {

    }

    public override void Dispatch_GC_ExitPlayerRoom(ArraySegment<byte> _buffer)
    {
        GC_ExitPlayerRoom packet = new GC_ExitPlayerRoom();
        packet.DeSerialize( _buffer );
        GameManager.Instance.CreateJob(() => { GameManager.Instance.Room.ExitRoom(packet.playerID); });
    }

    public override void Dispatch_GC_ResponseEnterRoom(ArraySegment<byte> _buffer)
    {
        GC_ResponseEnterRoom packet = new GC_ResponseEnterRoom();
        packet.DeSerialize(_buffer);

        if(packet.bSuccess == 0)
        {
            Debug.LogError("Fail Enter Room");
            return;
        }
        GameManager.Instance.CreateJob(() => { GameManager.Instance.Room.EnterRoom(packet.playerID); });
        foreach(ushort id in packet.playerList)
        {
            GameManager.Instance.CreateJob(() => { GameManager.Instance.Room.EnterRoom(id); });
        }
    }

    public override void Dispatch_GC_ResponsePlayerReady(ArraySegment<byte> _buffer)
    {
        GC_ResponsePlayerReady packet = new GC_ResponsePlayerReady();
        packet.DeSerialize(_buffer);

        if (packet.playerID != GameManager.Instance.PlayerID)
        {
            GameManager.Instance.CreateJob(GameManager.Instance.Room.EnemyReady);
        }
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

    public override void Send_CG_MergeDongle(CG_MergeDongle packet)
    {
        SendPacket(packet);
    }

    public override void Send_CG_PlayerReady(CG_PlayerReady packet)
    {
        SendPacket(packet);
    }

    public override void Send_CG_RequestEnterRoom(CG_RequestEnterRoom packet)
    {
        SendPacket(packet);
    }

    public override void Send_CG_ResponseKeepAlive(CG_ResponseKeepAlive packet)
    {
        SendPacket(packet);
    }

    public override void Send_CG_SendDonglePool(CG_SendDonglePool packet)
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
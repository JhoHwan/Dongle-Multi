using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PacketDispatcher(ArraySegment<byte> _buffer);

public abstract class ClientPacketHandler_Generated : PacketHandler
{
    private PacketDispatcher[] _dispatcher = new PacketDispatcher[(ushort)PacketType.INVALID_PACKET];

    public ClientPacketHandler_Generated()
    {
        for (int i = 0; i < _dispatcher.Length; i++)
        {
            _dispatcher[i] = Dispatch_Invalid_Packet;
        }

        
        _dispatcher[(ushort)PacketType.GC_SendPlayerInfo] = Dispatch_GC_SendPlayerInfo;
        _dispatcher[(ushort)PacketType.GC_CheckKeepAlive] = Dispatch_GC_CheckKeepAlive;
        _dispatcher[(ushort)PacketType.GC_ResponseEnterRoom] = Dispatch_GC_ResponseEnterRoom;
        _dispatcher[(ushort)PacketType.GC_BroadCastMoveSpawner] = Dispatch_GC_BroadCastMoveSpawner;
    }

    public override bool ProcessPacket(ArraySegment<byte> _buffer)
    {
        PacketReader pr = new PacketReader(_buffer);
        PacketHeader header = new PacketHeader();
        pr.Read(ref header);

        _dispatcher[(ushort)(header.type)](_buffer);

        return true;
    }

    public void Dispatch_Invalid_Packet(ArraySegment<byte> _buffer)
    {
        Debug.LogError("Dispatch_Invalid_Packet");
    }

    
    public abstract void Dispatch_GC_SendPlayerInfo(ArraySegment<byte> _buffer);
    public abstract void Dispatch_GC_CheckKeepAlive(ArraySegment<byte> _buffer);
    public abstract void Dispatch_GC_ResponseEnterRoom(ArraySegment<byte> _buffer);
    public abstract void Dispatch_GC_BroadCastMoveSpawner(ArraySegment<byte> _buffer);

    
    public abstract void Send_CG_ResponseKeepAlive(CG_ResponseKeepAlive packet);
    
    public abstract void Send_CG_RequestEnterRoom(CG_RequestEnterRoom packet);
    
    public abstract void Send_CG_SendMoveSpawner(CG_SendMoveSpawner packet);
    
}

public abstract class PacketHandler
{
    public abstract bool ProcessPacket(ArraySegment<byte> _buffer);
}
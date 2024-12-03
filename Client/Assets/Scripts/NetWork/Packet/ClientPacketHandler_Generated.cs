using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
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

        
        _dispatcher[(ushort)PacketType.GC_TestPacket] = Dispatch_GC_TestPacket;
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

    
    public abstract void Dispatch_GC_TestPacket(ArraySegment<byte> _buffer);

    
    public abstract void Send_CG_TestPacket(CG_TestPacket packet);
    
}

public abstract class PacketHandler
{
    public abstract bool ProcessPacket(ArraySegment<byte> _buffer);
}
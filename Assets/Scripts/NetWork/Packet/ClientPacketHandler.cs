using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public delegate void PacketDispatcher(ArraySegment<byte> _buffer);

public class ClientPacketHandler : PacketHandler
{
    private PacketDispatcher[] _dispatcher = new PacketDispatcher[(ushort)PacketType.INVALID_PACKET];

    public ClientPacketHandler()
    {
        for (int i = 0; i < _dispatcher.Length; i++)
        {
            _dispatcher[i] = Dispatch_Invalid_Packet;
        }

        
        _dispatcher[(ushort)PacketType.CG_TestPacket] = Dispatch_CG_TestPacket;
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

    
    public void Dispatch_CG_TestPacket(ArraySegment<byte> _buffer)
    {
        // 구현 필요
    }
    

    
    public void Send_GC_TestPacket(GC_TestPacket packet)
    {
        // 구현 필요
    }
    
}

public abstract class PacketHandler
{
    public abstract bool ProcessPacket(ArraySegment<byte> _buffer);
}
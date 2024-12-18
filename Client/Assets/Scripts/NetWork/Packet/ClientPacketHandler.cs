using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void PacketDispatcher(ArraySegment<byte> _buffer);

public abstract class _Generated : PacketHandler
{
    private PacketDispatcher[] _dispatcher = new PacketDispatcher[(ushort)PacketType.INVALID_PACKET];

    public _Generated()
    {
        for (int i = 0; i < _dispatcher.Length; i++)
        {
            _dispatcher[i] = Dispatch_Invalid_Packet;
        }

        
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

    

    
}

public abstract class PacketHandler
{
    public abstract bool ProcessPacket(ArraySegment<byte> _buffer);
}
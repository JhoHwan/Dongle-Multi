using NetWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ClientPacketHandler : ClientPacketHandler_Generated
{
    public Session Owner;

    public ClientPacketHandler(Session owner) : base()
    {
        Owner = owner;
    }

    public override void Dispatch_GC_TestPacket(ArraySegment<byte> _buffer)
    {
        GC_TestPacket packet = new GC_TestPacket();
        packet.DeSerialize(_buffer);

        Debug.Log($"Recv GC_TestPacket : {packet.id}, message : {packet.message}, Count : {packet.id_list.Count}");

        CG_TestPacket packet2 = new CG_TestPacket();
        packet2.id = packet.id;
        packet2.message = packet.message;
        packet2.id_list = packet.id_list;

        Send_CG_TestPacket(packet2);
    }

    public override void Send_CG_TestPacket(CG_TestPacket packet)
    {
        SendPacket(packet);
    }

    private void SendPacket(IPacket packet)
    {
        ArraySegment<byte> buffer;
        packet.Serialize(out buffer);
        Owner.Send(buffer);
    }
}
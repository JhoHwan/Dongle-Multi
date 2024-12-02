using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PacketType : ushort
{
    GC_TestPacket,
    CG_TestPacket,
    LD_TestPacket,
    
    INVALID_PACKET
}

public class CG_TestPacket : IPacket
{
    // ���� ����
    public ushort id;
	
    // ���ڿ�
    public string message;
	
    // ����Ʈ
	public List<ushort> id_list;
	


    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // ���� ����
        size = size + sizeof(ushort);
		
        // ���ڿ� ����
		size += sizeof(ushort);
		size += message.Length * sizeof(char);
		
        //����Ʈ ����
        size += sizeof(ushort);
		size += id_list.Count * sizeof(ushort);
		
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.CG_TestPacket,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(id);
        pw.Write(message);
        pw.Write(id_list);
        

        if(pw.GetSize() != header.size)
        {
            buffer = null; 
            return false;
        }

        buffer = pw.GetBuffer();
        return true;
    }

    public bool DeSerialize(ArraySegment<byte> buffer)
    {
        PacketHeader packetHeader = new PacketHeader();

        PacketReader pr = new PacketReader(buffer);
        
        pr.Read(ref packetHeader);
        pr.Read(ref id);
        pr.Read(ref message);
        pr.Read(ref id_list);
        
        return true;
    }
}

public class GC_TestPacket : IPacket
{
    // ���� ����
    public ushort id;
	
    // ���ڿ�
    public string message;
	
    // ����Ʈ
	public List<ushort> id_list;
	


    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // ���� ����
        size = size + sizeof(ushort);
		
        // ���ڿ� ����
		size += sizeof(ushort);
		size += message.Length * sizeof(char);
		
        //����Ʈ ����
        size += sizeof(ushort);
		size += id_list.Count * sizeof(ushort);
		
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_TestPacket,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(id);
        pw.Write(message);
        pw.Write(id_list);
        

        if(pw.GetSize() != header.size)
        {
            buffer = null; 
            return false;
        }

        buffer = pw.GetBuffer();
        return true;
    }

    public bool DeSerialize(ArraySegment<byte> buffer)
    {
        PacketHeader packetHeader = new PacketHeader();

        PacketReader pr = new PacketReader(buffer);
        
        pr.Read(ref packetHeader);
        pr.Read(ref id);
        pr.Read(ref message);
        pr.Read(ref id_list);
        
        return true;
    }
}

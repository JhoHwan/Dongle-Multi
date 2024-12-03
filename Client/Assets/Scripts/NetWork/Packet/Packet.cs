using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PacketType : ushort
{
    GC_SendPlayerInfo,
    GC_CheckKeepAlive,
    CG_ResponseKeepAlive,
    CG_RequestEnterRoom,
    GC_ResponseEnterRoom,
    CG_SendMoveSpawner,
    GC_BroadCastMoveSpawner,
    
    INVALID_PACKET
}

public class CG_ResponseKeepAlive : IPacket
{
    // 고정 길이
    public byte playerID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + sizeof(byte);
        // 문자열 길이
		
        //리스트 길이
        
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.CG_ResponseKeepAlive,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        

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
        pr.Read(ref playerID);
        
        return true;
    }
}

public class CG_RequestEnterRoom : IPacket
{
    // 고정 길이
    public ushort playerID;
	public ushort roomID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + sizeof(ushort)+ sizeof(ushort);
        // 문자열 길이
		
        //리스트 길이
        
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.CG_RequestEnterRoom,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        pw.Write(roomID);
        

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
        pr.Read(ref playerID);
        pr.Read(ref roomID);
        
        return true;
    }
}

public class CG_SendMoveSpawner : IPacket
{
    // 고정 길이
    public ushort playerID;
	public ushort roomID;
	public float x;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + sizeof(ushort)+ sizeof(ushort)+ sizeof(float);
        // 문자열 길이
		
        //리스트 길이
        
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.CG_SendMoveSpawner,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        pw.Write(roomID);
        pw.Write(x);
        

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
        pr.Read(ref playerID);
        pr.Read(ref roomID);
        pr.Read(ref x);
        
        return true;
    }
}

public class GC_SendPlayerInfo : IPacket
{
    // 고정 길이
    public byte playerID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + sizeof(byte);
        // 문자열 길이
		
        //리스트 길이
        
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_SendPlayerInfo,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        

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
        pr.Read(ref playerID);
        
        return true;
    }
}

public class GC_CheckKeepAlive : IPacket
{
    // 고정 길이
    public byte playerID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + sizeof(byte);
        // 문자열 길이
		
        //리스트 길이
        
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_CheckKeepAlive,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        

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
        pr.Read(ref playerID);
        
        return true;
    }
}

public class GC_ResponseEnterRoom : IPacket
{
    // 고정 길이
    public ushort roomID;
	public byte bSuccess;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + sizeof(ushort)+ sizeof(byte);
        // 문자열 길이
		
        //리스트 길이
        
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_ResponseEnterRoom,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(roomID);
        pw.Write(bSuccess);
        

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
        pr.Read(ref roomID);
        pr.Read(ref bSuccess);
        
        return true;
    }
}

public class GC_BroadCastMoveSpawner : IPacket
{
    // 고정 길이
    public ushort playerID;
	public float x;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + sizeof(ushort)+ sizeof(float);
        // 문자열 길이
		
        //리스트 길이
        
        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_BroadCastMoveSpawner,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        pw.Write(x);
        

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
        pr.Read(ref playerID);
        pr.Read(ref x);
        
        return true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum PacketType : ushort
{
    GC_SendPlayerInfo,
    GC_CheckKeepAlive,
    CG_ResponseKeepAlive,
    CG_RequestEnterRoom,
    GC_ResponseEnterRoom,
    GC_ExitPlayerRoom,
    CG_PlayerReady,
    GC_ResponsePlayerReady,
    GC_BroadCastGameStart,
    CG_SendDonglePool,
    GC_BroadCastDonglePool,
    CG_MergeDongle,
    GC_BroadCastMergeDongle,
    GC_BroadCastGameOver,
    
    INVALID_PACKET
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
        size = size + Marshal.SizeOf(typeof(byte));
        // 문자열 길이
		
        // 리스트 길이
        

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
        size = size + Marshal.SizeOf(typeof(byte));
        // 문자열 길이
		
        // 리스트 길이
        

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
        size = size + Marshal.SizeOf(typeof(byte));
        // 문자열 길이
		
        // 리스트 길이
        

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
        size = size + Marshal.SizeOf(typeof(ushort))+ Marshal.SizeOf(typeof(ushort));
        // 문자열 길이
		
        // 리스트 길이
        

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

public class GC_ResponseEnterRoom : IPacket
{
    // 고정 길이
    public ushort playerID;
	public ushort roomID;
	public byte bSuccess;
	public RoomInfo roomInfo;
	
    // 문자열
    
    // 리스트
	public List<ushort> playerList;
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(ushort))+ Marshal.SizeOf(typeof(ushort))+ Marshal.SizeOf(typeof(byte))+ Marshal.SizeOf(typeof(RoomInfo));
        // 문자열 길이
		
        // 리스트 길이
        size += sizeof(ushort);
		size += playerList.Count * Marshal.SizeOf(typeof(ushort));
		

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
        pw.Write(playerID);
        pw.Write(roomID);
        pw.Write(bSuccess);
        pw.Write(roomInfo);
        pw.Write(playerList);
        

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
        pr.Read(ref bSuccess);
        pr.Read(ref roomInfo);
        pr.Read(ref playerList);
        
        return true;
    }
}

public class GC_ExitPlayerRoom : IPacket
{
    // 고정 길이
    public byte playerID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(byte));
        // 문자열 길이
		
        // 리스트 길이
        

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_ExitPlayerRoom,
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

public class CG_PlayerReady : IPacket
{
    // 고정 길이
    public ushort roomID;
	public ushort playerID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(ushort))+ Marshal.SizeOf(typeof(ushort));
        // 문자열 길이
		
        // 리스트 길이
        

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.CG_PlayerReady,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(roomID);
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
        pr.Read(ref roomID);
        pr.Read(ref playerID);
        
        return true;
    }
}

public class GC_ResponsePlayerReady : IPacket
{
    // 고정 길이
    public ushort playerID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(ushort));
        // 문자열 길이
		
        // 리스트 길이
        

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_ResponsePlayerReady,
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

public class GC_BroadCastGameStart : IPacket
{
    // 고정 길이
    public ushort playerID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(ushort));
        // 문자열 길이
		
        // 리스트 길이
        

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_BroadCastGameStart,
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

public class CG_SendDonglePool : IPacket
{
    // 고정 길이
    public ushort playerID;
	public ushort roomID;
	
    // 문자열
    
    // 리스트
	public List<DongleInfo> dongleInfos;
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(ushort))+ Marshal.SizeOf(typeof(ushort));
        // 문자열 길이
		
        // 리스트 길이
        size += sizeof(ushort);
		size += dongleInfos.Count * Marshal.SizeOf(typeof(DongleInfo));
		

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.CG_SendDonglePool,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        pw.Write(roomID);
        pw.Write(dongleInfos);
        

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
        pr.Read(ref dongleInfos);
        
        return true;
    }
}

public class GC_BroadCastDonglePool : IPacket
{
    // 고정 길이
    public ushort playerID;
	
    // 문자열
    
    // 리스트
	public List<DongleInfo> dongleInfos;
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(ushort));
        // 문자열 길이
		
        // 리스트 길이
        size += sizeof(ushort);
		size += dongleInfos.Count * Marshal.SizeOf(typeof(DongleInfo));
		

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_BroadCastDonglePool,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        pw.Write(dongleInfos);
        

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
        pr.Read(ref dongleInfos);
        
        return true;
    }
}

public class CG_MergeDongle : IPacket
{
    // 고정 길이
    public ushort playerID;
	public ushort roomID;
	public ushort dongleID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(ushort))+ Marshal.SizeOf(typeof(ushort))+ Marshal.SizeOf(typeof(ushort));
        // 문자열 길이
		
        // 리스트 길이
        

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.CG_MergeDongle,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        pw.Write(roomID);
        pw.Write(dongleID);
        

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
        pr.Read(ref dongleID);
        
        return true;
    }
}

public class GC_BroadCastMergeDongle : IPacket
{
    // 고정 길이
    public ushort playerID;
	public ushort dongleID;
	
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size + Marshal.SizeOf(typeof(ushort))+ Marshal.SizeOf(typeof(ushort));
        // 문자열 길이
		
        // 리스트 길이
        

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_BroadCastMergeDongle,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        pw.Write(playerID);
        pw.Write(dongleID);
        

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
        pr.Read(ref dongleID);
        
        return true;
    }
}

public class GC_BroadCastGameOver : IPacket
{
    // 고정 길이
    
    // 문자열
    
    // 리스트
	
    public ushort GetDataSize()
    {
        int size = sizeof(ushort) + sizeof(ushort);
        
        // 고정 길이
        size = size ;
        // 문자열 길이
		
        // 리스트 길이
        

        return (ushort)size;
    }

    public bool Serialize(out ArraySegment<byte> buffer)
    {
        PacketHeader header = new PacketHeader
        {
            type = PacketType.GC_BroadCastGameOver,
            size = GetDataSize()
        };

        PacketWriter pw = new PacketWriter(header.size);
        pw.Write(header);
        

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
        
        return true;
    }
}

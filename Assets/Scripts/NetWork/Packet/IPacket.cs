using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PacketHeader
{
    public PacketType type;
    public ushort size;
}

public interface IPacket
{
    public ushort GetDataSize();
    public bool Serialize(out ArraySegment<byte> buffer);
    public bool DeSerialize(ArraySegment<byte> buffer);
}

public class PacketWriter
{
    private int _offset;
    private byte[] _buffer;

    public PacketWriter(int bufferSize)
    {
        _buffer = new byte[bufferSize];
        _offset = 0;
    }

    public ushort GetSize()
    {
        return (ushort)_offset;
    }

    public ArraySegment<byte> GetBuffer() { return new ArraySegment<byte>(_buffer); }

    public void Write<T>(T data) where T : struct
    {
        int size = Marshal.SizeOf(data);

        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(data, ptr, false);
            Marshal.Copy(ptr, _buffer, _offset, size);
            _offset += size;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public void Write(string data)
    {
        byte[] stringBytes = Encoding.Unicode.GetBytes(data);

        ushort len = (ushort)stringBytes.Length;
        Write(len);
        Array.Copy(stringBytes, 0, _buffer, _offset, len);

        _offset += len;
    }

    public void Write<T>(List<T> data) where T : struct
    {
        int size = data.Count * System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        Write((ushort)size);

        foreach(T item in data)
        {
            Write(item);
        }
    }
}

public class PacketReader
{
    private ArraySegment<byte> _buffer; 
    private int _offset;

    public PacketReader(ArraySegment<byte> buffer)
    {
        _buffer = buffer;
        _offset = 0;
    }

    public ushort GetSize()
    {
        return (ushort)_offset;
    }

    public void Read<T>(ref T data) where T : struct
    {
        int len = Marshal.SizeOf(typeof(T));

        // T로 변환
        IntPtr ptr = Marshal.AllocHGlobal(len);
        try
        {
            // 바이트 배열에서 포인터로 데이터 복사
            Marshal.Copy(_buffer.Array, _offset, ptr, len);

            // 포인터에서 T 구조체로 변환
            data = Marshal.PtrToStructure<T>(ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        _offset += len;
    }

    public void Read(ref string data)
    {
        ushort len = 0;
        Read(ref len);

        byte[] stringBytes = new byte[len];
        Array.Copy(_buffer.Array, _offset , stringBytes, 0, len);

        data = Encoding.Unicode.GetString(stringBytes);

        _offset += len;
    }

    public void Read<T>(ref List<T> data) where T : struct
    {
        ushort len = 0;
        Read(ref len);

        int typeSize = Marshal.SizeOf(typeof(T));

        int itemCount = len / typeSize;

        data = new List<T>(itemCount);

        // 리스트의 각 항목 읽기
        for (int i = 0; i < itemCount; i++)
        {
            T item = default;
            Read(ref item); // 항목 읽기
            data.Add(item); // 리스트에 추가
        }
    }
}
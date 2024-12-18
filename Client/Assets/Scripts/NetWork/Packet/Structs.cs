using System;
using System.Runtime.InteropServices;
///////////////////////////////
/////// AUTO-GENERATING ///////
///////////////////////////////


[StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
public struct RoomInfo
{
    
    public ushort id;
    public ushort playTime;
    public ushort maxPlayer;
}

[StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
public struct DongleInfo
{
    
    public ushort id;
    public ushort level;
    public float x;
    public float y;
    public float rotation;
}

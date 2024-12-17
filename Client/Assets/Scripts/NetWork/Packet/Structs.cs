using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 1), Serializable]
public struct DongleInfo
{
    public ushort id;
    public ushort level;
    public float x;
    public float y;
    public float rotation;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RoomInfo
{
    public ushort id;
    public ushort playTime;
    public ushort maxPlayer;
}

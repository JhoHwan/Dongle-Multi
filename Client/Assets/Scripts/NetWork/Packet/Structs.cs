using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct DongleInfo
{
    public ushort id;
    public ushort level;
    public byte isEnable;
    public float x;
    public float y;
    public float rotation;
}


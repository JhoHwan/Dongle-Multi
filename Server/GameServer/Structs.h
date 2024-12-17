#pragma once

#pragma pack(1)
struct DongleInfo
{
    uint16 id;
    uint16 level;
    float x;
    float y;
    float rotation;
};
#pragma pack()

#pragma pack(1)
struct RoomInfo
{
    uint16 id;
    uint16 playTime;
    uint16 maxPlayer;
};
#pragma pack()
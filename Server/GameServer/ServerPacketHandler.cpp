#include "pch.h"

#include "ServerPacketHandler.h"

bool ServerPacketHandler::ReadPacket(RecvBuffer& recvBuffer, OUT uint32& packetSize)
{
    if (recvBuffer.DataSize() < sizeof(PacketHeader)) return false;

    BYTE* buffer = recvBuffer.ReadPos();
    PacketHeader* header = reinterpret_cast<PacketHeader*>(buffer);

    if (header->packetSize > recvBuffer.DataSize())
    {
        return false;
    }

    packetSize = header->packetSize;

    //cout << "Recv Packet : " << packetSize << endl;

    return true;
}

bool ServerPacketHandler::ProcessPacket(BYTE* packet)
{
    PacketHeader* header = reinterpret_cast<PacketHeader*>(packet);
    PacketType type = header->packetType;

    _packetDispatchers[static_cast<uint16>(type)](packet);

    return true;
}

void ServerPacketHandler::Dispatch_CG_ResponseKeepAlive(BYTE* buffer)
{
}

void ServerPacketHandler::Dispatch_CG_RequestEnterRoom(BYTE* buffer)
{
    CG_RequestEnterRoom packet;
    packet.Deserialize(buffer);
    auto player = static_pointer_cast<GameServer>(_owner)->GetPlayer(packet.playerID);
    if (player == nullptr) return;

    auto room = GRoomManager.GetRoom(packet.roomID);

    if (room == nullptr)
    {
        GC_ResponseEnterRoom packet;
        packet.bSuccess = 0;
        auto sendBuffer = Send_GC_ResponseEnterRoom(packet);
        player->Send(sendBuffer);
        return;
    }

    room->EnterPlayer(player);
}

void ServerPacketHandler::Dispatch_CG_SendMoveSpawner(BYTE* buffer)
{
    CG_SendMoveSpawner rp;
    rp.Deserialize(buffer);
    auto room = GRoomManager.GetRoom(rp.roomID);

    GC_BroadCastMoveSpawner sp;
    sp.playerID = rp.playerID;
    sp.x = rp.x;

    cout << rp.playerID << ": move spawner (" << rp.x << ")" << endl;

    room->BroadCast(Send_GC_BroadCastMoveSpawner(sp));
}

shared_ptr<SendBuffer> ServerPacketHandler::Send_GC_SendPlayerInfo(GC_SendPlayerInfo& packet)
{
    auto sendBuffer = GSendBufferManager->Open(1024);
    packet.Serialize(sendBuffer->Buffer());
    sendBuffer->Close(packet.GetDataSize());
    return sendBuffer;
}

std::shared_ptr<SendBuffer> ServerPacketHandler::Send_GC_CheckKeepAlive(GC_CheckKeepAlive& packet)
{
    auto sendBuffer = GSendBufferManager->Open(1024);
    packet.Serialize(sendBuffer->Buffer());
    sendBuffer->Close(packet.GetDataSize());
    return sendBuffer;
}

shared_ptr<SendBuffer> ServerPacketHandler::Send_GC_ResponseEnterRoom(GC_ResponseEnterRoom& packet)
{
    auto sendBuffer = GSendBufferManager->Open(1024);
    packet.Serialize(sendBuffer->Buffer());
    sendBuffer->Close(packet.GetDataSize());
    return sendBuffer;
}

shared_ptr<SendBuffer> ServerPacketHandler::Send_GC_BroadCastMoveSpawner(GC_BroadCastMoveSpawner& packet)
{
    auto sendBuffer = GSendBufferManager->Open(1024);
    packet.Serialize(sendBuffer->Buffer());
    sendBuffer->Close(packet.GetDataSize());
    return sendBuffer;
}

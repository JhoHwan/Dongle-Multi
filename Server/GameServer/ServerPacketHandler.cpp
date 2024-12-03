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

void ServerPacketHandler::Dispatch_CG_TestPacket(BYTE* packet)
{
    cout << "CG_TestPacekt Recv" << endl;
}

shared_ptr<SendBuffer> ServerPacketHandler::Send_GC_TestPacket(GC_TestPacket& packet)
{
   auto sendBuffer = GSendBufferManager->Open(1024);
   packet.Serialize(sendBuffer->Buffer());
   sendBuffer->Close(packet.GetDataSize());
   return sendBuffer;
}

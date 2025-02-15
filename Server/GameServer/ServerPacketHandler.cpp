#include "pch.h"

#include "ServerPacketHandler.h"

PacketHandlerFunc GPacketHandler[UINT16_MAX];

bool Handle_INVALID(SessionRef& session, BYTE* buffer, int32 len)
{
    return false;
}

bool Handle_CS_LOGIN(SessionRef& session, Protocol::CS_LOGIN& pkt)
{
    return false;
}

bool Handle_CS_ENTER_GAME(SessionRef& session, Protocol::CS_ENTER_GAME& pkt)
{
    return false;
}

bool Handle_CS_LEAVE_GAME(SessionRef& session, Protocol::CS_LEAVE_GAME& pkt)
{
    return false;
}

bool Handle_CS_CHAT(SessionRef& session, Protocol::CS_CHAT& pkt)
{
    return false;
}

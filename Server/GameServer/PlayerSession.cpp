#include "pch.h"

#include "PlayerSession.h"
#include "GameServer.h"

IDGenerator PlayerSession::_idGenerator{};

PlayerSession::PlayerSession() : Session(), _id(-1)
{
}

PlayerSession::~PlayerSession()
{

}

void PlayerSession::OnConnected()
{
	Session::OnConnected();

}

void PlayerSession::OnDisconnected()
{
	Session::OnDisconnected();
}

void PlayerSession::OnSend(uint32 sentBytes)
{
	
}

void PlayerSession::OnRecv(BYTE* buffer, int32 len)
{
	auto session = GetSharedPtr();
	PacketHeader* header = reinterpret_cast<PacketHeader*>(buffer);

	ServerPacketHandler::HandlePacket(session, buffer, len);
}

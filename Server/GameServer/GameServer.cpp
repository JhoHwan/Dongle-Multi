#include "pch.h"

#include "GameServer.h"

GameServer::GameServer(NetAddress address, SessionFactory sessionFactory, shared_ptr<PacketHandler> packetHandler)
	: IOCPServer(address, sessionFactory, packetHandler)
{
}

shared_ptr<PlayerSession> GameServer::GetPlayer(uint16 id) const
{
	for (auto it = _sessions.begin(); it != _sessions.end(); ++it)
	{
		auto player = static_pointer_cast<PlayerSession>(*it);
		if (player->GetID() == id)
			return player;
	}

	return nullptr;
}
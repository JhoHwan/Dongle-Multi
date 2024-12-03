#include "pch.h"
#include "Room.h"
#include "RoomManager.h"

Room::Room(uint16 id) : _id(id), _playerCount(0)
{
}

void Room::EnterPlayer(shared_ptr<PlayerSession> player)
{
	player->SetRoom(shared_from_this());
	_players.insert(player);
	_playerCount++;

	GC_ResponseEnterRoom packet;
	packet.bSuccess = 1;
	packet.roomID = _id;

	player->Send(ServerPacketHandler::Send_GC_ResponseEnterRoom(packet));
}

void Room::ExitPlayer(shared_ptr<PlayerSession> player)
{
	_players.erase(player);
	_playerCount--;
	if (_playerCount <= 0)
	{
		//GRoomManager.ReleaseRoom(shared_from_this());
	}
}

void Room::BroadCast(shared_ptr<SendBuffer> sendBuffer)
{
	for (const auto& player : _players)
	{
		player->Send(sendBuffer);
	}
}

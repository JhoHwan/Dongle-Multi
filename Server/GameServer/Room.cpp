#include "pch.h"
#include "Room.h"
#include "RoomManager.h"

Room::Room(uint16 id) : _id(id), _playerCount(0), _readyCount(0), _state(State::WAITING)
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
	packet.playerID = player->GetID();

	for (const auto& p : _players)
	{
		if (p->GetID() == player->GetID()) continue;
		packet.playerList.emplace_back(p->GetID());
	}
	BroadCast(ServerPacketHandler::Send_GC_ResponseEnterRoom(packet));
}

void Room::ExitPlayer(shared_ptr<PlayerSession> player)
{
	_players.erase(player);
	_playerCount--;
	if (_playerCount <= 0)
	{
		//GRoomManager.ReleaseRoom(shared_from_this());
	}

	GC_ExitPlayerRoom packet;
	packet.playerID = player->GetID();
	auto buf = ServerPacketHandler::Send_GC_ExitPlayerRoom(packet);
	BroadCast(buf);
	_readyCount = 0;

	if (_state == State::INGAME)
	{
		_state = State::WAITING;
		GC_BroadCastGameOver sp;
		auto buf = ServerPacketHandler::Send_GC_BroadCastGameOver(sp);
		BroadCast(buf);
	}
}

void Room::BroadCast(shared_ptr<SendBuffer> sendBuffer)
{
	for (const auto& player : _players)
	{
		player->Send(sendBuffer);
	}
}

void Room::PlayerReady(uint16 playerID)
{
	_readyCount++;

	GC_ResponsePlayerReady packet;
	packet.playerID = playerID;
	auto buf = ServerPacketHandler::Send_GC_ResponsePlayerReady(packet);
	BroadCast(buf);

	if (_readyCount == 2)
	{
		GameStart();
	}
}

void Room::GameStart()
{
	cout << "Room(" << _id << ") Game Start!" << endl;
	_state = State::INGAME;
	GC_BroadCastGameStart packet;
	auto buf = ServerPacketHandler::Send_GC_BroadCastGameStart(packet);
	BroadCast(buf);
}

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
	_id = _idGenerator.GenerateID();
	GC_SendPlayerInfo packet;
	packet.playerID = _id;
	Send(ServerPacketHandler::Send_GC_SendPlayerInfo(packet));
}

void PlayerSession::OnDisconnected()
{
	Session::OnDisconnected();
	_idGenerator.ReleaseID(_id);

	if (_room != nullptr)
		_room->ExitPlayer(static_pointer_cast<PlayerSession>(shared_from_this()));
}

void PlayerSession::OnSend(uint32 sentBytes)
{
	cout << "Send Packet" << endl;
}

void PlayerSession::OnRecv(uint32 recvBytes)
{
}


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

void PlayerSession::OnRecv(uint32 recvBytes)
{
}


#include "pch.h"
#include "GameServer.h"


void Work(shared_ptr<IOCPServer> server)
{
	while (1)
	{
		server->Dispatch();
	}
}

int main()
{
	NetAddress addr(L"121.183.244.212", 7777);
	shared_ptr<GameServer> server 
		= std::make_shared<GameServer>(addr, make_shared<PlayerSession>, make_shared<ServerPacketHandler>());
	server->Start();

	GRoomManager.CreateRoom();

	vector<thread> threads;
	for (int i = 0; i < thread::hardware_concurrency() * 2; i++)
	{
		threads.emplace_back(Work, server);
	}

	

	while (true) {}

	server->Stop();
	for (int i = 0; i < thread::hardware_concurrency() * 2; i++)
	{
		threads[i].join();
	}
}

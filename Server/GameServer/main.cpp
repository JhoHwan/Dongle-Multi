#include "pch.h"
#include "PlayerSession.h"

void Work(shared_ptr<IOCPServer> server)
{
	while (1)
	{
		server->Dispatch();
	}
}

int main()
{
	NetAddress addr(L"127.0.0.1", 7777);
	SessionFactory sessionFactory = [](){ return make_shared<PlayerSession>(); };
	shared_ptr<IOCPServer> server = make_shared<IOCPServer>(addr, sessionFactory, make_shared<ServerPacketHandler>());
	server->Start();
	vector<thread> threads;

	for (int i = 0; i < thread::hardware_concurrency() * 2; i++)
	{
		threads.emplace_back(Work, server);
	}

	GC_TestPacket packet;
	packet.id = 1;
	packet.message = L"Hello World!";
	packet.id_list = { 1, 3, 4 };

	auto sendBuffer = ServerPacketHandler::Send_GC_TestPacket(packet);

	while (1)
	{
		this_thread::sleep_for(500ms);
		server->BroadCast(sendBuffer);
	}

	for (int i = 0; i < thread::hardware_concurrency() * 2; i++)
	{
		threads[i].join();
	}
}

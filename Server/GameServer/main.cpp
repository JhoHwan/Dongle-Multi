#include "pch.h"

#include "GameServer.h"
#include "Map.h"

void Work(shared_ptr<IOCPServer> server)
{
	while (1)
	{
		server->Dispatch(10, 10);
	}
}

atomic<int> g_test = 0;

void Test(DoubleJobQueue* map)
{
	this_thread::sleep_for(1s);

	shared_ptr<Job> job = make_shared<Job>([]()
		{
			g_test.fetch_add(1);
		});

	while (true)
	{
		map->InsertJob(job);
		auto nextTime = std::chrono::high_resolution_clock::now() + 2ms;
		while (std::chrono::high_resolution_clock::now() < nextTime)
		{
			std::this_thread::yield();
		}
		this_thread::sleep_for(0.1ms);
	}
}

using Timer = std::chrono::high_resolution_clock;

int main()
{
	ServerPacketHandler::Init();

	Map* map = new Map();

 	NetAddress addr(L"127.0.0.1", 7777);
	shared_ptr<GameServer> server 
		= std::make_shared<GameServer>(addr, make_shared<PlayerSession>);
	server->Start();

	const int THREAD_COUNT = /*thread::hardware_concurrency() * 2*/4;

	vector<thread> threads;
	for (int i = 0; i < THREAD_COUNT; i++)
	{
		threads.emplace_back(Work, server);
	}

	atomic<uint64> frame = 0;

	threads.emplace_back([&frame]() 
		{ 
			while (true)
			{
				this_thread::sleep_for(3s);
				cout << "작업 수 : "<< g_test.load() << ", 프레임 : " << frame.load() / 3 << endl;
				frame.store(0);
				g_test.store(0);
			}
		});

	while (true) 
	{
		auto remainTime = 33333us;
		auto time = map->ExecuteJob(remainTime);

		auto nextTime = Timer::now() + time;
		auto start = Timer::now();
		while (Timer::now() < nextTime)
		{
			std::this_thread::yield();
		}
		auto end = Timer::now();

		auto t = end - start;

		map->SwapQueue();
		frame.fetch_add(1);
	}

	delete map;
	server->Stop();
	for (int i = 0; i < THREAD_COUNT; i++)
	{
		threads[i].join();
	}
}

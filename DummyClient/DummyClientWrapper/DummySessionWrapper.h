#pragma once

using namespace System;

class DummySession;

namespace DummyClientWrapper 
{
	public ref class ManagedDummySession
	{
	public:
		ManagedDummySession();
		~ManagedDummySession();
		!ManagedDummySession();

		void OnConnected();

		void Send(String^ message);
		void Disconnect();

		SOCKET GetSocket();

		shared_ptr<Session> GetNativeSession();

	public:
		Action^ ConnectedEvent;

	private:
		std::shared_ptr<DummySession>* _native;
	};
}

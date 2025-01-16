#pragma once
#include <vcclr.h>

using namespace DummyClientWrapper;

public class DummySession : public Session
{
public:
	DummySession(gcroot<ManagedDummySession^> obj) : _managedObject(obj)
	{
	}

	virtual ~DummySession() override
	{

	}

	virtual void OnConnected() override
	{
		_managedObject->OnConnected();
	}

	virtual void OnDisconnected() override
	{

	}

	virtual void OnSend(uint32 sentBytes) override
	{

	}

	virtual void OnRecv(uint32 recvBytes) override
	{

	}
private:
	gcroot<ManagedDummySession^> _managedObject;
};

#include "pch.h"

#include "DummySession.h"

using namespace DummyClientWrapper;

DummyClientWrapper::ManagedDummySession::ManagedDummySession()
{
    _native = new std::shared_ptr<DummySession>(std::make_shared<DummySession>(gcroot<ManagedDummySession^>(this)));
}

DummyClientWrapper::ManagedDummySession::~ManagedDummySession()
{
    this->!ManagedDummySession();
}

DummyClientWrapper::ManagedDummySession::!ManagedDummySession()
{
}

void DummyClientWrapper::ManagedDummySession::OnConnected()
{
    if (ConnectedEvent == nullptr) return;
    ConnectedEvent();
}

void DummyClientWrapper::ManagedDummySession::Send(String^ message)
{
    IntPtr ptr = System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(message);
    void* messagePtr = static_cast<void*>(ptr.ToPointer());
    auto sendBuffer = make_shared<SendBuffer>(message->Length);
    sendBuffer->CopyData(messagePtr, message->Length);

    (*_native)->Send(sendBuffer);
}

void DummyClientWrapper::ManagedDummySession::Disconnect()
{
    (*_native)->Disconnect();
    (*_native).reset();
    delete _native;
}

SOCKET DummyClientWrapper::ManagedDummySession::GetSocket()
{
    return (*_native)->GetSocket();
}

shared_ptr<Session> DummyClientWrapper::ManagedDummySession::GetNativeSession()
{
    return (*_native);
}
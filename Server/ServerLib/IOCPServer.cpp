#include "pch.h"
#include "type.h"
#include "IOCPServer.h"
#include "IOCPEvent.h"
#include "SendBuffer.h"

IOCPServer::IOCPServer(NetAddress address, SessionFactory sessionFactory, shared_ptr<PacketHandler> packetHandler) 
    : _address(address), _iocpHandle(INVALID_HANDLE_VALUE),
      _sessionFactory(sessionFactory), _packetHandler(packetHandler)
{
}

IOCPServer::~IOCPServer()
{
    _listener.reset();
    _packetHandler.reset();
}

shared_ptr<Session> IOCPServer::CreateSession() const
{
    auto session = _sessionFactory();
    session->SetPacketHandler(_packetHandler);
    return session;
}

void IOCPServer::BroadCast(shared_ptr<SendBuffer> sendBuffer)
{
    lock_guard<mutex> lock(_mutex);
    if (_sessions.size() == 0) return;
    for (const auto& session : _sessions)
    {
        session->Send(sendBuffer);
    }
}

bool IOCPServer::Start()
{
    _packetHandler->SetOwner(shared_from_this());

    _iocpHandle = ::CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 0);
    if (_iocpHandle == INVALID_HANDLE_VALUE)
    {
        cout << "CreateIoCompletionPort Error" << endl;
        return false;
    }

    WSADATA wsaData;
    if (::WSAStartup(MAKEWORD(2, 2), OUT & wsaData))
    {
        cout << "WSAStartup Error" << endl;
        return false;
    }

    _listener = make_unique<Listener>(10);

    _listener->StartAccept(shared_from_this());
    return true;
}

bool IOCPServer::Stop()
{
    if (_iocpHandle != INVALID_HANDLE_VALUE)
    {
        ::CloseHandle(_iocpHandle);
        _iocpHandle = INVALID_HANDLE_VALUE;
    }

    ::WSACleanup();

    return true;
}

bool IOCPServer::Register(HANDLE handle)
{
    // IOCP에 핸들 등록
    const unsigned int threadNum = std::thread::hardware_concurrency() * 2; //스레드풀 크기 설정

    if (::CreateIoCompletionPort(handle, _iocpHandle, 0, threadNum) == nullptr)
    {
        cout << "Register Error: " << GetLastError() << endl;
        return false;
    }

    return true;
}

bool IOCPServer::RegisterSocket(SOCKET socket)
{
    // 소켓을 IOCP에 등록
    return Register(reinterpret_cast<HANDLE>(socket));
}

void IOCPServer::Dispatch()
{
    DWORD lpNumberOfBytes;
    IOCPEvent* iocpEvent = nullptr;
    ULONG_PTR key = 0;
    if (::GetQueuedCompletionStatus(_iocpHandle, &lpNumberOfBytes, &key, reinterpret_cast<LPOVERLAPPED*>(&iocpEvent), INFINITE))
    {
        EventType eventType = iocpEvent->GetEventType();
        if (eventType == EventType::Accept)
            _listener->ProcessAccept(static_cast<AcceptEvent*>(iocpEvent));
        else
            iocpEvent->session->Dispatch(iocpEvent, static_cast<uint32>(lpNumberOfBytes));
    }
    else
    {
        EventType eventType = iocpEvent->GetEventType();
        iocpEvent->session->Dispatch(iocpEvent, static_cast<uint32>(lpNumberOfBytes));
    }
}

void IOCPServer::AddSession(shared_ptr<Session> session)
{
    lock_guard<mutex> lock(_mutex);
    _sessions.insert(session);
    session->SetServer(shared_from_this());
}

void IOCPServer::DeleteSession(std::shared_ptr<Session> session)
{
    lock_guard<mutex> lock(_mutex);

    _sessions.erase(session);

    // 세션 객체의 레퍼런스 감소
    session.reset();
}
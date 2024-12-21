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
    lock_guard<mutex> lock(_lock);
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

void IOCPServer::DispatchIocpEvent(uint16 time)
{
    DWORD lpNumberOfBytes;
    IOCPEvent* iocpEvent = nullptr;
    ULONG_PTR key = 0;
    if (::GetQueuedCompletionStatus(_iocpHandle, &lpNumberOfBytes, &key, reinterpret_cast<LPOVERLAPPED*>(&iocpEvent), static_cast<DWORD>(time)))
    {
        EventType eventType = iocpEvent->GetEventType();
        if (eventType == EventType::Accept)
            _listener->ProcessAccept(static_cast<AcceptEvent*>(iocpEvent));
        else
            iocpEvent->session->Dispatch(iocpEvent, static_cast<uint32>(lpNumberOfBytes));
    }
    else
    {
        if (GetLastError() == WAIT_TIMEOUT) return;
        EventType eventType = iocpEvent->GetEventType();
        iocpEvent->session->Dispatch(iocpEvent, static_cast<uint32>(lpNumberOfBytes));
    }
}

void IOCPServer::DispatchJob(uint16 time)
{
    auto endTime = chrono::system_clock::now() + chrono::milliseconds(time);

    while (true)
    {
        auto jobQueue = GJobManager->Pop();
        if (jobQueue == nullptr) return;

        if (jobQueue->TryExcute() == false)
        {
            GJobManager->Push(jobQueue); // 이미 다른 쓰레드에서 작업중이면 다시 반환
        }

        auto curTime = chrono::system_clock::now();
        if (curTime >= endTime) // 한 개의 JobQueue작업 후 시간이 초과했으면 리턴
        {
            return;
        }
    }
}

void IOCPServer::Dispatch(uint16 iocpDispatchTime, uint16 jobDispatchTime)
{
    DispatchIocpEvent(iocpDispatchTime);

    DispatchJob(jobDispatchTime);
}

void IOCPServer::AddSession(shared_ptr<Session> session)
{
    lock_guard<mutex> lock(_lock);
    _sessions.insert(session);
    session->SetServer(shared_from_this());
}

void IOCPServer::DeleteSession(std::shared_ptr<Session> session)
{
    lock_guard<mutex> lock(_lock);

    _sessions.erase(session);

    // 세션 객체의 레퍼런스 감소
    session.reset();
}
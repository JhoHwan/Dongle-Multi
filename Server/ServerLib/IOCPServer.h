#pragma once
#include "NetAddress.h"
#include "Listener.h"
#include "Session.h"

class PacketHandler;

using SessionFactory = function<shared_ptr<Session>(void)>;

class IOCPServer : public std::enable_shared_from_this<IOCPServer>
{
public:

    // 생성자 및 소멸자
    IOCPServer() = delete;
    IOCPServer(NetAddress address, SessionFactory sessionFactory, shared_ptr<PacketHandler> packetHandler);
    ~IOCPServer();

    shared_ptr<Session> CreateSession() const;

    virtual void BroadCast(shared_ptr<SendBuffer> sendBuffer);

    // 서버 관리
    bool Start();
    bool Stop();

    // IOCP 등록
    bool Register(HANDLE handle);
    bool RegisterSocket(SOCKET socket);

    // 세션 관리
    void AddSession(std::shared_ptr<Session> session);
    void DeleteSession(std::shared_ptr<Session> session);

    

    // 이벤트 디스패처
    void Dispatch();

    // Getter
    inline const NetAddress& GetAddress() const { return _address; }

private:
    HANDLE _iocpHandle;
    NetAddress _address;

    unique_ptr<Listener> _listener;
    shared_ptr<PacketHandler> _packetHandler;

    set<std::shared_ptr<Session>> _sessions;
    SessionFactory _sessionFactory;
// Mutex
private:
    mutex _mutex;
};

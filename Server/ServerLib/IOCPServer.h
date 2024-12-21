#pragma once
#include "NetAddress.h"
#include "Listener.h"
#include "Session.h"

class PacketHandler;

using SessionFactory = function<shared_ptr<Session>(void)>;

class IOCPServer : public std::enable_shared_from_this<IOCPServer>
{
public:

    // ������ �� �Ҹ���
    IOCPServer() = delete;
    IOCPServer(NetAddress address, SessionFactory sessionFactory, shared_ptr<PacketHandler> packetHandler);
    ~IOCPServer();

    shared_ptr<Session> CreateSession() const;

    virtual void BroadCast(shared_ptr<SendBuffer> sendBuffer);

    // ���� ����
    bool Start();
    bool Stop();

    // IOCP ���
    bool Register(HANDLE handle);
    bool RegisterSocket(SOCKET socket);

    // ���� ����
    void AddSession(std::shared_ptr<Session> session);
    void DeleteSession(std::shared_ptr<Session> session);

    // �̺�Ʈ ����ó
    void DispatchIocpEvent(uint16);
    void DispatchJob(uint16);
    void Dispatch(uint16 iocpDispatchTime, uint16 jobDispatchTime);

    // Getter
    inline const NetAddress& GetAddress() const { return _address; }

protected:
    HANDLE _iocpHandle;
    NetAddress _address;

    unique_ptr<Listener> _listener;
    shared_ptr<PacketHandler> _packetHandler;

    set<std::shared_ptr<Session>> _sessions;
    SessionFactory _sessionFactory;
// Mutex
protected:
    mutex _lock;
};

#pragma once

#include "NetAddress.h"
#include "IOCPEvent.h"
#include "RecvBuffer.h"

class IOCPServer;
class SendBuffer;
class PacketHandler;

class Session : public std::enable_shared_from_this<Session>
{
    enum { MAX_SEND_SIZE = 512 };
public:
    // ������ �� �Ҹ���
    Session();
    ~Session();

    void Dispatch(IOCPEvent* iocpEvent, uint32 numberOfBytesTransferred);

    // ���� ����
    void Connect();
    void ProcessConnect();

    void Disconnect();
    void RegisterDisconnect();
    void ProcessDisconnect();

    // TODO : �ӽ÷� ���� ���, SendBuffer ���� ���� ����
    // ������ ���� ����
    void Send(shared_ptr<SendBuffer> sendBuffer);
    void RegisterSend();
    void ProcessSend(uint32 sentBytes);

    // ������ ���� ����
    void RegisterRecv();
    void ProcessRecv(uint32 recvBytes);

    // �̺�Ʈ �ڵ鷯
    virtual void OnConnected();
    virtual void OnDisconnected();
    virtual void OnSend(uint32 sentBytes);
    virtual void OnRecv(uint32 recvBytes);

    // Setter
    inline void SetServer(std::shared_ptr<IOCPServer> server) { _server = server; }
    inline void SetAddress(const NetAddress& address) { _address = address; }
    inline void SetPacketHandler(shared_ptr<PacketHandler> packetHandler) 
    { _packetHandler = packetHandler; }

    // Getteter 
    inline SOCKET GetSocket() const { return _socket; }
    inline RecvBuffer& GetRecvBuffer() { return _recvBuffer; }
    inline const NetAddress& GetAddress() const { return _address; }

private:
    atomic<bool> _isConnect;
    SOCKET _socket;
    std::shared_ptr<IOCPServer> _server;
    std::shared_ptr<PacketHandler> _packetHandler;
    NetAddress _address;

    RecvBuffer _recvBuffer;
    queue<shared_ptr<SendBuffer>> _sendQueue;

    recursive_mutex _sendLock;
    atomic<bool> _sendRegistered;
    //TODO : Lock-Free Queue �����غ���
//IOCP Event
private:
    RecvEvent _recvEvent;
    SendEvent _sendEvent;
    DisconnectEvent _disConnectEvent;
};


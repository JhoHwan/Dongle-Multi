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
    // 생성자 및 소멸자
    Session();
    ~Session();

    void Dispatch(IOCPEvent* iocpEvent, uint32 numberOfBytesTransferred);

    // 연결 관리
    void Connect();
    void ProcessConnect();

    void Disconnect();
    void RegisterDisconnect();
    void ProcessDisconnect();

    // TODO : 임시로 버퍼 사용, SendBuffer 따로 제작 예정
    // 데이터 전송 관리
    void Send(shared_ptr<SendBuffer> sendBuffer);
    void RegisterSend();
    void ProcessSend(uint32 sentBytes);

    // 데이터 수신 관리
    void RegisterRecv();
    void ProcessRecv(uint32 recvBytes);

    // 이벤트 핸들러
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

protected:
    atomic<bool> _isConnect;
    SOCKET _socket;
    std::shared_ptr<IOCPServer> _server;
    std::shared_ptr<PacketHandler> _packetHandler;
    NetAddress _address;

    RecvBuffer _recvBuffer;
    queue<shared_ptr<SendBuffer>> _sendQueue;

    recursive_mutex _sendLock;
    atomic<bool> _sendRegistered;
    //TODO : Lock-Free Queue 적용해보기
//IOCP Event
protected:
    RecvEvent _recvEvent;
    SendEvent _sendEvent;
    DisconnectEvent _disConnectEvent;
};


#pragma once

class NetAddress;

class SocketUtil
{
public:
    // ���� ����
    static SOCKET CreateSocket();

    // ���� ���ε�
    static bool Bind(SOCKET socket, const NetAddress& address);

    // ������ ���� ��� ���·� ����
    static bool Listen(SOCKET socket, int backlog = SOMAXCONN);

    // ���� �ɼ� ����
    static bool SetReuseAddr(SOCKET socket);
    static bool SetNoDelay(SOCKET socket);
    static bool SetLinger(SOCKET socket, uint16 onoff, uint16 linger);

    // Accept�� ���� �ɼ� ����
    static bool SetAcceptSockOption(SOCKET acceptedSocket, SOCKET listenSocket);

    // ��Ĺ���� NetAddress ���� �ҷ�����
    static bool GetNetAddressBySocket(SOCKET socket, NetAddress& netAddress);

    // ���� �ݱ�
    static void CloseSocket(SOCKET socket);

    static LPFN_DISCONNECTEX GetDisconnectEx();

private:
    static void LoadDisconnectEx();
    static LPFN_DISCONNECTEX DisconnectEx;

};
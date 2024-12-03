#include "pch.h"
#include "Session.h"
#include "SocketUtil.h"
#include "IOCPServer.h"
#include "SendBuffer.h"
#include "PacketHandler.h"

Session::Session() 
	: _recvEvent(), _disConnectEvent(), _recvBuffer(4096)
{ 
	_socket = SocketUtil::CreateSocket();
	_isConnect.store(false);
	_sendRegistered.store(false);
}

Session::~Session()
{
	cout << "Release Session" << endl;
	SocketUtil::CloseSocket(_socket);
}

void Session::Dispatch(IOCPEvent* iocpEvent, uint32 numberOfBytesTransferred)
{
	EventType eventType = iocpEvent->GetEventType();
	switch (eventType)
	{
	case EventType::Send:
		iocpEvent->session->ProcessSend(numberOfBytesTransferred);
		break;
	case EventType::Recv:
		iocpEvent->session->ProcessRecv(numberOfBytesTransferred);
		break;
	case EventType::Disconnect:
		iocpEvent->session->ProcessDisconnect();
		break;
	default:
		Disconnect();
		break;
	}
}

void Session::Send(shared_ptr<SendBuffer> sendBuffer)
{
	lock_guard<recursive_mutex> lock(_sendLock);
	_sendQueue.push(sendBuffer);

	if (_sendRegistered.exchange(true) == false)
	{
		RegisterSend();
	}
}

void Session::RegisterSend()
{
	if (_isConnect.load() == false)
		return;

	_sendEvent.Init();
	_sendEvent.session = shared_from_this();

	{
		lock_guard<recursive_mutex> lock(_sendLock);

		uint32 writeSize = 0;
		while (_sendQueue.empty() == false)
		{
			shared_ptr<SendBuffer> sendBuffer = _sendQueue.front();

			if (writeSize + sendBuffer->WriteSize() > MAX_SEND_SIZE)
			{
				break;
			}

			writeSize += sendBuffer->WriteSize();

			_sendQueue.pop();
			_sendEvent.sendBuffers.push_back(sendBuffer);
		}

	}

	vector<WSABUF> wsaBufs;
	for (auto sendBuffer : _sendEvent.sendBuffers)
	{
		WSABUF wsaBuf;
		wsaBuf.buf = reinterpret_cast<char*>(sendBuffer->Buffer());
		wsaBuf.len = sendBuffer->WriteSize();
		wsaBufs.emplace_back(wsaBuf);
	}

	DWORD lpNumberOfBytesSent;
	if (SOCKET_ERROR == ::WSASend(GetSocket(), wsaBufs.data(), static_cast<DWORD>(wsaBufs.size()), &lpNumberOfBytesSent, 0, reinterpret_cast<LPOVERLAPPED>(&_sendEvent), nullptr))
	{
		auto errCode = WSAGetLastError();
		if (errCode != WSA_IO_PENDING)
		{
			// TODO : Error Logging
			if (errCode != WSAECONNRESET)
			{
				cout << "WSASend Error : " << errCode << endl;
			}
			_sendEvent.session.reset(); // Release-Ref
			_sendEvent.sendBuffers.clear();
			_sendRegistered.store(false);
		}
	}
}

void Session::ProcessSend(uint32 sentBytes)
{
	_sendEvent.session.reset();
	_sendEvent.sendBuffers.clear();


	if (sentBytes == 0)
	{
		Disconnect();
		return;
	}

	OnSend(sentBytes);

	lock_guard<recursive_mutex> lock(_sendLock);
	if (_sendQueue.empty() == true)
	{
		_sendRegistered.store(false);
	}
	else
		RegisterSend();
}

void Session::RegisterRecv()
{
	if (_isConnect.load() == false)
		return;

	_recvEvent.session = shared_from_this();

	WSABUF wsaBuf{};
	wsaBuf.buf = reinterpret_cast<char*>(_recvBuffer.WritePos());
	wsaBuf.len = _recvBuffer.Capacity();

	DWORD lpNumberOfBytesRecvd = 0;
	DWORD flag = 0;
	if (SOCKET_ERROR == ::WSARecv(GetSocket(), &wsaBuf, 1, &lpNumberOfBytesRecvd, &flag, reinterpret_cast<LPOVERLAPPED>(&_recvEvent), nullptr))
	{
		auto errCode = WSAGetLastError();
		if (errCode != WSA_IO_PENDING)
		{
			//TODO : Error Log
			cout << "WSARecv Error" << errCode << endl;
			return;
		}
	}
}

void Session::ProcessRecv(uint32 recvBytes)
{
	_recvEvent.session.reset(); // Release-Ref
	if (_recvBuffer.OnWirte(recvBytes) == false)
	{
		cout << "RecvBuffer Overflow!" << endl;
		RegisterRecv();
		return;
	}

	if (recvBytes == 0)
	{
		Disconnect();
		return;
	}

	OnRecv(recvBytes);

	_recvBuffer.Clean();

	RegisterRecv();
}

void Session::Connect()
{
}

void Session::ProcessConnect()
{
	_isConnect.store(true);

	OnConnected();

	RegisterRecv();
}

void Session::Disconnect()
{
	_isConnect.store(false);

	RegisterDisconnect();
}

void Session::RegisterDisconnect()
{
	auto DisconnectEx = SocketUtil::GetDisconnectEx();
	_disConnectEvent.Init();
	_disConnectEvent.session = shared_from_this();

	if (SOCKET_ERROR == DisconnectEx(_socket, reinterpret_cast<LPOVERLAPPED>(&_disConnectEvent), TF_REUSE_SOCKET, 0))
	{
		auto errCode = WSAGetLastError();
		if (errCode != WSA_IO_PENDING)
		{
			_disConnectEvent.session.reset();
			cout << "DisconnectEx Error" << errCode << endl;
			return;
		}
	}
}

void Session::ProcessDisconnect()
{
	_disConnectEvent.session.reset();
	OnDisconnected();
	_server->DeleteSession(shared_from_this());
}

void Session::OnConnected()
{
	auto ip = _address.GetIpAddress();
	wprintf(L"Connect : %s\n", ip.c_str());
}

void Session::OnDisconnected()
{
	auto ip = _address.GetIpAddress();
	wprintf(L"Disconnect : %s\n", ip.c_str());
}

void Session::OnSend(uint32 sentBytes)
{
	//wprintf(L"Send : %d\n", sentBytes);
}

void Session::OnRecv(uint32 recvBytes)
{
	//wprintf(L"Recv : %d\n", recvBytes);

	if (_packetHandler == nullptr) return;

	uint32 packetSize = 0;

	while (_packetHandler->ReadPacket(_recvBuffer, OUT packetSize))
	{
		BYTE* packet = _recvBuffer.ReadPos();
		_recvBuffer.OnRead(packetSize);


		if (_packetHandler->ProcessPacket(packet))
		{
			//TODO : Error Log
		}
	}
}


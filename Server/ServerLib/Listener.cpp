#include "pch.h"
#include "Listener.h"

#include "IOCPServer.h"
#include "IOCPEvent.h"
#include "NetAddress.h"
#include "Session.h"
#include "SocketUtil.h"

Listener::Listener(unsigned int maxAcceptNum) : MAX_ACCEPT_NUM(maxAcceptNum), _acceptEvents(maxAcceptNum, AcceptEvent())
{

}

Listener::~Listener()
{
	cout << "Release Listener" << endl;
}

// 소캣 초기화 후 비동기 Accept를 등록한다.
bool Listener::StartAccept(shared_ptr<IOCPServer> server)
{
	cout << "Start Listener" << endl;

	_server = server;

	_socket = SocketUtil::CreateSocket();
	if (_socket == INVALID_SOCKET)
	{
		cout << "WSASocket Error : " << WSAGetLastError() << endl;
		return false;
	}

	server->RegisterSocket(_socket);

	// 소켓 옵션 SO_REUSEADDR 설정 (주소 재사용 허용)
	if (SocketUtil::SetReuseAddr(_socket) == false)
	{
		cout << "SetReuseAddr Error : " << WSAGetLastError() << endl;
		return false;
	}

	if (SocketUtil::SetLinger(_socket, 0, 0) == false)
		return false;

	// 소켓 바인딩
	if (SocketUtil::Bind(_socket, _server->GetAddress()) == false)
	{
		cout << "Bind Error : " << WSAGetLastError() << endl;
		return false;
	}



	// 소켓을 클라이언트 요청 수신 대기 상태로 설정
	if (SocketUtil::Listen(_socket) == false)
	{
		cout << "Listen Error : " << WSAGetLastError() << endl;
		return false;
	}

	for (int i = 0; i < MAX_ACCEPT_NUM; i++)
	{
		RegisterAccept(&_acceptEvents[i]);
	}

	cout << "Start Listener Finish" << endl;
	return true;
}

void Listener::RegisterAccept(AcceptEvent* acceptEvent)
{ 
	// 새로운 Session 생성
	shared_ptr<Session> session = _server->CreateSession();

	// acceptEvent 초기화 및 Session 연결
	acceptEvent->Init();
	acceptEvent->session = session;
	
	DWORD bytesReceived = 0;
	if (false == ::AcceptEx(_socket, session->GetSocket(), session->GetRecvBuffer().WritePos(), 0,
		sizeof(SOCKADDR_IN) + 16, sizeof(SOCKADDR_IN) + 16, OUT & bytesReceived, reinterpret_cast<LPOVERLAPPED>(acceptEvent)))
	{
		auto errCode = WSAGetLastError();
		if (errCode != WSA_IO_PENDING)
		{
			//TODO : LOG
			acceptEvent->session = nullptr;
			cout << errCode << endl;
			RegisterAccept(acceptEvent);
		}
	}
}

void Listener::ProcessAccept(AcceptEvent* acceptEvent)
{
	// 소캣을 IOCP 핸들에 연결
	if (_server->RegisterSocket(acceptEvent->session->GetSocket()) == false)
	{
		cout << "RegisterSocket Error" << endl;
		RegisterAccept(acceptEvent);
		return;
	}

	// Accept된 소켓에 리스닝 소켓의 옵션을 적용
	if (SocketUtil::SetAcceptSockOption(acceptEvent->session->GetSocket(), _socket) == false)
	{
		cout << "SetAcceptSockOption Error : " << WSAGetLastError() << endl;
		RegisterAccept(acceptEvent);
		return;
	}

	NetAddress address; 
	if (false == SocketUtil::GetNetAddressBySocket(acceptEvent->session->GetSocket(), address))
	{
		cout << "GetNetAddressBySocket Error : " << WSAGetLastError() << endl;
		RegisterAccept(acceptEvent);
		return;
	}
	acceptEvent->session->SetAddress(address);

	// 서버에 세션 추가
	_server->AddSession(acceptEvent->session);
	acceptEvent->session->ProcessConnect();
	acceptEvent->session = nullptr;

	// 다음 연결 요청을 처리하도록 Accept 등록
	RegisterAccept(acceptEvent);
}
#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include <thread>
#include <vector>

#pragma comment(lib, "ws2_32.lib")

void ClientWorker(int clientId, const std::string& serverIp, int serverPort)
{
    for (int connectionAttempt = 0; connectionAttempt < 1; ++connectionAttempt) // 최대 3번 연결 시도
    {
        SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (clientSocket == INVALID_SOCKET)
        {
            std::cerr << "Client " << clientId << ": Socket creation failed!" << std::endl;
            return;
        }

        sockaddr_in serverAddr = {};
        serverAddr.sin_family = AF_INET;
        serverAddr.sin_port = htons(serverPort);
        inet_pton(AF_INET, serverIp.c_str(), &serverAddr.sin_addr);

        if (connect(clientSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
        {
            std::cerr << "Client " << clientId << ": Connection failed!" << std::endl;
            closesocket(clientSocket);
            continue; // 재시도
        }

        std::cout << "Client " << clientId << ": Connected to server!" << std::endl;

        wchar_t recvBuffer[1024] = {};
        for (int i = 0; i < 0; i++) // 5번 메시지 송수신
        {
            int bytesReceived = recv(clientSocket, reinterpret_cast<char*>(recvBuffer), sizeof(recvBuffer), 0);
            if (bytesReceived > 0)
            {
                std::wcout << L"Client " << clientId << L": Received" << std::endl;
                int sendResult = send(clientSocket, reinterpret_cast<const char*>(recvBuffer), bytesReceived, 0);
                if (sendResult == SOCKET_ERROR)
                {
                    std::cerr << "Client " << clientId << ": Send failed!" << std::endl;
                    break;
                }
            }
            else if (bytesReceived == 0)
            {
                std::cout << "Client " << clientId << ": Server closed connection." << std::endl;
                break;
            }
            else
            {
                std::cerr << "Client " << clientId << ": Receive failed!" << std::endl;
                break;
            }

            memset(recvBuffer, 0, sizeof(recvBuffer));
        }
        std::this_thread::sleep_for(std::chrono::seconds(3));

        closesocket(clientSocket);
        std::cout << "Client " << clientId << ": Disconnected after 5 messages." << std::endl;

        // 연결 끊고 1초 대기 후 다시 연결
    }
}

int main()
{
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
    {
        std::cerr << "WSAStartup failed!" << std::endl;
        return 1;
    }

    const std::string serverIp = "127.0.0.1"; // 서버 IP
    const int serverPort = 7777;             // 서버 포트
    const int clientCount = 100;               // 생성할 클라이언트 수

    std::vector<std::thread> clientThreads;

    for (int i = 1; i <= clientCount; ++i)
    {
        clientThreads.emplace_back(ClientWorker, i, serverIp, serverPort);
    }

    for (auto& thread : clientThreads)
    {
        thread.join();
    }

    WSACleanup();
    return 0;
}

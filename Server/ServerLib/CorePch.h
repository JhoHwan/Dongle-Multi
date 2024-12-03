#pragma once

#include <memory>
#include <atomic>
#include <iostream>
#include <thread>
#include <algorithm>
#include <vector>
#include <queue>
#include <array>
#include <set>
#include <mutex>
#include <functional>
#include <limits>

using namespace std;

#include <WinSock2.h>
#include <WS2tcpip.h>
#include <mswsock.h>
#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib,"mswsock.lib")

#include "Type.h"

#include "RecvBuffer.h"

#include "SendBuffer.h"
#include "SendBufferChunk.h"
#include "IPacket.h"
#include "PacketHandler.h"
#include "SendBufferManager.h"
#include "IOCPServer.h"
#include "RecvBuffer.h"

#include "CoreTLS.h"
#include "CoreGlobal.h"








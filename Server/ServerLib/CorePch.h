#pragma once

#include "Type.h"

#include <memory>
#include <atomic>
#include <iostream>
#include <thread>
#include <algorithm>
#include <vector>
#include <queue>
#include <array>
#include <set>
#include <map>
#include <mutex>
#include <functional>
#include <limits>
#include <shared_mutex>
#include <chrono>

#include <WinSock2.h>
#include <WS2tcpip.h>
#include <mswsock.h>
#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib,"mswsock.lib")

using namespace std;

#include "RecvBuffer.h"

#include "SendBuffer.h"
#include "SendBufferChunk.h"
#include "IPacket.h"
#include "PacketHandler.h"
#include "SendBufferManager.h"
#include "IOCPServer.h"
#include "RecvBuffer.h"
#include "Job.h"
#include "JobQueue.h"
#include "JobManager.h"

#include "CoreTLS.h"
#include "CoreGlobal.h"








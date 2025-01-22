#pragma once

#include "CorePch.h"

#pragma comment(lib, "ServerLib.lib")

#ifdef _DEBUG
#pragma comment(lib,"libprotobufd.lib")
#else
#pragma comment(lib,"libprotobuf.lib")
#endif // DEBUG

#include "Protobuf\Protocol.pb.h"

#include "FlatBuffer\Protocol_generated.h"

#include "Structs.h"
#include "IDGenerator.h"
#include "Packet.h"
#include "ServerPacketHandler.h"
#include "PlayerSession.h"
#include "Room.h"
#include "RoomManager.h"
#include "GameServer.h"

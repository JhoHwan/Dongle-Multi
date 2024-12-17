#pragma once
///////////////////////////////
/////// AUTO-GENERATING ///////
///////////////////////////////

using PacketDispatcher = std::function<void(BYTE*)>;

class ServerPacketHandler : public PacketHandler
{
public:
	ServerPacketHandler()
	{
		for (int i = 0; i < _packetDispatchers.size(); i++)
		{
			_packetDispatchers[i] = std::bind(&ServerPacketHandler::DispatchInvalidPacket, this);
		}
		
		_packetDispatchers[static_cast<uint16>(PacketType::CG_ResponseKeepAlive)] = std::bind(&ServerPacketHandler::Dispatch_CG_ResponseKeepAlive, this, placeholders::_1);
		_packetDispatchers[static_cast<uint16>(PacketType::CG_RequestEnterRoom)] = std::bind(&ServerPacketHandler::Dispatch_CG_RequestEnterRoom, this, placeholders::_1);
		_packetDispatchers[static_cast<uint16>(PacketType::CG_SendDonglePool)] = std::bind(&ServerPacketHandler::Dispatch_CG_SendDonglePool, this, placeholders::_1);

	}
	~ServerPacketHandler() {}

// PacketDispatcher
public:
	void Dispatch_CG_ResponseKeepAlive(BYTE* buffer);
	void Dispatch_CG_RequestEnterRoom(BYTE* buffer);
	void Dispatch_CG_SendDonglePool(BYTE* buffer);
	

public:
	static shared_ptr<SendBuffer> Send_GC_SendPlayerInfo(GC_SendPlayerInfo& packet);
	static shared_ptr<SendBuffer> Send_GC_CheckKeepAlive(GC_CheckKeepAlive& packet);
	static shared_ptr<SendBuffer> Send_GC_ResponseEnterRoom(GC_ResponseEnterRoom& packet);
	static shared_ptr<SendBuffer> Send_GC_BroadCastDonglePool(GC_BroadCastDonglePool& packet);
	

// PacketHandler을(를) 통해 상속됨
public:
	bool ReadPacket(RecvBuffer& recvBuffer, OUT uint32& packetSize) override;
	bool ProcessPacket(BYTE* packet) override;

private:
	array<PacketDispatcher, UINT16_MAX> _packetDispatchers;
};

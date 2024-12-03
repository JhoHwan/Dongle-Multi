#pragma once
///////////////////////////////
/////// AUTO-GENERATING ///////
///////////////////////////////

using PacketDispatcher = std::function<void(void)>;

class ServerPacketHandler.h : public PacketHandler
{
public:
	ServerPacketHandler()
	{
		for (int i = 0; i < _packetDispatchers.size(); i++)
		{
			_packetDispatchers[i] = std::bind(&ServerPacketHandler::DispatchInvalidPacket, this);
		}
		
		_packetDispatchers[static_cast<uint16>(PacketType::GC_TestPacket)] = std::bind(&ServerPacketHandler::Dispatch_GC_TestPacket, this);

	}
	~ServerPacketHandler() {}

// PacketDispatcher
public:
	void Dispatch_GC_TestPacket();
	

public:
	void Send_CG_TestPacket();
	

// PacketHandler을(를) 통해 상속됨
public:
	bool ReadPacket(RecvBuffer& recvBuffer, OUT uint32& packetSize) override;
	bool ProcessPacket(BYTE* packet) override;

private:
	array<PacketDispatcher, UINT16_MAX> _packetDispatchers;
};

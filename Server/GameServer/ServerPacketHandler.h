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
		
		_packetDispatchers[static_cast<uint16>(PacketType::CG_TestPacket)] = std::bind(&ServerPacketHandler::Dispatch_CG_TestPacket, this, placeholders::_1);
		_packetDispatchers[static_cast<uint16>(PacketType::CG_ResponseKeepAlive)] = std::bind(&ServerPacketHandler::Dispatch_CG_ResponseKeepAlive, this, placeholders::_1);

	}
	~ServerPacketHandler() {}

// PacketDispatcher
public:
	void Dispatch_CG_TestPacket(BYTE* buffer);
	void Dispatch_CG_ResponseKeepAlive(BYTE* buffer);
	

public:
	static shared_ptr<SendBuffer> Send_GC_TestPacket(GC_TestPacket& packet);
	static shared_ptr<SendBuffer> Send_GC_CheckKeepAlive(GC_CheckKeepAlive& packet);
	

// PacketHandler을(를) 통해 상속됨
public:
	bool ReadPacket(RecvBuffer& recvBuffer, OUT uint32& packetSize) override;
	bool ProcessPacket(BYTE* packet) override;

private:
	array<PacketDispatcher, UINT16_MAX> _packetDispatchers;
};

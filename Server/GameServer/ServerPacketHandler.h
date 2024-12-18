#pragma once
///////////////////////////////
/////// AUTO-GENERATING ///////
///////////////////////////////

using PacketDispatcher = std::function<void(BYTE*)>;

class  : public PacketHandler
{
public:
	()
	{
		for (int i = 0; i < _packetDispatchers.size(); i++)
		{
			_packetDispatchers[i] = std::bind(&ServerPacketHandler::DispatchInvalidPacket, this);
		}
		

	}
	~() {}

// PacketDispatcher
public:
	

public:
	

// PacketHandler��(��) ���� ��ӵ�
public:
	bool ReadPacket(RecvBuffer& recvBuffer, OUT uint32& packetSize) override;
	bool ProcessPacket(BYTE* packet) override;

private:
	array<PacketDispatcher, UINT16_MAX> _packetDispatchers;
};

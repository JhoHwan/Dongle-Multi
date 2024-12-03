#pragma once

enum class PacketType : uint16;

class IOCPServer;

class PacketHandler
{
public:
	void SetOwner(shared_ptr<IOCPServer> owner) { _owner = owner; }

	virtual bool ReadPacket(RecvBuffer& recvBuffer, OUT uint32& packetSize) = 0;
	virtual bool ProcessPacket(BYTE* packet) = 0;

	inline void DispatchInvalidPacket()
	{
		cout << "InvalidPacket" << endl;
	}

private:
	shared_ptr<IOCPServer> _owner;
};


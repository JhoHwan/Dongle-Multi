#pragma once
///////////////////////////////
/////// AUTO-GENERATING ///////
///////////////////////////////

enum class PacketType : uint16
{
    GC_SendPlayerInfo,
	GC_CheckKeepAlive,
	CG_ResponseKeepAlive,
	CG_RequestEnterRoom,
	GC_ResponseEnterRoom,
	CG_SendMoveSpawner,
	GC_BroadCastMoveSpawner,
	CG_SendDonglePool,
	GC_BroadCastDonglePool,
	
	INVALID_PACKET,
};

class GC_SendPlayerInfo : public IPacket
{
public:
	//고정 길이
	uint8 playerID;
	
	//문자열
	
	//리스트
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(playerID);
		//문자열 size
		
		//리스트 size
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::GC_SendPlayerInfo;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << playerID;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> playerID;

		return GetDataSize() == header.packetSize;
	}
};

class GC_CheckKeepAlive : public IPacket
{
public:
	//고정 길이
	uint8 playerID;
	
	//문자열
	
	//리스트
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(playerID);
		//문자열 size
		
		//리스트 size
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::GC_CheckKeepAlive;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << playerID;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> playerID;

		return GetDataSize() == header.packetSize;
	}
};

class GC_ResponseEnterRoom : public IPacket
{
public:
	//고정 길이
	uint16 roomID;
	uint8 bSuccess;
	
	//문자열
	
	//리스트
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(roomID)+ sizeof(bSuccess);
		//문자열 size
		
		//리스트 size
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::GC_ResponseEnterRoom;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << roomID << bSuccess;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> roomID >> bSuccess;

		return GetDataSize() == header.packetSize;
	}
};

class GC_BroadCastMoveSpawner : public IPacket
{
public:
	//고정 길이
	uint16 playerID;
	float x;
	
	//문자열
	
	//리스트
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(playerID)+ sizeof(x);
		//문자열 size
		
		//리스트 size
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::GC_BroadCastMoveSpawner;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << playerID << x;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> playerID >> x;

		return GetDataSize() == header.packetSize;
	}
};

class GC_BroadCastDonglePool : public IPacket
{
public:
	//고정 길이
	uint16 playerID;
	
	//문자열
	
	//리스트
	vector<DongleInfo> dongleInfos;
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(playerID);
		//문자열 size
		
		//리스트 size
		size += sizeof(uint16);
		size += dongleInfos.size() * sizeof(DongleInfo);
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::GC_BroadCastDonglePool;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << playerID << dongleInfos;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> playerID >> dongleInfos;

		return GetDataSize() == header.packetSize;
	}
};

class CG_ResponseKeepAlive : public IPacket
{
public:
	//고정 길이
	uint8 playerID;
	
	//문자열
	
	//리스트
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(playerID);
		//문자열 size
		
		//리스트 size
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::CG_ResponseKeepAlive;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << playerID;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> playerID;

		return GetDataSize() == header.packetSize;
	}
};

class CG_RequestEnterRoom : public IPacket
{
public:
	//고정 길이
	uint16 playerID;
	uint16 roomID;
	
	//문자열
	
	//리스트
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(playerID)+ sizeof(roomID);
		//문자열 size
		
		//리스트 size
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::CG_RequestEnterRoom;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << playerID << roomID;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> playerID >> roomID;

		return GetDataSize() == header.packetSize;
	}
};

class CG_SendMoveSpawner : public IPacket
{
public:
	//고정 길이
	uint16 playerID;
	uint16 roomID;
	float x;
	
	//문자열
	
	//리스트
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(playerID)+ sizeof(roomID)+ sizeof(x);
		//문자열 size
		
		//리스트 size
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::CG_SendMoveSpawner;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << playerID << roomID << x;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> playerID >> roomID >> x;

		return GetDataSize() == header.packetSize;
	}
};

class CG_SendDonglePool : public IPacket
{
public:
	//고정 길이
	uint16 playerID;
	uint16 roomID;
	
	//문자열
	
	//리스트
	vector<DongleInfo> dongleInfos;
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//고정 길이 size
		size = size + sizeof(playerID)+ sizeof(roomID);
		//문자열 size
		
		//리스트 size
		size += sizeof(uint16);
		size += dongleInfos.size() * sizeof(DongleInfo);
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::CG_SendDonglePool;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << playerID << roomID << dongleInfos;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> playerID >> roomID >> dongleInfos;

		return GetDataSize() == header.packetSize;
	}
};

#pragma once
///////////////////////////////
/////// AUTO-GENERATING ///////
///////////////////////////////

enum class PacketType : uint16
{
    GC_TestPacket,
	CG_TestPacket,
	
	INVALID_PACKET,
};

class GC_TestPacket : public IPacket
{
public:
	//���� ����
	uint16 id;
	
	//���ڿ�
	wstring message;
	
	//����Ʈ
	vector<uint16> id_list;
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//���� ���� size
		size = size + sizeof(id);
		
		//���ڿ� size
		size += sizeof(uint16);
		size += message.size() * sizeof(wchar);
		
		//����Ʈ size
		size += sizeof(uint16);
		size += id_list.size() * sizeof(uint16);
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::GC_TestPacket;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << id << message << id_list;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> id >> message >> id_list;

		return GetDataSize() == header.packetSize;
	}
};

class CG_TestPacket : public IPacket
{
public:
	//���� ����
	uint16 id;
	
	//���ڿ�
	wstring message;
	
	//����Ʈ
	vector<uint16> id_list;
	
public:
	uint16 GetDataSize() const override
	{
		size_t size = sizeof(PacketHeader);

		//���� ���� size
		size = size + sizeof(id);
		
		//���ڿ� size
		size += sizeof(uint16);
		size += message.size() * sizeof(wchar);
		
		//����Ʈ size
		size += sizeof(uint16);
		size += id_list.size() * sizeof(uint16);
		
		return static_cast<uint16>(size);
	}

	bool Serialize(BYTE* buffer) const override
	{
		PacketHeader header;
		header.packetType = PacketType::CG_TestPacket;
		header.packetSize = GetDataSize();

		PacketWriter pw(buffer);
		pw << header << id << message << id_list;
		
		return pw.GetSize() == GetDataSize();
	}

	bool Deserialize(BYTE* buffer) override
	{
		PacketReader pr(buffer);

		PacketHeader header;
		pr >> header >> id >> message >> id_list;

		return GetDataSize() == header.packetSize;
	}
};

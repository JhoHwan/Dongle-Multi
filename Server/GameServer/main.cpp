#include "pch.h"
#include "GameServer.h"

void Work(shared_ptr<IOCPServer> server)
{
	while (1)
	{
		server->Dispatch(10, 10);
	}
}

class CustomAllocator : public flatbuffers::Allocator {
public:
	explicit CustomAllocator(uint8_t* buffer, size_t size)
		: buffer_(buffer), capacity_(size), offset_(0) {
	}

	// �޸� �Ҵ�
	uint8_t* allocate(size_t size) override {
		if (offset_ + size > capacity_) {
			throw std::runtime_error("CustomAllocator: Out of memory!");
		}
		uint8_t* allocated = buffer_ + offset_;
		offset_ += size;
		return allocated;
	}

	// �޸� ���� (FlatBuffers������ �Ϲ������� �ʿ� ����)
	void deallocate(uint8_t* p, size_t size) override {
		// CustomAllocator�� �޸� ������ ó������ ����
	}

	// ���� ������ ��ȯ
	size_t get_offset() const { return offset_; }

private:
	uint8_t* buffer_; // ����ڰ� ������ �޸� ����
	size_t capacity_; // ���� ũ��
	size_t offset_;   // ���� ������
};

template<typename T>
struct is_std_vector : std::false_type {};

template<typename T, typename Alloc>
struct is_std_vector<std::vector<T, Alloc>> : std::true_type {};

template<typename Builder, typename... Args>
auto transformArgs(Builder& builder, Args&&... args) {
	return std::make_tuple(
		[&]() -> decltype(auto) {
			if constexpr (std::is_same_v<std::decay_t<Args>, std::string> || std::is_same_v<std::decay_t<Args>, const char*>) {
				// std::string or const char* -> FlatBuffers�� string
				return builder.CreateString(args);
			}
			else if constexpr (is_std_vector<std::decay_t<Args>>::value) {
				// std::vector<T> -> FlatBuffers�� Vector<T>
				using ElementType = typename std::decay_t<Args>::value_type;
				return builder.CreateVector(args);
			}
			else {
				// �ٸ� Ÿ���� �״�� ��ȯ
				return std::forward<Args>(args);
			}
		}()...
			);
}

template<typename Ret, typename... FuncArgs ,typename... Args>
shared_ptr<SendBuffer> SerializeFlatBuffer(Flatbuffer::PacketContent type, flatbuffers::Offset<Ret>(*func)(flatbuffers::FlatBufferBuilder&, FuncArgs...) , Args&&... args)
{
	std::vector<BYTE> vec(128);

	BYTE* buf = vec.data();

	CustomAllocator allocator(buf, 128);

	flatbuffers::FlatBufferBuilder builder(128, &allocator);

	auto transformedArgs = transformArgs(builder, std::forward<Args>(args)...);

	auto message = std::apply([&](auto&&... transformed) {
		return func(builder, std::forward<decltype(transformed)>(transformed)...);
		}, transformedArgs);


	auto packet = Flatbuffer::CreatePacket(builder, type, message.Union());

	builder.Finish(packet);

	PacketHeader* header = reinterpret_cast<PacketHeader*>(builder.GetBufferPointer() - sizeof(PacketHeader));

	header->packetType = PacketType::GC_SendPlayerInfo;
	header->packetSize = builder.GetSize() + sizeof(PacketHeader);

	shared_ptr<SendBuffer> sendbuffer = make_shared<SendBuffer>(header->packetSize);
	sendbuffer->CopyData(header, header->packetSize);

	return sendbuffer;
}

int main()
{
	uint16_t id = 12;
	string name = "Cho";

	vector<uint16> vec;
	vec.push_back(1);
	vec.push_back(2);
	vec.push_back(3);

	auto sendbuffer = SerializeFlatBuffer(Flatbuffer::PacketContent_S_Test2, Flatbuffer::CreateS_Test2, id, "Cho", vec);
	auto buffer = sendbuffer->Buffer();

	// ������ȭ
	PacketHeader* recvHeader = reinterpret_cast<PacketHeader*>(buffer);
	auto type = recvHeader->packetType;
	auto size = recvHeader->packetSize;
	
	auto recvPacket = Flatbuffer::GetPacket(&recvHeader[1]);

	auto message = recvPacket->content_as_S_Test2();
	auto newid = message->id();
	string newName = message->name()->str();
	for (auto num : *message->ids())
	{
		cout << num << " ";
	}

 	NetAddress addr(L"127.0.0.1", 7777);
	shared_ptr<GameServer> server 
		= std::make_shared<GameServer>(addr, make_shared<PlayerSession>, make_shared<ServerPacketHandler>());
	server->Start();

	GRoomManager.CreateRoom();

	vector<thread> threads;
	for (int i = 0; i < thread::hardware_concurrency() * 2; i++)
	{
		threads.emplace_back(Work, server);
	}

	while (true) {}

	server->Stop();
	for (int i = 0; i < thread::hardware_concurrency() * 2; i++)
	{
		threads[i].join();
	}
}

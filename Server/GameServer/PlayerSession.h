#pragma once

class Room;

class PlayerSession : public Session
{
public:
	PlayerSession();
	~PlayerSession();

	// 이벤트 핸들러
	virtual void OnConnected() override;
	virtual void OnDisconnected() override;
	virtual void OnSend(uint32 sentBytes) override;
	virtual void OnRecv(uint32 recvBytes) override;

	uint16 GetID() const { return _id; }
	void SetRoom(shared_ptr<Room> room) { _room = room; }
private:
	uint16 _id;
    static IDGenerator _idGenerator;

	std::shared_ptr<IOCPServer> _server;
	std::shared_ptr<Room> _room;
};


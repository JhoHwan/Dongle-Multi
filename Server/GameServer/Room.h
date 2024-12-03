#pragma once

class Room;

class Room : public enable_shared_from_this<Room>
{
public:
	Room(uint16 id);
	~Room() {}
	uint16 GetID() const { return _id; }
	
	void EnterPlayer(shared_ptr<PlayerSession> player);
	void ExitPlayer(shared_ptr<PlayerSession> player);

	void BroadCast(shared_ptr<SendBuffer> sendBuffer);

private:
	uint8 _playerCount;
	uint16 _id;
	set<shared_ptr<PlayerSession>> _players;
};
#pragma once

class Room;

class Room : public enable_shared_from_this<Room>
{
public:
	enum State
	{
		WAITING,
		INGAME,
		FINISHED,
		INVALID
	}; 

	Room(uint16 id);
	~Room() {}
	uint16 GetID() const { return _id; }
	
	void EnterPlayer(shared_ptr<PlayerSession> player);
	void ExitPlayer(shared_ptr<PlayerSession> player);

	void BroadCast(shared_ptr<SendBuffer> sendBuffer);

	void PlayerReady(uint16 playerID);
	void GameStart();

private:
	State _state;
	uint8 _playerCount;
	uint16 _id;

	uint16 _readyCount;

	set<shared_ptr<PlayerSession>> _players;
};
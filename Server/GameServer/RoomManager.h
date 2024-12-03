#pragma once

class RoomManager
{
public:
	void CreateRoom();
	void ReleaseRoom(shared_ptr<Room> room);
	shared_ptr<Room> GetRoom(uint16 id);

private:
	static IDGenerator _idGenerator;

	shared_mutex _lock;
	map<uint16, shared_ptr<Room>> _rooms;

};

extern RoomManager GRoomManager;
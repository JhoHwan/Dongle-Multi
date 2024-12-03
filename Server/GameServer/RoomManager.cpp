#include "pch.h"

#include "RoomManager.h"

RoomManager GRoomManager;

IDGenerator RoomManager::_idGenerator;

void RoomManager::CreateRoom()
{
	unique_lock lock(_lock);
	uint16 id = _idGenerator.GenerateID();
	shared_ptr<Room> room = make_shared<Room>(id);
	_rooms[id] = room;
}

void RoomManager::ReleaseRoom(shared_ptr<Room> room)
{
	unique_lock lock(_lock);
	_idGenerator.ReleaseID(room->GetID());
	_rooms.erase(room->GetID());
	room.reset();
}

shared_ptr<Room> RoomManager::GetRoom(uint16 id)
{
	if(_rooms.count(id) == 0) 	
		return nullptr;

	return _rooms[id];
}

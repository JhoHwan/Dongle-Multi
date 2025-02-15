#pragma once

class GameServer : public IOCPServer
{
public:
	GameServer(NetAddress address, SessionFactory sessionFactory);

	shared_ptr<PlayerSession> GetPlayer(uint16 id) const;
private:
};


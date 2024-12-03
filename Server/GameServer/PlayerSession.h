#pragma once

class PlayerSession : public Session
{
public:
	PlayerSession();
	~PlayerSession();

	virtual void OnSend(uint32 sentBytes)
	{
		cout << "Send Packet" << endl;
	}

private:
	int id;

};


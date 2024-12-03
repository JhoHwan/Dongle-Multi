#include "pch.h"
#include "CoreGlobal.h"
#include "SendBufferManager.h"

SendBufferManager* GSendBufferManager;

class CoreGlobal
{
public:
	CoreGlobal()
	{
		GSendBufferManager = new SendBufferManager();
	}

	~CoreGlobal()
	{
		delete GSendBufferManager;
	}
} GCoreGlobal;
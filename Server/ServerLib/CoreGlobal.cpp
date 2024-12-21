#include "pch.h"
#include "CoreGlobal.h"
#include "SendBufferManager.h"

SendBufferManager* GSendBufferManager;
JobManager* GJobManager;

class CoreGlobal
{
public:
	CoreGlobal()
	{
		GSendBufferManager = new SendBufferManager();
		GJobManager = new JobManager();
	}

	~CoreGlobal()
	{
		delete GSendBufferManager;
		delete GJobManager;
	}
} GCoreGlobal;
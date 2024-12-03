#include "pch.h"
#include "CoreTLS.h"
#include "SendBufferChunk.h"

thread_local shared_ptr<SendBufferChunk> LSendBufferChunk;

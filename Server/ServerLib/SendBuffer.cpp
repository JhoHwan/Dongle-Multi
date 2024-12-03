#include "pch.h"
#include "SendBuffer.h"
#include "SendBufferChunk.h"

SendBuffer::SendBuffer(shared_ptr<SendBufferChunk> owner, BYTE* buffer, uint32 allocSize)
	:_buffer(buffer), _writeSize(0), _allocSize(allocSize), _owner(owner)
{
}

SendBuffer::~SendBuffer()
{
}

void SendBuffer::Close(uint32 writeSize)
{
	if(_allocSize >= writeSize)
	_writeSize = writeSize;
	_owner->Close(writeSize);
}



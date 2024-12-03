#include "pch.h"
#include "SendBufferChunk.h"

SendBufferChunk::SendBufferChunk() : _isOpen(false), _usedSize(0)
{
	_buffer.resize(SEND_BUFFER_CHUNK_SIZE);
}

SendBufferChunk::~SendBufferChunk()
{
}

void SendBufferChunk::Reset()
{
	_isOpen = false;
	_usedSize = 0;
}

shared_ptr<SendBuffer> SendBufferChunk::Open(uint32 allocSize)
{
	if (_isOpen == true) return nullptr;

	if (allocSize > Capacity())
		return nullptr;

	_isOpen = true;


	return make_shared<SendBuffer>(shared_from_this(), Buffer(), allocSize);
}

void SendBufferChunk::Close(uint32 writeSize)
{
	if (_isOpen == false) return;

	_isOpen = false;
	_usedSize += writeSize;
}

#include "pch.h"
#include "SendBufferManager.h"
#include "SendBufferChunk.h"
#include "SendBuffer.h"

shared_ptr<SendBuffer> SendBufferManager::Open(uint32 size)
{
	if (LSendBufferChunk == nullptr)
	{
		LSendBufferChunk = Pop();
		LSendBufferChunk->Reset();
	}

	if (LSendBufferChunk->Capacity() < size)
	{
		LSendBufferChunk = Pop();
		LSendBufferChunk->Reset();
	}

	return LSendBufferChunk->Open(size);
}

shared_ptr<SendBufferChunk> SendBufferManager::Pop()
{
	{
		lock_guard<mutex> lock(_lock);
		if (_sendBufferChunks.empty() == false)
		{
			shared_ptr<SendBufferChunk> sendBufferChunk = _sendBufferChunks.back();
			_sendBufferChunks.pop_back();
			return sendBufferChunk;
		}
	}

	return shared_ptr<SendBufferChunk>(new SendBufferChunk(), PushGlobal);
}

void SendBufferManager::Push(shared_ptr<SendBufferChunk> buffer)
{
	lock_guard<mutex> lock(_lock);
	_sendBufferChunks.push_back(buffer);
}

void SendBufferManager::PushGlobal(SendBufferChunk* buffer)
{
	GSendBufferManager->Push(shared_ptr<SendBufferChunk>(buffer, PushGlobal));
}

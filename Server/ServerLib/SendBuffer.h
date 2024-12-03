#pragma once

class SendBufferChunk;

class SendBuffer
{
public:
	SendBuffer(shared_ptr<SendBufferChunk> owner, BYTE* buffer, uint32 allocSize);
	~SendBuffer();

	BYTE* Buffer() { return _buffer; }
	uint32 WriteSize() const { return _writeSize; }
	void Close(uint32 writeSize);

private:
	BYTE* _buffer;
	uint32 _allocSize;
	uint32 _writeSize;

	shared_ptr<SendBufferChunk> _owner;
};


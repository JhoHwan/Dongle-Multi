#pragma once

class SendBuffer;

class SendBufferChunk : public enable_shared_from_this<SendBufferChunk>
{
	enum { SEND_BUFFER_CHUNK_SIZE = 6000 };

public:
	SendBufferChunk();
	~SendBufferChunk();

	void Reset();
	shared_ptr<SendBuffer> Open(uint32 allocSize);
	void Close(uint32 writeSize);

	inline bool IsOpen() const { return _isOpen; }
	inline BYTE* Buffer() { return &_buffer[_usedSize]; }
	inline uint32 Capacity() { return static_cast<uint32>(_buffer.size()) - _usedSize; }

private:
	vector<BYTE> _buffer;
	bool _isOpen;
	uint32 _usedSize;
};


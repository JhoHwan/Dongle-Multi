#pragma once

class SendBuffer;
class SendBufferChunk;

class SendBufferManager
{
public:
	// 큰 버퍼에서 내가 일정 부분만큼 사용하기 위해 연다는 느낌
	shared_ptr<SendBuffer> Open(uint32 size);

private:
	shared_ptr<SendBufferChunk> Pop();
	void Push(shared_ptr<SendBufferChunk> buffer);

	static void	PushGlobal(SendBufferChunk* buffer);

private:
	mutex _lock;
	vector<shared_ptr<SendBufferChunk>> _sendBufferChunks;
};


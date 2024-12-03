#pragma once

class SendBuffer;
class SendBufferChunk;

class SendBufferManager
{
public:
	// ū ���ۿ��� ���� ���� �κи�ŭ ����ϱ� ���� ���ٴ� ����
	shared_ptr<SendBuffer> Open(uint32 size);

private:
	shared_ptr<SendBufferChunk> Pop();
	void Push(shared_ptr<SendBufferChunk> buffer);

	static void	PushGlobal(SendBufferChunk* buffer);

private:
	mutex _lock;
	vector<shared_ptr<SendBufferChunk>> _sendBufferChunks;
};


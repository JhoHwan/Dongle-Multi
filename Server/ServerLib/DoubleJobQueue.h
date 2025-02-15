#pragma once

class DoubleJobQueue
{
public:
	DoubleJobQueue();
	~DoubleJobQueue();

	void InsertJob(shared_ptr<Job> job);

	std::chrono::nanoseconds ExecuteJob(std::chrono::microseconds time);

	void SwapQueue();

private:
	std::mutex _lock;

	std::queue<shared_ptr<Job>>* _insertJobQueue;
	std::queue<shared_ptr<Job>>* _executeJobQueue;

};

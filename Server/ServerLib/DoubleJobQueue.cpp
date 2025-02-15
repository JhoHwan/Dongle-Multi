#include "pch.h"
#include "DoubleJobQueue.h"
#include <chrono>

DoubleJobQueue::DoubleJobQueue()
{
	_insertJobQueue = new queue<shared_ptr<Job>>();
	_executeJobQueue = new queue<shared_ptr<Job>>();
}

DoubleJobQueue::~DoubleJobQueue()
{
	delete _insertJobQueue;
	delete _executeJobQueue;
}

void DoubleJobQueue::InsertJob(shared_ptr<Job> job)
{
	std::lock_guard<mutex> lock(_lock);

	_insertJobQueue->push(job);
}

std::chrono::nanoseconds DoubleJobQueue::ExecuteJob(std::chrono::microseconds time)
{
	const auto startTime = std::chrono::high_resolution_clock::now();
	int32 executeJobCount = 0;
	int32 totalJobCount = _executeJobQueue->size();
	while (!_executeJobQueue->empty())
	{
		auto job = _executeJobQueue->back();
		job->Execute();

		_executeJobQueue->pop();
		++executeJobCount;
	}
	const auto curTime = std::chrono::steady_clock::now();
	const auto remainTime = time - (curTime - startTime);

	if (remainTime.count() < 0)
	{
		std::cout << "Job Time Out!" << endl;
		return 0ns;
	}

	return remainTime;
}



void DoubleJobQueue::SwapQueue()
{
	std::lock_guard<mutex> lock(_lock);

	std::swap(_insertJobQueue, _executeJobQueue);
}



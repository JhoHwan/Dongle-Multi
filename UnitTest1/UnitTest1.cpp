// stdafx.h �Ǵ� �ʿ��� ������� �����մϴ�.
#include "pch.h"
#include "CppUnitTest.h"

#include <queue>
#include <memory>
#include <mutex>
#include <thread>
#include <chrono>
#include <vector>
#include <atomic>
#include <functional>
#include <type_traits>
#include <iostream>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace std;
using namespace std::chrono;

//------------------------------------------------------------------------------
// �������ֽ� Job Ŭ���� (���ø� ������ ����)
//------------------------------------------------------------------------------

template<typename B, typename T>
using Base_Check = typename enable_if<is_base_of<T, B>::value, bool>::type;

class Job
{
    using Function = function<void()>;

public:
    Job() = delete;
    Job(const Job&) = delete;
    Job(Job&&) = default;

    // Derived Member Function
    template<typename R, typename T, typename B, typename... Args, Base_Check<T, B> = true>
    Job(R(T::* func)(Args...), B* obj, Args... args)
    {
        // static_cast<T*>(obj) �� �� ��ȯ �� ��� �Լ� ȣ��
        _func = [obj, func, args...]() { (static_cast<T*>(obj)->*func)(args...); };
    }

    // Member Function (������ ����)
    template<typename R, typename Obj, typename... Args>
    Job(R(Obj::* func)(Args...), Obj* obj, Args... args)
    {
        _func = [obj, func, args...]() { (obj->*func)(args...); };
    }

    // Member Function (shared_ptr ����)
    template<typename R, typename Obj, typename... Args>
    Job(R(Obj::* func)(Args...), shared_ptr<Obj> obj, Args... args)
    {
        _func = [obj, func, args...]() { (obj.get()->*func)(args...); };
    }

    // Static �Լ� Ȥ�� �Ϲ� �Լ� ������
    template<typename R, typename... Args>
    Job(R(*func)(Args...), Args... args)
    {
        _func = [func, args...]() { (*func)(args...); };
    }

    // Lambda �Լ�
    Job(function<void()>&& func)
        : _func(std::move(func))
    {
    }

    void Execute() const
    {
        _func();
    }
    void operator() () const
    {
        _func();
    }

private:
    Function _func;
};

//------------------------------------------------------------------------------
// DoubleJobQueue Ŭ���� (������ ���õ� �ڵ�� ����)
//------------------------------------------------------------------------------

class DoubleJobQueue
{
public:
    DoubleJobQueue();
    ~DoubleJobQueue();
    void InsertJob(shared_ptr<Job> job);
    void ExecuteJob();
    void SwapQueue();
private:
    queue<shared_ptr<Job>>* _insertJobQueue;
    queue<shared_ptr<Job>>* _executeJobQueue;
    mutex _lock;
};

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
    lock_guard<mutex> lock(_lock);
    _insertJobQueue->push(job);
}

void DoubleJobQueue::ExecuteJob()
{
    // ������ ���õ� ��� back()���� �۾��� ���� �����մϴ�.
    while (!_executeJobQueue->empty())
    {
        auto job = _executeJobQueue->back();
        job->Execute();
        _executeJobQueue->pop();
    }
}

void DoubleJobQueue::SwapQueue()
{
    lock_guard<mutex> lock(_lock);
    swap(_insertJobQueue, _executeJobQueue);
}

//------------------------------------------------------------------------------
// ���� ���� ī���� (�� �۾� ���� �� ����)
//------------------------------------------------------------------------------
atomic<int> g_executedJobCount(0);

// ���� �� ī���͸� ������Ű�� �Լ� (Job ���� �� �Լ� �����ͷ� ���)
void IncrementCounter()
{
    g_executedJobCount.fetch_add(1, memory_order_relaxed);
}

//------------------------------------------------------------------------------
// UnitTest �ڵ�
//------------------------------------------------------------------------------
namespace DoubleJobQueueTest
{
    TEST_CLASS(DoubleJobQueueTests)
    {
    public:
        TEST_METHOD(TestConcurrentInsertAndExecute)
        {
            // ī���� �ʱ�ȭ
            g_executedJobCount.store(0);

            DoubleJobQueue jobQueue;

            const int numInsertionThreads = 1;
            const int jobsPerThread = 1000;
            int swapCount = 0;
            vector<thread> insertionThreads;

            // ���� ������: �� ������� jobsPerThread ��ŭ IncrementCounter �۾��� �����Ͽ� ť�� �߰��մϴ�.
            for (int i = 0; i < numInsertionThreads; i++)
            {
                insertionThreads.emplace_back([&jobQueue, jobsPerThread]()
                    {
                        for (int j = 0; j < jobsPerThread; j++)
                        {
                            // Job ���� �� static �Լ� ������ ��� (IncrementCounter)
                            auto job = make_shared<Job>(&IncrementCounter);
                            jobQueue.InsertJob(job);

                            // �۾� ���� �ణ�� ���� (1ms)
                            this_thread::sleep_for(milliseconds(1));
                        }
                    });
            }


            // ���� ������: 33ms���� SwapQueue() �� ExecuteJob() ȣ��
            auto testDuration = seconds(3);  // �� 3�� ���� ����
            auto startTime = steady_clock::now();
            while (steady_clock::now() - startTime < testDuration)
            {
                jobQueue.SwapQueue();
                jobQueue.ExecuteJob();
                this_thread::sleep_for(milliseconds(33));
                swapCount++;
            }

            // ��� ���� �����尡 ����� ������ ���
            for (auto& t : insertionThreads)
            {
                if (t.joinable())
                    t.join();
            }

            // ���� ������ ���� �� �������� �� �ִ� �۾����� ó���ϱ� ���� ���������� Swap/Execute
            jobQueue.SwapQueue();
            jobQueue.ExecuteJob();

            // ��ü ���� �۾� ���� ����� �۾� ���� ��ġ�ϴ��� Ȯ��
            int expectedJobs = numInsertionThreads * jobsPerThread;
            Assert::AreEqual(expectedJobs, (int)g_executedJobCount.load(), L"��� �۾��� ������� �ʾҽ��ϴ�.");
        }
    };
}

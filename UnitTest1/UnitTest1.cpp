// stdafx.h 또는 필요한 헤더들을 포함합니다.
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
// 제공해주신 Job 클래스 (템플릿 생성자 포함)
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
        // static_cast<T*>(obj) 로 형 변환 후 멤버 함수 호출
        _func = [obj, func, args...]() { (static_cast<T*>(obj)->*func)(args...); };
    }

    // Member Function (포인터 인자)
    template<typename R, typename Obj, typename... Args>
    Job(R(Obj::* func)(Args...), Obj* obj, Args... args)
    {
        _func = [obj, func, args...]() { (obj->*func)(args...); };
    }

    // Member Function (shared_ptr 인자)
    template<typename R, typename Obj, typename... Args>
    Job(R(Obj::* func)(Args...), shared_ptr<Obj> obj, Args... args)
    {
        _func = [obj, func, args...]() { (obj.get()->*func)(args...); };
    }

    // Static 함수 혹은 일반 함수 포인터
    template<typename R, typename... Args>
    Job(R(*func)(Args...), Args... args)
    {
        _func = [func, args...]() { (*func)(args...); };
    }

    // Lambda 함수
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
// DoubleJobQueue 클래스 (질문에 제시된 코드와 동일)
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
    // 질문에 제시된 대로 back()에서 작업을 꺼내 실행합니다.
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
// 전역 실행 카운터 (각 작업 실행 시 증가)
//------------------------------------------------------------------------------
atomic<int> g_executedJobCount(0);

// 실행 시 카운터를 증가시키는 함수 (Job 생성 시 함수 포인터로 사용)
void IncrementCounter()
{
    g_executedJobCount.fetch_add(1, memory_order_relaxed);
}

//------------------------------------------------------------------------------
// UnitTest 코드
//------------------------------------------------------------------------------
namespace DoubleJobQueueTest
{
    TEST_CLASS(DoubleJobQueueTests)
    {
    public:
        TEST_METHOD(TestConcurrentInsertAndExecute)
        {
            // 카운터 초기화
            g_executedJobCount.store(0);

            DoubleJobQueue jobQueue;

            const int numInsertionThreads = 1;
            const int jobsPerThread = 1000;
            int swapCount = 0;
            vector<thread> insertionThreads;

            // 삽입 스레드: 각 스레드는 jobsPerThread 만큼 IncrementCounter 작업을 생성하여 큐에 추가합니다.
            for (int i = 0; i < numInsertionThreads; i++)
            {
                insertionThreads.emplace_back([&jobQueue, jobsPerThread]()
                    {
                        for (int j = 0; j < jobsPerThread; j++)
                        {
                            // Job 생성 시 static 함수 생성자 사용 (IncrementCounter)
                            auto job = make_shared<Job>(&IncrementCounter);
                            jobQueue.InsertJob(job);

                            // 작업 사이 약간의 지연 (1ms)
                            this_thread::sleep_for(milliseconds(1));
                        }
                    });
            }


            // 메인 스레드: 33ms마다 SwapQueue() 및 ExecuteJob() 호출
            auto testDuration = seconds(3);  // 총 3초 동안 실행
            auto startTime = steady_clock::now();
            while (steady_clock::now() - startTime < testDuration)
            {
                jobQueue.SwapQueue();
                jobQueue.ExecuteJob();
                this_thread::sleep_for(milliseconds(33));
                swapCount++;
            }

            // 모든 삽입 스레드가 종료될 때까지 대기
            for (auto& t : insertionThreads)
            {
                if (t.joinable())
                    t.join();
            }

            // 삽입 스레드 종료 후 남아있을 수 있는 작업들을 처리하기 위해 마지막으로 Swap/Execute
            jobQueue.SwapQueue();
            jobQueue.ExecuteJob();

            // 전체 삽입 작업 수와 실행된 작업 수가 일치하는지 확인
            int expectedJobs = numInsertionThreads * jobsPerThread;
            Assert::AreEqual(expectedJobs, (int)g_executedJobCount.load(), L"모든 작업이 실행되지 않았습니다.");
        }
    };
}

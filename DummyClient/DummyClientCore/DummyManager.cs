using DummyClientWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyClientCore
{
    public class DummyManager
    {
        private readonly int _threadCount;
        private List<Thread> _threads;
        private ManagedIOCPCore _iocpCore;
        private ManagedConnector _connector;

        private List<ManagedDummySession> _sessionList; 

        public DummyManager(int threadCount)
        {
            _threadCount = threadCount;
            _threads = new List<Thread>();
            _iocpCore = new ManagedIOCPCore();
            _connector = new ManagedConnector();
            _sessionList = new List<ManagedDummySession>();
        }

        public void Start()
        {
            for (int i = 0; i < _threadCount; i++)
            {
                Thread thread = new Thread(new ThreadStart(Dispatch));
                thread.IsBackground = true;
                _threads.Add(thread);
                thread.Start();
                Console.WriteLine($"Thread {i} Start!");
            }
        }

        public void Connect(string ip, ushort port)
        {
            Session session = new Session();

            _iocpCore.RegisterSocket(session.GetSocket());

            _connector.Connect(ip, port, session);
        }

        public void DisconnectAll()
        {
            foreach(ManagedDummySession session in _sessionList)
            {
                session.Disconnect();
            }

            _sessionList.Clear();
        }

        private void Dispatch()
        {
            while (true)
            {
                _iocpCore.Dispatch(10);
            }
        }

        public void Register(ManagedDummySession session)
        {
            _iocpCore.RegisterSocket(session.GetSocket());
        }

    }
}

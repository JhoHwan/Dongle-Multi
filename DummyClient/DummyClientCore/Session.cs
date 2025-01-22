using DummyClientWrapper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DummyClientCore
{
    public class Session : ManagedDummySession
    {
        private readonly ServerService _service;

        private bool _isExecute = false;

        public Session(ServerService service) : base() 
        {
            _service = service;
        }

        public void Connect()
        {
            CreateSocket();

            _service.RegisterIOCP(this);

            _service.Connector.Connect("127.0.0.1", 7777, this);
        }

        public async Task Execute()
        {
            _isExecute = true;

            for(int i = 0; i < 10; i++) 
            {
                Connect();

                await Task.Delay(500);

                Disconnect();

                await Task.Delay(500);
            }
        }

        public void Stop()
        {
            if (_isExecute)
            {
                _isExecute = false;
            }
        }

        public override void OnConnected()
        {
            Console.WriteLine($"Session {GetSocket()} Connected");
        }

        public override void OnDisConnected()
        {
            Console.WriteLine($"Session {GetSocket()} Disconnected");
        }

        public override void OnRecv(uint sentBytes)
        {

        }
        public override void OnSend(uint sentBytes)
        {

        }
    }
}

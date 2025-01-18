using DummyClientWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClientCore
{
    internal class Session : ManagedDummySession
    {
        public override void OnConnected()
        {
            throw new NotImplementedException();
        }

        public override void OnDisConnected()
        {
            throw new NotImplementedException();
        }

        public override void OnRecv(uint sentBytes)
        {
            throw new NotImplementedException();
        }
        public override void OnSend(uint sentBytes)
        {
            throw new NotImplementedException();
        }
    }
}

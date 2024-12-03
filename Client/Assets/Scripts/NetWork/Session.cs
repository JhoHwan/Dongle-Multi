using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace NetWork
{
    public class ServerSession : PacketSession
    {
        private ClientPacketHandler _packetHandler;

        public ServerSession() :base() 
        {
            _packetHandler = new ClientPacketHandler(this);
        }


        public override void OnConnected(EndPoint endPoint)
        {
            Debug.Log($"Connect to Server {endPoint}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Debug.Log($"DisConnect to Server {endPoint}");

        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            _packetHandler.ProcessPacket(buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            Debug.Log($"Recv : {numOfBytes}");
        }
    }


    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 6;

        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            int packetCount = 0;

            while (true)
            {
                if (buffer.Count < HeaderSize)
                    break;

                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
                if (buffer.Count < dataSize)
                    break;

                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        private Socket _socket;
        int _disconnected = 0;

        private RecvBuffer _recvBuffer = new RecvBuffer(65535);

        private readonly object _lock = new object();
        private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public bool IsConnected() { return _socket != null; } 

        private void Clear()
        {
            lock (_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(List<ArraySegment<byte>> sendBufferList)
        {
            if (sendBufferList.Count == 0)
            {
                return;
            }

            lock (_lock)
            {
                foreach (var sendBuff in sendBufferList)
                {
                    _sendQueue.Enqueue(sendBuff);
                }

                if (_pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            Clear();
        }

        private void RegisterSend()
        {
            if (_disconnected == 1)
                return;

            while (_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }
            _sendArgs.BufferList = _pendingList;

            try
            {
                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false)
                    OnSendCompleted(null, _sendArgs);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        private void RegisterRecv()
        {
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false)
                    OnRecvCompleted(null, _recvArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterRecv Failed {e}");
            }
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    if (_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                Disconnect();
            }
        }
    }
}
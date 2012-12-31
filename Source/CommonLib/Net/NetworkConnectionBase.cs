using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Net
{
    public abstract class NetworkConnectionBase
    {
        private static readonly log4net.ILog _Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const int ReceiveBufferSize = 16 * 1024;

        private readonly byte[] _ReceiveBuffer;
        private int _ReceivePosition;
        private int _ReceiveSize;

        private readonly Socket _Client;
        public Socket Client
        {
            get { return _Client; }
        }

        private IPacketLenghtDatabase _PacketDatabase;
        public IPacketLenghtDatabase PacketDatabase
        {
            get { return _PacketDatabase; }
            set
            {
                _PacketDatabase = value;

                if (_ReceiveSize - _ReceivePosition > 2)
                    ProcessPackets();
            }
        }
        public bool Connected { get; private set; }

        private event EventHandler Disconnected;
        private event EventHandler<PacketEventArgs> PacketReceived;

        protected NetworkConnectionBase(Socket client)
        {
            _Client = client;

            _ReceiveBuffer = new byte[ReceiveBufferSize];
            _ReceivePosition = 0;
            _ReceiveSize = 0;

            Connected = client.Connected;
        }

        public void Start()
        {
            if (Connected && _ReceiveBuffer.Length - _ReceiveSize > 0)
            {
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();

                e.Completed += AsyncReceived;
                e.SetBuffer(_ReceiveBuffer, _ReceiveSize, _ReceiveBuffer.Length - _ReceiveSize);

                if (!_Client.ReceiveAsync(e))
                    Disconnect();
            }
            else if (Connected)
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (!Connected)
                return;

            if (_Client.Connected)
                _Client.Disconnect(false);

            OnDisconnect();

            Connected = false;
        }

        private void AsyncReceived(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                _Log.InfoFormat("Disconnecting socket({0}) with error: {1}", _Client.RemoteEndPoint, e.SocketError);
                Disconnect();
                return;
            }

            if (e.BytesTransferred == 0)
            {
                _Log.InfoFormat("Client closed connection from {0}.", _Client.RemoteEndPoint);
                Disconnect();
                return;
            }

            _ReceiveSize += e.BytesTransferred;

            ProcessPackets();
            Start();
        }

        private void ProcessPackets()
        {
            if (_PacketDatabase == null || !Connected)
                return;

            while (_ReceiveSize - _ReceivePosition >= 2)
            {
                ushort packetID = BitConverter.ToUInt16(_ReceiveBuffer, _ReceivePosition);
                int size = _PacketDatabase.GetPacketLenght(packetID);
                bool isFixed = true;

                if (size == -1)
                {
                    if (_ReceiveSize - _ReceivePosition >= 4)
                        size = BitConverter.ToUInt16(_ReceiveBuffer, _ReceivePosition + 2);
                    else
                        break;

                    isFixed = false;
                }

                if (size == 0)
                {
                    _Log.WarnFormat("Closing connection from {0}: Invalid packet received(0x{1:x}).", _Client.RemoteEndPoint, packetID);
                    Disconnect();
                    return;
                }

                int offset = isFixed ? 2 : 4;
                OnPacketReceived(packetID, size, new BinaryBuffer(_ReceiveBuffer, _ReceivePosition + offset, size - offset));

                _ReceivePosition += size;
            }

            if (_ReceiveSize > 0)
            {
                if (_ReceiveSize == _ReceivePosition)
                {
                    _ReceiveSize = 0;
                    _ReceivePosition = 0;
                }
                else
                {
                    Buffer.BlockCopy(_ReceiveBuffer, _ReceivePosition, _ReceiveBuffer, 0, _ReceiveSize - _ReceivePosition);
                    _ReceiveSize -= _ReceivePosition;
                    _ReceivePosition = 0;
                }
            }
        }

        public void Send(byte[] data)
        {
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();

            e.SetBuffer(data, 0, data.Length);

            _Client.SendAsync(e);
        }

        public void Send(BinaryBuffer data)
        {
            Send(data.GetData());
        }

        protected virtual void OnDisconnect()
        {
            EventHandler d = Disconnected;
            if (d != null)
                d(this, null);
        }

        protected virtual void OnPacketReceived(ushort packetID, int packetSize, BinaryBuffer packetData)
        {
            EventHandler<PacketEventArgs> pr = PacketReceived;
            if (pr != null)
                pr(this, new PacketEventArgs(packetID, packetSize, packetData));
        }
    }
}

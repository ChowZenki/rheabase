using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Net
{
    public class ConnectionListener
    {
        private INetworkConnectionFactory _Factory;
        private TcpListener _Listener;

        public IPAddress Address { get; private set; }
        public int Port { get; private set; }

        public ConnectionListener(INetworkConnectionFactory clientFactory)
        {
            _Factory = clientFactory;
        }

        public void Bind(IPAddress address, int port)
        {
            Address = address;
            Port = port;
            _Listener = new TcpListener(address, port);
        }

        public void Listen()
        {
            _Listener.Start();
            Start();
        }

        public void Stop()
        {
            _Listener.Stop();
        }

        private void Start()
        {
            _Listener.AcceptSocketAsync().ContinueWith((Task<Socket> t) =>
                {
                    Socket sock = t.Result;

                    if (CheckConnection(sock))
                        _Factory.Create(sock).Start();

                    Start();
                });
        }

        private bool CheckConnection(Socket sock)
        {
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CommonLib;

namespace AccountServer
{
    public class CharServerService : ICharServerService
    {
        private bool _Valid;

        public int OnlinePlayers { get; set; }

        public string Name { get; private set; }
        public IPAddress Address { get; private set; }
        public int Port { get; private set; }

        public bool RegisterServer(string name, IPAddress publicAddress, int publicPort)
        {
            OperationContext.Current.Channel.Closed += ChannelOnClosed;
            OperationContext.Current.Channel.Faulted += ChannelOnFaulted;

            Name = name;
            Address = publicAddress;
            Port = publicPort;

            return _Valid = AccountServer.Instance.RegisterCharServer(this);
        }

        private void Disconnect(bool isGracefully)
        {
            if (_Valid)
            {
                AccountServer.Instance.UnregisterCharServer(this);
                _Valid = false;
            }
        }

        private void ChannelOnFaulted(object sender, EventArgs eventArgs)
        {
            Disconnect(false);
        }

        private void ChannelOnClosed(object sender, EventArgs eventArgs)
        {
            Disconnect(true);
        }

        public void Disconnect()
        {
            Disconnect(true);
        }
    }
}

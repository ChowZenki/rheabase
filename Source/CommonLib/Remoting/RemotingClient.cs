using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Remoting
{
    public class RemotingClient
    {
        private readonly Dictionary<string, IRemotedObject<Object>> _Clients;

        public Uri BaseUri { get; private set; }

        public RemotingClient(string baseUri)
        {
            _Clients = new Dictionary<string, IRemotedObject<Object>>();
            BaseUri = new Uri(baseUri);
        }

        public RemotedObject<T> Get<T>(string name) where T : class
        {
            IRemotedObject<Object> cache;

            if (_Clients.TryGetValue(name, out cache))
            {
                if (cache.RemotedType == typeof(T))
                    return (RemotedObject<T>)cache;

                return null;
            }

            RemotedObject<T> client = new RemotedObject<T>(new NetTcpBinding(), new EndpointAddress(new Uri(BaseUri, name)));
            client.Open();

            if (client.State != CommunicationState.Opened)
                return null;

            _Clients.Add(name, client);

            return client;
        }

        public void Disconnect()
        {
            foreach (KeyValuePair<string, IRemotedObject<Object>> remoted in _Clients)
                remoted.Value.Close();
        }
    }
}

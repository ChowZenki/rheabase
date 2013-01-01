using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Remoting
{
    public class RemotingServer
    {
        private readonly List<ServiceHost> _Services;

        public Uri BaseUri { get; private set; }
        public bool Running { get; private set; }

        public RemotingServer(string baseUri)
        {
            _Services = new List<ServiceHost>();
            BaseUri = new Uri(baseUri);
        }

        public void ExposeSingleton(string name, Type iface, object singleton)
        {
            Uri address = new Uri(BaseUri, name);
            ServiceHost host = new ServiceHost(singleton);

            host.AddServiceEndpoint(iface, new NetTcpBinding(), address);
            _Services.Add(host);

            if (Running)
                host.Open();
        }

        public void ExposeType<T>(string name, Type iface)
        {
            Uri address = new Uri(BaseUri, name);
            ServiceHost host = new ServiceHost(typeof(T));

            host.AddServiceEndpoint(iface, new NetTcpBinding(), address);
            _Services.Add(host);
            
            if (Running)
                host.Open();
        }

        public void Start()
        {
            if (Running)
                return;

            foreach (ServiceHost host in _Services)
                host.Open();

            Running = true;
        }

        public void Stop()
        {
            if (!Running)
                return;

            foreach (ServiceHost host in _Services)
                host.Close();

            Running = false;
        }
    }
}

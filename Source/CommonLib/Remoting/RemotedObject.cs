using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Remoting
{
    public class RemotedObject<T> : ClientBase<T>, IRemotedObject<T> where T : class
    {
        public event EventHandler Disconnected;

        public RemotedObject(Binding binding, EndpointAddress address)
            : base(binding, address)
        {
            ChannelFactory.Faulted += ChannelFactory_Faulted;
        }

        private void ChannelFactory_Faulted(object sender, EventArgs e)
        {
            EventHandler d = Disconnected;

            if (d != null)
                d(sender, e);
        }

        public Type RemotedType
        {
            get { return typeof(T); }
        }

        public T Proxy
        {
            get { return Channel; }
        }
    }
}

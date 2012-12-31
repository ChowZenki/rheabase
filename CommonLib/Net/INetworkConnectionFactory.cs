using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Net
{
    public interface INetworkConnectionFactory
    {
        NetworkConnectionBase Create(Socket client);
    }
}

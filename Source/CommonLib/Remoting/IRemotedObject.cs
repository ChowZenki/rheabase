using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Remoting
{
    public interface IRemotedObject<out T>
    {
        event EventHandler Disconnected;

        Type RemotedType { get; }
        T Proxy { get; }

        void Close();
    }
}

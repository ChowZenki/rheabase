using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Net
{
    public interface IPacketLenghtDatabase
    {
        int GetPacketLenght(ushort packetID);
    }
}

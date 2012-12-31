using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonLib.Net
{
    public class PacketEventArgs : EventArgs
    {
        public ushort PacketID { get; private set; }
        public int PacketSize { get; private set; }
        public BinaryBuffer PacketData { get; private set; }

        public PacketEventArgs(ushort packetID, int packetSize, BinaryBuffer packetData)
        {
            PacketID = packetID;
            PacketSize = packetSize;
            PacketData = packetData;
        }
    }
}

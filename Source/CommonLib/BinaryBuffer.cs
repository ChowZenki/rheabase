using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class BinaryBuffer
    {
        private byte[] _Buffer;
        private int _Offset;
        private int _Lenght;

        public BinaryBuffer(int size)
            : this(new byte[size], 0, size)
        {

        }

        public BinaryBuffer(byte[] buffer, int offset, int size)
        {
            _Buffer = buffer;
            _Offset = offset;
            _Lenght = size;
        }

        public byte ReadByte(int position)
        {
            if (position >= _Lenght)
                throw new IndexOutOfRangeException();

            return _Buffer[_Offset + position];
        }

        public short ReadInt16(int position)
        {
            if (position + 2 > _Lenght)
                throw new IndexOutOfRangeException();

            return BitConverter.ToInt16(_Buffer, _Offset + position);
        }

        public int ReadInt32(int position)
        {
            if (position + 4 > _Lenght)
                throw new IndexOutOfRangeException();

            return BitConverter.ToInt32(_Buffer, _Offset + position);
        }

        public ushort ReadUInt16(int position)
        {
            if (position + 2 > _Lenght)
                throw new IndexOutOfRangeException();

            return BitConverter.ToUInt16(_Buffer, _Offset + position);
        }

        public uint ReadUInt32(int position)
        {
            if (position + 4 > _Lenght)
                throw new IndexOutOfRangeException();

            return BitConverter.ToUInt32(_Buffer, _Offset + position);
        }

        public string ReadCString(int position, int size)
        {
            StringBuilder sb = new StringBuilder(size);

            if (position + size > _Lenght)
                throw new IndexOutOfRangeException();

            int offset = _Offset + position;
            for (int i = 0; i < size; i++)
            {
                if (_Buffer[offset + i] == 0)
                    break;

                sb.Insert(i, (char)_Buffer[offset + i]);
            }

            return sb.ToString();
        }

        public void Read(int position, byte[] buffer, int offset, int size)
        {
            Buffer.BlockCopy(_Buffer, _Offset + position, buffer, offset, size);
        }

        public byte[] ReadBytes(int position,  int size)
        {
            byte[] data = new byte[size];

            Buffer.BlockCopy(_Buffer, _Offset + position, data, 0, size);

            return data;
        }

        public void WriteByte(int position, byte value)
        {
            if (position >= _Lenght)
                throw new IndexOutOfRangeException();

            _Buffer[_Offset + position] = value;
        }

        public unsafe void WriteInt16(int position, short value)
        {
            if (position + 2 > _Lenght)
                throw new IndexOutOfRangeException();

            fixed (byte* ptr = &_Buffer[_Offset + position])
                *(short*)ptr = value;
        }

        public unsafe void WriteInt32(int position, int value)
        {
            if (position + 4 > _Lenght)
                throw new IndexOutOfRangeException();

            fixed (byte* ptr = &_Buffer[_Offset + position])
                *(int*)ptr = value;
        }

        public unsafe void WriteUInt16(int position, ushort value)
        {
            if (position + 2 > _Lenght)
                throw new IndexOutOfRangeException();

            fixed (byte* ptr = &_Buffer[_Offset + position])
                *(ushort*)ptr = value;
        }

        public unsafe void WriteUInt32(int position, uint value)
        {
            if (position + 4 > _Lenght)
                throw new IndexOutOfRangeException();

            fixed (byte* ptr = &_Buffer[_Offset + position])
                *(uint*)ptr = value;
        }

        public void WriteCString(int position, string str, int size)
        {
            int i;

            if (position + size > _Lenght)
                throw new IndexOutOfRangeException();

            int offset = _Offset + position;
            for (i = 0; i < str.Length && i < size; i++)
                _Buffer[offset + i] = (byte)str[i];

            if (i < size)
                _Buffer[offset + i] = 0;
        }

        public void Write(int position, byte[] buffer, int offset, int size)
        {
            if (position + buffer.Length > _Lenght)
                throw new IndexOutOfRangeException();

            Buffer.BlockCopy(buffer, offset, _Buffer, _Offset + position, size);
        }

        public void WriteBytes(int position, byte[] buffer)
        {
            Write(position, buffer, 0, buffer.Length);
        }

        public byte[] GetData()
        {
            return _Buffer;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonLib.Net
{
    public class FixedMemoryStream : Stream
    {
        private byte[] _Buffer;
        private int _Offset;

        public FixedMemoryStream(byte[] buffer, int offset, int size)
        {
            if (offset + size > buffer.Length)
                throw new IndexOutOfRangeException("buffer");

            _Buffer = buffer;
            _Offset = offset;
            _Length = size;
            _Position = 0;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        private readonly int _Length;
        public override long Length
        {
            get { return _Length; }
        }

        private int _Position;
        public override long Position
        {
            get { return _Position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = Math.Min(count, _Length - _Position);

            Buffer.BlockCopy(_Buffer, _Offset + _Position, buffer, offset, read);

            _Position += read;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            int oldPosition = _Position;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    _Position = (int)offset;
                    break;
                case SeekOrigin.Current:
                    _Position += (int)offset;
                    break;
                case SeekOrigin.End:
                    _Position = _Length + (int)offset;
                    break;
            }

            if (_Position >= _Length)
            {
                _Position = oldPosition;
                throw new IOException("");
            }

            return _Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}

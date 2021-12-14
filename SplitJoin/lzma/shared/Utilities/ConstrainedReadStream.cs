namespace ManagedLzma
{
    /// <remarks>
    /// This class is internal, the caller can be trusted to pass valid paramaters, so we only check them in debug mode.
    /// </remarks>
    internal sealed class ConstrainedReadStream : Stream
    {
        private Stream mStream;
        private long mOrigin;
        private long mCursor;
        private long mLength;

        public ConstrainedReadStream(Stream stream, long origin, long length)
        {
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(stream != null);
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(stream.CanRead);
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(stream.CanSeek);
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(0 <= origin && origin <= stream.Length);
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(0 <= length && length <= stream.Length - origin);

            mStream = stream;
            mOrigin = origin;
            mLength = length;

            stream.Position = origin;
        }

        protected override void Dispose(bool disposing)
        {
            // We don't own the stream, so don't dispose it. We just clear the reference to detect accidental calls.
            mStream = null;
            base.Dispose(disposing);
        }

        public override bool CanRead => mStream.CanRead;
        public override bool CanSeek => mStream.CanSeek;
        public override bool CanWrite => false;
        public override long Length => mLength;

        public override long Position
        {
            get { return mCursor; }
            set
            {
                ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(0 <= value && value <= mLength);
                mCursor = value;
                mStream.Position = mOrigin + value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: return this.Position = offset;
                case SeekOrigin.Current: return this.Position = this.Position + offset;
                case SeekOrigin.End: return this.Position = mLength + offset;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override int Read(byte[] buffer, int offset, int length)
        {
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(buffer != null);
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(0 <= offset && offset <= buffer.Length);
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(0 <= length && length <= buffer.Length - offset);

            var remaining = mLength - mCursor;
            if (length > remaining)
                length = (int)remaining;

            var available = mStream.Read(buffer, offset, length);
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(0 <= available && available <= length);
            mCursor += available;
            return available;
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        public override void Flush()
        {
            throw new InvalidOperationException();
        }
    }
}

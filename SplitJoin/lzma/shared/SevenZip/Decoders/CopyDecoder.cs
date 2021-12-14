using System.Collections.Immutable;

namespace ManagedLzma.SevenZip.Reader
{
    internal sealed class CopyArchiveDecoder : DecoderNode
    {
        private sealed class OutputStream : ReaderNode
        {
            private CopyArchiveDecoder mOwner;
            public OutputStream(CopyArchiveDecoder owner) { mOwner = owner; }
            public override void Dispose() { mOwner = null; }
            public override void Skip(int count) => mOwner.Skip(count);
            public override int Read(byte[] buffer, int offset, int count) => mOwner.Read(buffer, offset, count);
        }

        private ReaderNode mInput;
        private OutputStream mOutput;
        private long mLength;
        private long mPosition;

        public CopyArchiveDecoder(ImmutableArray<byte> settings, long length)
        {
            if (settings.IsDefault)
                throw new ArgumentNullException("settings");

            if (!settings.IsEmpty)
                throw new InvalidDataException();

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            mOutput = new OutputStream(this);
            mLength = length;
        }

        public override void Dispose()
        {
            mOutput.Dispose();
            mInput?.Dispose();
        }

        public override void SetInputStream(int index, ReaderNode stream, long length)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException("index");

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (mInput != null)
                throw new InvalidOperationException();

            mInput = stream;
        }

        public override ReaderNode GetOutputStream(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException("index");

            return mOutput;
        }

        private void Skip(int count)
        {
            mInput.Skip(count);
        }

        private int Read(byte[] buffer, int offset, int count)
        {
            var result = mInput.Read(buffer, offset, count);
            mPosition += result;
            return result;
        }
    }
}

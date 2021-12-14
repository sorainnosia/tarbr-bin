﻿namespace ManagedLzma.LZMA
{
    public sealed class AsyncEncoder : IDisposable
    {
        #region Variables

        // immutable
        private readonly object mSyncObject;

        // multithreaded access (under lock)
        private Task mEncoderTask;
        private Task mDisposeTask;
        private bool mRunning;

        // owned by encoder task
        private Master.LZMA.CLzmaEnc mEncoder;

        #endregion

        #region Implementation

        public AsyncEncoder(EncoderSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            mSyncObject = new object();

            mEncoder = new Master.LZMA.CLzmaEnc();
            mEncoder.LzmaEnc_SetProps(settings.GetInternalSettings());
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        public Task DisposeAsync()
        {
            // We need to ensure that cleanup only happens once, so we need to remember that we started it.
            // We also need to make sure that the returned task completes *after* everything has been disposed.
            // Both can be covered by keeping track of the disposal via a Task object.
            //
            lock (mSyncObject)
            {
                if (mDisposeTask == null)
                {
                    if (mRunning)
                    {
                        mDisposeTask = mEncoderTask.ContinueWith(new Action<Task>(delegate
                        {
                            lock (mSyncObject)
                                DisposeInternal();
                        }));

                        Monitor.PulseAll(mSyncObject);
                    }
                    else
                    {
                        DisposeInternal();
                        mDisposeTask = Utilities.CompletedTask;
                    }
                }

                return mDisposeTask;
            }
        }

        private void DisposeInternal()
        {
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(Monitor.IsEntered(mSyncObject));
            ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(!mRunning);

            // mDisposeTask may not be set yet if we complete mEncoderTask from another thread.
            // However even if mDisposeTask is not set we can be sure that the encoder is not running.

            mEncoder.LzmaEnc_Destroy(Master.LZMA.ISzAlloc.SmallAlloc, Master.LZMA.ISzAlloc.BigAlloc);
        }

        public Task EncodeAsync(IStreamReader input, IStreamWriter output, CancellationToken ct = default(CancellationToken))
        {
            lock (mSyncObject)
            {
                if (mDisposeTask != null)
                    throw new OperationCanceledException();

                // TODO: make this wait async as well
                while (mRunning)
                {
                    Monitor.Wait(mSyncObject);

                    if (mDisposeTask != null)
                        throw new OperationCanceledException();
                }

                mRunning = true;
            }

            var task = Task.Run(async delegate
            {
                var res = mEncoder.LzmaEnc_Encode(new AsyncOutputProvider(output), new AsyncInputProvider(input), null, Master.LZMA.ISzAlloc.SmallAlloc, Master.LZMA.ISzAlloc.BigAlloc);
                if (res != Master.LZMA.SZ_OK)
                    throw new InvalidOperationException();

                await output.CompleteAsync().ConfigureAwait(false);
            }, ct);

            mEncoderTask = task.ContinueWith(delegate
            {
                lock (mSyncObject)
                {
                    ManagedLzma.LZMA.Master.SevenZip.Utils.Assert(mRunning);
                    mRunning = false;
                    Monitor.PulseAll(mSyncObject);
                }
            }, CancellationToken.None, TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);

            return task;
        }

        #endregion
    }
}

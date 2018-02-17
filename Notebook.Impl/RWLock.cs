using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Notebook.Impl
{
    public class RWLock
    {
        enum LockState
        {
            Locking,
            Locked,
            Unlockring,
            Unlocked
        }

        abstract class Lock : IDisposable
        {
            public LockState State { get; private set; }

            protected readonly RWLock _lock;
            protected readonly int _threadId;

            public Lock(RWLock @lock)
            {
                _threadId = Thread.CurrentThread.ManagedThreadId;
                _lock = @lock;
                this.State = LockState.Locking;
            }

            protected void SetLocked()
            {
                this.State = LockState.Locked;
            }

            protected void SetUnlocking()
            {
                this.State = LockState.Unlockring;
            }

            public virtual void Dispose()
            {
                this.State = LockState.Unlocked;
            }
        }

        class ReaderLock : Lock
        {
            public ReaderLock(RWLock @lock)
                : base(@lock)
            {
                _lock.AcquireReaderLock();
                this.SetLocked();
            }

            public override void Dispose()
            {
                this.SetUnlocking();
                _lock.ReleaseReaderLock(this);
                base.Dispose();
            }
        }

        class WriterLock : Lock
        {
            public WriterLock(RWLock @lock)
                : base(@lock)
            {
                _lock.AcquireWriterLock();
                this.SetLocked();
            }

            public override void Dispose()
            {
                this.SetUnlocking();
                _lock.ReleaseWriterLock(this);
                base.Dispose();
            }
        }

        ReaderWriterLock _lock = new ReaderWriterLock();
        List<Lock> _lockers = new List<Lock>();

        public RWLock()
        {
        }

        private void AcquireReaderLock()
        {
            _lock.AcquireReaderLock(Timeout.Infinite);
        }

        private void ReleaseReaderLock(ReaderLock locker)
        {
            _lock.ReleaseReaderLock();
            lock (_lockers) _lockers.Remove(locker);
        }

        private void AcquireWriterLock()
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
        }

        private void ReleaseWriterLock(WriterLock locker)
        {
            _lock.ReleaseWriterLock();
            lock (_lockers) _lockers.Remove(locker);
        }

        public IDisposable Write()
        {
            var locker = new WriterLock(this);
            lock (_lockers) _lockers.Add(locker);
            return locker;
        }

        public IDisposable Read()
        {
            var locker = new ReaderLock(this);
            lock (_lockers) _lockers.Add(locker);
            return locker;
        }
    }
}

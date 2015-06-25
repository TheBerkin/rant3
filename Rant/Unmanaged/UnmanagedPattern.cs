using System;
using System.Runtime.InteropServices;

namespace Rant.Unmanaged
{
    internal class UnmanagedPattern : IDisposable
    {
        private GCHandle _handle;
        private bool _disposed;
        public RantPattern Pattern { get; }

        public UnmanagedPattern(RantPattern pattern)
        {
            Pattern = pattern;
            _handle = GCHandle.Alloc(this);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _handle.Free();
            _disposed = true;
        }
    }
}
using System;
using System.Runtime.InteropServices;

namespace Rant.Unmanaged
{
    internal class UnmanagedOutput : IDisposable
    {
        private GCHandle _handle;
        private bool _disposed;
        public RantOutput Output { get; }

        public UnmanagedOutput(RantOutput output)
        {
            Output = output;
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
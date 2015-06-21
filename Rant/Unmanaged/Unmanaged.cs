using System;
using System.Runtime.InteropServices;

namespace Rant.Unmanaged
{
    internal static class Unmanaged
    {
        public static Unmanaged<T> Make<T>(T value) => new Unmanaged<T>(value);
    }

    internal sealed class Unmanaged<T> : IDisposable
    {
        private GCHandle _handle;
        private bool _disposed;

        public T Value { get; }

        public Unmanaged(T value)
        {
            Value = value;
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
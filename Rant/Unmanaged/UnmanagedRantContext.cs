#if !UNITY

using System;
using System.Runtime.InteropServices;

namespace Rant.Unmanaged
{
    internal class UnmanagedRantContext : IDisposable
    {
        private GCHandle _handle;
        private bool _disposed;

        public RantEngine Rant { get; set; }

        public string LastErrorMessage { get; private set; } = "";

        public ErrorCode LastErrorCode { get; private set; } = ErrorCode.OK;

        public UnmanagedRantContext()
        {
            _handle = GCHandle.Alloc(this);
        }

        public ErrorCode Run(Action func)
        {
            try
            {
                func();
                LastErrorMessage = String.Empty;
                LastErrorCode = ErrorCode.OK;
            }
            catch (RantRuntimeException ex)
            {
                LastErrorMessage = ex.Message;
                LastErrorCode = ErrorCode.RuntimeError;
            }
            catch (RantCompilerException ex)
            {
                LastErrorMessage = ex.Message;
                LastErrorCode = ErrorCode.CompileError;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                LastErrorCode = ErrorCode.OtherError;
            }
            return LastErrorCode;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _handle.Free();
            _disposed = true;
        }
    }
}

#endif
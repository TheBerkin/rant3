using System;

namespace Rant.Unmanaged
{
    internal class UnmanagedRantContext
    {
        public RantEngine Rant { get; set; }

        public string LastErrorMessage { get; private set; } = "";

        public ErrorCode LastErrorCode { get; private set; } = ErrorCode.OK;

        public UnmanagedRantContext()
        {
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
    }
}
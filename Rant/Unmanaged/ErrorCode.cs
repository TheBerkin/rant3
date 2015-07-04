#if !UNITY

namespace Rant.Unmanaged
{
    internal enum ErrorCode
    {
        OK = 0,
        CompileError = 1,
        RuntimeError = 2,
        OtherError = 3
    }
}

#endif
using System.Runtime.InteropServices;

namespace Rant.Unmanaged
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PatternOptions
    {
        public int CharLimit;
        public double Timeout;
    }
}
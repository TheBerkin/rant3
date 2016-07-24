namespace Rant.Core.IO
{
    internal static class IOExtensions
    {
        public static bool GetFlag(this byte field, int pos)
        {
            return ((field >> pos) & 0x1) == 1;
        }

        public static bool GetFlag(this short field, int pos)
        {
            return ((field >> pos) & 0x1) == 1;
        }

        public static bool GetFlag(this int field, int pos)
        {
            return ((field >> pos) & 0x1) == 1;
        }

        public static bool GetFlag(this long field, int pos)
        {
            return ((field >> pos) & 0x1) == 1;
        }
    }
}

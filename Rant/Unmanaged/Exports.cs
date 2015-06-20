using System;
using System.Runtime.InteropServices;

using RGiesecke.DllExport;

namespace Rant.Unmanaged
{
    internal static class Exports
    {
        [DllExport("RantCreateContext", CallingConvention.Cdecl)]
        public static UnmanagedRantContext CreateContext() => new UnmanagedRantContext();

        [DllExport("RantGetLastError", CallingConvention.Cdecl)]
        public static ErrorCode GetLastError(UnmanagedRantContext context) => context.LastErrorCode;

        [DllExport("RantGetLastErrorMessage", CallingConvention.Cdecl)]
        public static string GetLastErrorMessage(UnmanagedRantContext context) => context.LastErrorMessage;

        [DllExport("RantLoadEngine", CallingConvention.Cdecl)]
        public static ErrorCode LoadEngine(UnmanagedRantContext context, string dictionaryPath) => context.Run(() => context.Rant = new RantEngine(dictionaryPath));

        [DllExport("RantCompilePatternString", CallingConvention.Cdecl)]
        public static ErrorCode CompilePatternString(UnmanagedRantContext context, string patternString, out RantPattern patternCompiled)
        {
            RantPattern pattern = null;
            context.Run(() => pattern = RantPattern.FromString(patternString));
            patternCompiled = pattern;
            return context.LastErrorCode;
        }

        [DllExport("RantCompilePatternFile", CallingConvention.Cdecl)]
        public static ErrorCode CompilePatternFile(UnmanagedRantContext context, string patternPath, out RantPattern patternCompiled)
        {
            RantPattern pattern = null;
            context.Run(() => pattern = RantPattern.FromFile(patternPath));
            patternCompiled = pattern;
            return context.LastErrorCode;
        }

        [DllExport("RantRunPattern", CallingConvention.Cdecl)]
        public static ErrorCode RunPattern(UnmanagedRantContext context, RantPattern pattern, PatternOptions options, out RantOutput output)
        {
            RantOutput o = null;
            context.Run(() => o = context.Rant.Do(pattern, options.CharLimit, options.Timeout));
            output = o;
            return context.LastErrorCode;
        }

        [DllExport("RantRunPatternSeed", CallingConvention.Cdecl)]
        public static ErrorCode RunPatternSeed(UnmanagedRantContext context, RantPattern pattern, PatternOptions options, long seed, out RantOutput output)
        {
            RantOutput o = null;
            context.Run(() => o = context.Rant.Do(pattern, seed, options.CharLimit, options.Timeout));
            output = o;
            return context.LastErrorCode;
        }

        [DllExport("RantGetMainValue", CallingConvention.Cdecl)]
        public static string GetMainValue(RantOutput output) => output?.Main;

        [DllExport("RantLoadPackage", CallingConvention.Cdecl)]
        public static ErrorCode LoadPackage(UnmanagedRantContext context, string packagePath) => context.Run(() => context.Rant.LoadPackage(packagePath));
    }
}
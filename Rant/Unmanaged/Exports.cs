#if !UNITY

using System;
using System.Linq;
using System.Runtime.InteropServices;

using RGiesecke.DllExport;

namespace Rant.Unmanaged
{
    internal static class Exports
    {
        [DllExport("RantCreateContext", CallingConvention.Cdecl)]
        public static UnmanagedRantContext CreateContext() => new UnmanagedRantContext();

        [DllExport("RantIsEngineLoaded", CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static bool IsEngineLoaded(UnmanagedRantContext context) => context.Rant != null;

        [DllExport("RantReleaseContext", CallingConvention.Cdecl)]
        public static void ReleaseContext(UnmanagedRantContext context) => context.Dispose();

        [DllExport("RantReleasePattern", CallingConvention.Cdecl)]
        public static void ReleasePattern(UnmanagedPattern pattern) => pattern.Dispose();

        [DllExport("RantReleaseOutput", CallingConvention.Cdecl)]
        public static void ReleaseOutput(UnmanagedOutput output) => output.Dispose();

        [DllExport("RantGetLastError", CallingConvention.Cdecl)]
        public static ErrorCode GetLastError(UnmanagedRantContext context) => context.LastErrorCode;

        [DllExport("RantGetLastErrorMessage", CallingConvention.Cdecl)]
        public static string GetLastErrorMessage(UnmanagedRantContext context) => context.LastErrorMessage;

        [DllExport("RantLoadEngine", CallingConvention.Cdecl)]
        public static ErrorCode LoadEngine(UnmanagedRantContext context, string dictionaryPath) => context.Run(() => context.Rant = new RantEngine(dictionaryPath));

        [DllExport("RantCompilePatternString", CallingConvention.Cdecl)]
        public static ErrorCode CompilePatternString(UnmanagedRantContext context, string patternString, out UnmanagedPattern patternCompiled)
        {
            RantPattern pattern = null;
            context.Run(() => pattern = RantPattern.FromString(patternString));
            patternCompiled = new UnmanagedPattern(pattern);
            return context.LastErrorCode;
        }

        [DllExport("RantCompilePatternFile", CallingConvention.Cdecl)]
        public static ErrorCode CompilePatternFile(UnmanagedRantContext context, string patternPath, out UnmanagedPattern patternCompiled)
        {
            RantPattern pattern = null;
            context.Run(() => pattern = RantPattern.FromFile(patternPath));
            patternCompiled = new UnmanagedPattern(pattern);
            return context.LastErrorCode;
        }

        [DllExport("RantRunPattern", CallingConvention.Cdecl)]
        public static ErrorCode RunPattern(UnmanagedRantContext context, UnmanagedPattern pattern, PatternOptions options, out UnmanagedOutput output)
        {
            RantOutput o = null;
            context.Run(() => o = context.Rant.Do(pattern.Pattern, options.CharLimit, options.Timeout));
            output = new UnmanagedOutput(o);
            return context.LastErrorCode;
        }

        [DllExport("RantRunPatternSeed", CallingConvention.Cdecl)]
        public static ErrorCode RunPatternSeed(UnmanagedRantContext context, UnmanagedPattern pattern, PatternOptions options, long seed, out UnmanagedOutput output)
        {
            RantOutput o = null;
            context.Run(() => o = context.Rant.Do(pattern.Pattern, seed, options.CharLimit, options.Timeout));
            output = new UnmanagedOutput(o);
            return context.LastErrorCode;
        }

        [DllExport("RantGetMainValue", CallingConvention.Cdecl)]
        public static string GetMainValue(UnmanagedOutput output) => output?.Output?.Main;

        [DllExport("RantGetOutputChannelNames", CallingConvention.Cdecl)]
        public static string[] GetOutputChannelNames(UnmanagedOutput output, out int count)
        {
            var names = output.Output.Select(e => e.Name).ToArray();
            count = names.Length;
            return names;
        }

        [DllExport("RantGetOutputValue", CallingConvention.Cdecl)]
        public static string GetOutputValue(UnmanagedOutput output, string channelName) => output?.Output?[channelName];

        [DllExport("RantLoadPackage", CallingConvention.Cdecl)]
        public static ErrorCode LoadPackage(UnmanagedRantContext context, string packagePath) => context.Run(() => context.Rant.LoadPackage(packagePath));
    }
}

#endif
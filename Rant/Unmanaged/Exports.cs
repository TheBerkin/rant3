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

        [DllExport("RantGetLastError", CallingConvention.Cdecl)]
        public static ErrorCode GetLastError(UnmanagedRantContext context) => context.LastErrorCode;

        [DllExport("RantGetLastErrorMessage", CallingConvention.Cdecl)]
        public static string GetLastErrorMessage(UnmanagedRantContext context) => context.LastErrorMessage;

        [DllExport("RantLoadEngine", CallingConvention.Cdecl)]
        public static ErrorCode LoadEngine(UnmanagedRantContext context, string dictionaryPath) => context.Run(() => context.Rant = new RantEngine(dictionaryPath));

        [DllExport("RantCompilePatternString", CallingConvention.Cdecl)]
        public static ErrorCode CompilePatternString(UnmanagedRantContext context, string patternString, out Unmanaged<RantPattern> patternCompiled)
        {
            RantPattern pattern = null;
            context.Run(() => pattern = RantPattern.FromString(patternString));
            patternCompiled = Unmanaged.Make(pattern);
            return context.LastErrorCode;
        }

        [DllExport("RantCompilePatternFile", CallingConvention.Cdecl)]
        public static ErrorCode CompilePatternFile(UnmanagedRantContext context, string patternPath, out Unmanaged<RantPattern> patternCompiled)
        {
            RantPattern pattern = null;
            context.Run(() => pattern = RantPattern.FromFile(patternPath));
            patternCompiled = Unmanaged.Make(pattern);
            return context.LastErrorCode;
        }

        [DllExport("RantRunPattern", CallingConvention.Cdecl)]
        public static ErrorCode RunPattern(UnmanagedRantContext context, Unmanaged<RantPattern> pattern, PatternOptions options, out Unmanaged<RantOutput> output)
        {
            RantOutput o = null;
            context.Run(() => o = context.Rant.Do(pattern.Value, options.CharLimit, options.Timeout));
            output = Unmanaged.Make(o);
            return context.LastErrorCode;
        }

        [DllExport("RantRunPatternSeed", CallingConvention.Cdecl)]
        public static ErrorCode RunPatternSeed(UnmanagedRantContext context, Unmanaged<RantPattern> pattern, PatternOptions options, long seed, out Unmanaged<RantOutput> output)
        {
            RantOutput o = null;
            context.Run(() => o = context.Rant.Do(pattern.Value, seed, options.CharLimit, options.Timeout));
            output = Unmanaged.Make(o);
            return context.LastErrorCode;
        }

        [DllExport("RantGetMainValue", CallingConvention.Cdecl)]
        public static string GetMainValue(Unmanaged<RantOutput> output) => output?.Value?.Main;

        [DllExport("RantGetOutputChannelNames", CallingConvention.Cdecl)]
        public static string[] GetOutputChannelNames(Unmanaged<RantOutput> output, out int count)
        {
            var names = output.Value.Select(e => e.Name).ToArray();
            count = names.Length;
            GC.KeepAlive(names);
            return names;
        }

        [DllExport("RantGetOutputValue", CallingConvention.Cdecl)]
        public static string GetOutputValue(Unmanaged<RantOutput> output, string channelName) => output?.Value?[channelName];

        [DllExport("RantLoadPackage", CallingConvention.Cdecl)]
        public static ErrorCode LoadPackage(UnmanagedRantContext context, string packagePath) => context.Run(() => context.Rant.LoadPackage(packagePath));
    }
}
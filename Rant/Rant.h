// This is the header file for using Rant from C/C++.
// To use, simply reference the Rant.lib and Rant.dll files in your project and include this header.
// Make sure you compile Rant under x86 or x64, depending on your system.

#include <stdint.h>

#define RANTAPI extern "C" __declspec(dllimport)

typedef void* RANTCONTEXT;
typedef void* RANTPATTERN;
typedef void* RANTOUTPUT;
typedef int RANTRESULT;
typedef const char* RANTSTR;

#define RANT_OK 0
#define RANT_COMPILE_ERROR 1
#define RANT_RUNTIME_ERROR 2
#define RANT_MISC_ERROR 3

struct RANTOPTIONS
{
	int32_t CharLimit;
	double Timeout;
};

RANTAPI RANTCONTEXT __cdecl RantCreateContext();

RANTAPI void __cdecl RantReleaseContext(RANTCONTEXT context);

RANTAPI RANTRESULT __cdecl RantGetLastError(RANTCONTEXT context);

RANTAPI const char* __cdecl RantGetLastErrorMessage(RANTCONTEXT context);

RANTAPI RANTRESULT __cdecl RantLoadEngine(RANTCONTEXT context, RANTSTR dictionaryPath);

RANTAPI bool __cdecl RantIsEngineLoaded(RANTCONTEXT context);

RANTAPI RANTRESULT __cdecl RantCompilePatternString(RANTCONTEXT context, RANTSTR patternString, RANTPATTERN* patternCompiled);

RANTAPI RANTRESULT __cdecl RantCompilePatternFile(RANTCONTEXT context, RANTSTR patternPath, RANTPATTERN* patternCompiled);

RANTAPI RANTRESULT __cdecl RantRunPattern(RANTCONTEXT context, RANTPATTERN pattern, RANTOPTIONS options, RANTOUTPUT* output);

RANTAPI RANTRESULT __cdecl RantRunPatternSeed(RANTCONTEXT context, RANTPATTERN pattern, RANTOPTIONS options, int64_t seed, RANTOUTPUT* output);

RANTAPI RANTSTR __cdecl RantGetMainValue(RANTOUTPUT output);

RANTAPI RANTSTR* __cdecl RantGetOutputChannelNames(RANTOUTPUT output, int* count);

RANTAPI RANTSTR __cdecl RantGetOutputValue(RANTOUTPUT output, RANTSTR channelName);

RANTAPI RANTRESULT __cdecl RantLoadPackage(RANTCONTEXT context, RANTSTR packagePath);
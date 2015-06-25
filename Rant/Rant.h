// This is the header file for using Rant from C/C++.
// To use, simply reference the Rant.lib and Rant.dll files in your project and include this header.
// Make sure you compile Rant under x86 or x64, depending on your system.

#pragma once

#include <stdint.h>

#define RANTAPI(result) extern "C" __declspec(dllimport) result __cdecl

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
	double Timeout;
	int32_t CharLimit;
};

RANTAPI(RANTCONTEXT) RantCreateContext();

RANTAPI(void) RantReleaseContext(RANTCONTEXT context);

RANTAPI(void) RantReleasePattern(RANTPATTERN pattern);

RANTAPI(void) RantReleaseOutput(RANTOUTPUT output);

RANTAPI(RANTRESULT) RantGetLastError(RANTCONTEXT context);

RANTAPI(RANTSTR) RantGetLastErrorMessage(RANTCONTEXT context);

RANTAPI(RANTRESULT) RantLoadEngine(RANTCONTEXT context, RANTSTR dictionaryPath);

RANTAPI(bool) RantIsEngineLoaded(RANTCONTEXT context);

RANTAPI(RANTRESULT) RantCompilePatternString(RANTCONTEXT context, RANTSTR patternString, RANTPATTERN* patternCompiled);

RANTAPI(RANTRESULT) RantCompilePatternFile(RANTCONTEXT context, RANTSTR patternPath, RANTPATTERN* patternCompiled);

RANTAPI(RANTRESULT) RantRunPattern(RANTCONTEXT context, RANTPATTERN pattern, RANTOPTIONS options, RANTOUTPUT* output);

RANTAPI(RANTRESULT) RantRunPatternSeed(RANTCONTEXT context, RANTPATTERN pattern, RANTOPTIONS options, int64_t seed, RANTOUTPUT* output);

RANTAPI(RANTSTR) RantGetMainValue(RANTOUTPUT output);

RANTAPI(RANTSTR*) RantGetOutputChannelNames(RANTOUTPUT output, int* count);

RANTAPI(RANTSTR) RantGetOutputValue(RANTOUTPUT output, RANTSTR channelName);

RANTAPI(RANTRESULT) RantLoadPackage(RANTCONTEXT context, RANTSTR packagePath);
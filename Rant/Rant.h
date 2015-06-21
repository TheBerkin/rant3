#include <stdint.h>

#define RANTAPI extern "C" __declspec(dllimport)


typedef void* RANTCONTEXT;
typedef void* RANTPATTERN;
typedef void* RANTOUTPUT;
typedef int RANTRESULT;

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

RANTAPI RANTRESULT __cdecl RantLoadEngine(RANTCONTEXT context, const char* dictionaryPath);

RANTAPI RANTRESULT __cdecl RantCompilePatternString(RANTCONTEXT context, const char* patternString, RANTPATTERN* patternCompiled);

RANTAPI RANTRESULT __cdecl RantCompilePatternFile(RANTCONTEXT context, const char* patternPath, RANTPATTERN* patternCompiled);

RANTAPI RANTRESULT __cdecl RantRunPattern(RANTCONTEXT context, RANTPATTERN pattern, RANTOPTIONS options, RANTOUTPUT* output);

RANTAPI RANTRESULT __cdecl RantRunPatternSeed(RANTCONTEXT context, RANTPATTERN pattern, RANTOPTIONS options, int64_t seed, RANTOUTPUT* output);

RANTAPI const char* __cdecl RantGetMainValue(RANTOUTPUT output);

RANTAPI RANTRESULT __cdecl RantLoadPackage(RANTCONTEXT context, const char* packagePath);

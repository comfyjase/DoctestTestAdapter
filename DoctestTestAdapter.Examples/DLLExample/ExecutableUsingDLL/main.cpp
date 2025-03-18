#define DOCTEST_CONFIG_IMPLEMENTATION_IN_DLL
#include <doctest.h>
#include "TestIsEvenExecutableUsingDLL.h"

#include <iostream>

// To workaround: winbase.h(7937,5): warning C5039: 'TpSetCallbackCleanupGroup': pointer or reference to potentially throwing function passed to 'extern "C"' function under -EHc. Undefined behavior may occur if this function throws an exception.
// Want to keep /Wall active for my code but not have windows causing build errors.
#pragma warning(push)
#pragma warning(disable : 5039)

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

#pragma warning(pop) 

int main(int argc, char** argv)
{
	LoadLibrary(L"DLL.dll");

	doctest::Context context;
	
	context.applyCommandLine(argc, argv);

	int doctestResult = context.run();

	if (context.shouldExit())	// important - query flags (and --exit) rely on the user doing this
		return doctestResult;

	return doctestResult;
}

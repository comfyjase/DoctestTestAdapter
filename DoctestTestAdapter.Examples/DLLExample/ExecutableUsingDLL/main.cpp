#define DOCTEST_CONFIG_IMPLEMENTATION_IN_DLL
#include <doctest.h>
#include "TestIsEvenExecutableUsingDLL.h"

#include <iostream>

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

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

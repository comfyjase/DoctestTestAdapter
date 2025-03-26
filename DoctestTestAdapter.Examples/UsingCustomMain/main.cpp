#define DOCTEST_CONFIG_IMPLEMENT
#include <doctest.h>

#pragma warning(push)

// Want to keep all warnings enabled but seeing "function not inlined" for TEST_CASE_FIXTURE code 
// so ignoring this specific warning for the examples
#pragma warning(disable : 4710)

#include "TestIsEvenUsingCustomMain.h"

#pragma warning(pop)

int main(int argc, char** argv)
{
	doctest::Context context;

	context.applyCommandLine(argc, argv);

	int doctestResult = context.run();
	if (context.shouldExit())	// important - query flags (and --exit) rely on the user doing this
		return doctestResult;	// propagate the result of the tests

	// Any custom main logic would go here...
	// But to keep this example as simple as possible, let's not do anything here

	return doctestResult;
}

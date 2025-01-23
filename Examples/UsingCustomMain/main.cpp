#define DOCTEST_CONFIG_IMPLEMENT
#include <doctest.h>

#include "TestIsEven.h"

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

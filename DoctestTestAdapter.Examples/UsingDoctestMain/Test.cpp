#define DOCTEST_CONFIG_IMPLEMENT_WITH_MAIN
#include <doctest.h>

#pragma warning(push)

// Want to keep all warnings enabled but seeing "function not inlined" for TEST_CASE_FIXTURE code 
// so ignoring this specific warning for the examples
#pragma warning(disable : 4710)

#include "TestIsEvenUsingDoctestMain.h"
// Add other test header files here...

#pragma warning(pop)

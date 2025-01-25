#pragma once

#include <doctest.h>
#include <MathUtilitiesDLL.h>

namespace TestExecutableUsingDLL
{
    TEST_CASE("[ExecutableUsingDLL] - Testing IsEven")
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }
}

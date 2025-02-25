#pragma once

#include <doctest.h>
#include <MathUtilitiesDLL.h>

namespace TestExecutableUsingDLL
{
    TEST_CASE("[ExecutableUsingDLL] Testing IsEven Always Pass")
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }

    TEST_CASE("[ExecutableUsingDLL] Testing IsEven Always Fail")
    {
        CHECK(IsEven(1));
        CHECK(IsEven(3));
        CHECK(IsEven(5));
    }

    TEST_CASE("[ExecutableUsingDLL] Testing IsEven Always Skipped" * doctest::skip())
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }
}

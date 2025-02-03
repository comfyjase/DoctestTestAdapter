#pragma once

#include <doctest.h>
#include "MathUtilitiesDLL.h"

namespace TestDLL
{
    TEST_CASE("[DLL] Testing IsEven Always Pass")
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }

    TEST_CASE("[DLL] Testing IsEven Always Fail")
    {
        CHECK(IsEven(7));
        CHECK(IsEven(9));
        CHECK(IsEven(11));
    }

    TEST_CASE("[DLL] Testing IsEven Always Skipped" * doctest::skip())
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }
}

#pragma once

#include <doctest.h>
#include "MathUtilitiesDLL.h"

namespace TestDLL
{
    TEST_CASE("[DLL] - Testing IsEven Always Pass")
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }

    TEST_CASE("[DLL] - Testing IsEven Always Fail")
    {
        CHECK(IsEven(1));
        CHECK(IsEven(3));
        CHECK(IsEven(5));
    }

    TEST_CASE("[Skip][DLL] - Testing IsEven Always Skipped")
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }
}

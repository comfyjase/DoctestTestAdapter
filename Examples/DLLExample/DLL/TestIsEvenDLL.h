#pragma once

#include <doctest.h>
#include "MathUtilitiesDLL.h"

namespace TestDLL
{
    TEST_CASE("[DLL] - Testing IsEven")
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }
}

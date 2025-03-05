#pragma once

#include <doctest.h>
#include "MathUtilitiesDLL.h"

namespace TestDLL
{
    /********************************************************************************************************************/
    /*TEST_CASE("[DLL] Testing IsEven Is Block Mult-Line Commented Out New Line SHOULD NOT APPEAR IN TEST EXPLORER")*****/
    /*{																													*/
    /*	CHECK(IsEven(2));																								*/
    /*}																													*/
    /********************************************************************************************************************/

    //TEST_CASE("[DLL] Testing IsEven Is Single Commented Out SHOULD NOT APPEAR IN TEST EXPLORER")
    //{
    //    CHECK(IsEven(2));
    //}

    /*TEST_CASE("[DLL] Testing IsEven Is Block Commented Out On Same Line SHOULD NOT APPEAR IN TEST EXPLORER")
    {
        CHECK(IsEven(2));
    }*/

    /*
    TEST_CASE("[DLL] Testing IsEven Is Block Commented Out New Line SHOULD NOT APPEAR IN TEST EXPLORER")
    {
        CHECK(IsEven(2));
    }
    */

#if 0
    TEST_CASE("[DLL] Testing Is Even Is Wrapped In '#if 0' Block SHOULD NOT APPEAR IN TEST EXPLORER")
    {
        CHECK(IsEven(2));
    }
#endif

#if false
    TEST_CASE("[DLL] Testing Is Even Is Wrapped In '#if false' Block SHOULD NOT APPEAR IN TEST EXPLORER")
    {
        CHECK(IsEven(2));
    }
#endif

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

    TEST_CASE("[DLL] Testing IsEven, Always Pass, With Commas In Name")
    {
        CHECK(IsEven(2));
        CHECK(IsEven(4));
        CHECK(IsEven(6));
    }
}

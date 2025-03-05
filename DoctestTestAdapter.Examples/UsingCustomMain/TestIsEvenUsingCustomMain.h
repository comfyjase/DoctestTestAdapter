#pragma once

#include <doctest.h>

namespace TestUsingCustomMain
{
	/********************************************************************************************************************************/
	/*TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Mult-Line Commented Out New Line SHOULD NOT APPEAR IN TEST EXPLORER")*****/
	/*{																																*/
	/*	CHECK(IsEven(2));																											*/
	/*}																																*/
	/********************************************************************************************************************************/

	//TEST_CASE("[UsingCustomMain] Testing IsEven Is Single Commented Out SHOULD NOT APPEAR IN TEST EXPLORER")
	//{
	//    CHECK(IsEven(2));
	//}

	/*TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Commented Out On Same Line SHOULD NOT APPEAR IN TEST EXPLORER")
	{
		CHECK(IsEven(2));
	}*/

	/*
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Commented Out New Line SHOULD NOT APPEAR IN TEST EXPLORER")
	{
		CHECK(IsEven(2));
	}
	*/

#if 0
	TEST_CASE("[UsingCustomMain] Testing Is Even Is Wrapped In '#if 0' Block SHOULD NOT APPEAR IN TEST EXPLORER")
	{
		CHECK(IsEven(2));
	}
#endif

#if false
	TEST_CASE("[UsingCustomMain] Testing Is Even Is Wrapped In '#if false' Block SHOULD NOT APPEAR IN TEST EXPLORER")
	{
		CHECK(IsEven(2));
	}
#endif

	bool IsEven(int number)
	{
		return (number % 2 == 0);
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Pass")
	{
		CHECK(IsEven(2));
		CHECK(IsEven(4));
		CHECK(IsEven(6));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Fail")
	{
		CHECK(IsEven(1));
		CHECK(IsEven(3));
		CHECK(IsEven(5));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Skipped" * doctest::skip())
	{
		CHECK(IsEven(2));
		CHECK(IsEven(4));
		CHECK(IsEven(6));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven, Always Pass, With Commas In Name")
	{
		CHECK(IsEven(2));
		CHECK(IsEven(4));
		CHECK(IsEven(6));
	}
}

#pragma once

#include <doctest.h>

namespace TestUsingCustomMain
{
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
}

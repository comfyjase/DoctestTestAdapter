#pragma once

#include <doctest.h>

namespace TestUsingDoctestMain
{
	bool IsEven(int number)
	{
		return (number % 2 == 0);
	}

	TEST_CASE("[UsingDoctestMain] - Testing IsEven")
	{
		CHECK(IsEven(2));
		CHECK(IsEven(4));
		CHECK(IsEven(6));
	}
}

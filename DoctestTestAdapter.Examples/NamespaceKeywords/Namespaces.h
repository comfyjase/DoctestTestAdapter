#pragma once

#include <iostream>
#include <doctest.h>

namespace Test
{
	using namespace std;

	TEST_CASE("Testing Namespaces With Using")
	{
		CHECK(true);
	}
} // end namespace Test

namespace std
{
	void Print(string message)
	{
		cout << message << endl;
	}
} // end namespace std

namespace {

	using namespace Test;

	const double pi = 3.1415926535897932384626433832795;

} // end anonymous namespace

namespace Test
{
	using namespace std;

	TEST_CASE("Testing Multiple Namespaces With The Same Name")
	{
		CHECK(true);
	}
} // end namespace Test

namespace Test
{
	TEST_SUITE("[Multiple Namespaces With The Same Name Suite]")
	{
		using namespace std;

		TEST_CASE("Testing Multiple Namespaces With The Same Name")
		{
			CHECK(true);
		}
	}
} // end namespace Test

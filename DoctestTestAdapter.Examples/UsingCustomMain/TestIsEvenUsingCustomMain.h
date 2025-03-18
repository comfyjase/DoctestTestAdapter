#pragma once

#include <doctest.h>

#define CUSTOM_TEST_MACRO_TRUE true
#define CUSTOM_TEST_MACRO_FALSE false

bool IsEven(int number)
{
	return (number % 2 == 0);
}

// =====================================================================================================
//
//	TEST_CASES_IN_NO_NAMESPACE_OR_TEST_SUITE
//
#pragma region TEST_CASES_IN_NO_NAMESPACE_OR_TEST_SUITE

TEST_CASE("[UsingCustomMain] Testing IsEven Always Pass In No Namespace Or Test Suite")
{
	CHECK(IsEven(2));
}

TEST_CASE("[UsingCustomMain] Testing IsEven Always Fail In No Namespace Or Test Suite")
{
	CHECK(IsEven(3));
}

TEST_CASE("[UsingCustomMain] Testing IsEven Always Skip In No Namespace Or Test Suite" * doctest::skip())
{
	CHECK(IsEven(3));
}

TEST_CASE("[UsingCustomMain] Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In No Namespace Or Test Suite")
{
	CHECK(IsEven(2));
}

TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In No Namespace Or Test Suite")
{
	CHECK(IsEven(2));
}

//TEST_CASE("[UsingCustomMain] Testing IsEven Is Commented Out In No Namespace Or Test Suite")
//{
//	CHECK(IsEven(2));
//}

/*
TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Commented Out In No Namespace Or Test Suite")
{
	CHECK(IsEven(2));
}
*/

#if 0
TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if 0 In No Namespace Or Test Suite")
{
	CHECK(IsEven(2));
}
#endif // 0

#if false
TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if false In No Namespace Or Test Suite")
{
	CHECK(IsEven(2));
}
#endif // false

#if CUSTOM_TEST_MACRO_FALSE
TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if CUSTOM_TEST_MACRO_FALSE In No Namespace Or Test Suite")
{
	CHECK(IsEven(2));
}
#endif // CUSTOM_TEST_MACRO_FALSE

#if CUSTOM_TEST_MACRO_TRUE
TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In No Namespace Or Test Suite")
{
	CHECK(IsEven(2));
}
#endif // CUSTOM_TEST_MACRO_TRUE

#pragma endregion

// =====================================================================================================
//
//	TEST_CASES_IN_TEST_SUITE
//
#pragma region TEST_CASES_IN_TEST_SUITE

TEST_SUITE("[UsingCustomMainTestSuite]")
{
	TEST_CASE("[UsingCustomMain] Testing IsEven Always Pass In Test Suite")
	{
		CHECK(IsEven(2));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Fail In Test Suite")
	{
		CHECK(IsEven(3));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Skip In Test Suite" * doctest::skip())
	{
		CHECK(IsEven(3));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Test Suite")
	{
		CHECK(IsEven(2));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Test Suite")
	{
		CHECK(IsEven(2));
	}

	//TEST_CASE("[UsingCustomMain] Testing IsEven Is Commented Out In Test Suite")
	//{
	//	CHECK(IsEven(2));
	//}

	/*
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Commented Out In Test Suite")
	{
		CHECK(IsEven(2));
	}
	*/

#if 0
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if 0 In Test Suite")
	{
		CHECK(IsEven(2));
	}
#endif

#if false
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if false In Test Suite")
	{
		CHECK(IsEven(2));
	}
#endif

#if CUSTOM_TEST_MACRO_FALSE
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if CUSTOM_TEST_MACRO_FALSE In Test Suite")
	{
		CHECK(IsEven(2));
	}
#endif

#if CUSTOM_TEST_MACRO_TRUE
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Test Suite")
	{
		CHECK(IsEven(2));
	}
#endif
}

#pragma endregion

// =====================================================================================================
//
//	TEST_CASES_IN_SKIPPED_TEST_SUITE
//
#pragma region TEST_CASES_IN_SKIPPED_TEST_SUITE

TEST_SUITE("[UsingCustomMainSkippedTestSuite]" * doctest::skip())
{
	TEST_CASE("[UsingCustomMain] Testing IsEven Always Pass In Skipped Test Suite")
	{
		CHECK(IsEven(2));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Fail In Skipped Test Suite")
	{
		CHECK(IsEven(3));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Skip In Skipped Test Suite" * doctest::skip())
	{
		CHECK(IsEven(3));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Skipped Test Suite")
	{
		CHECK(IsEven(2));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Skipped Test Suite")
	{
		CHECK(IsEven(2));
	}

	//TEST_CASE("[UsingCustomMain] Testing IsEven Is Commented Out In Skipped Test Suite")
	//{
	//	CHECK(IsEven(2));
	//}

	/*
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Commented Out In Skipped Test Suite")
	{
		CHECK(IsEven(2));
	}
	*/

#if 0
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if 0 In Skipped Test Suite")
	{
		CHECK(IsEven(2));
	}
#endif

#if false
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if false In Skipped Test Suite")
	{
		CHECK(IsEven(2));
	}
#endif

#if CUSTOM_TEST_MACRO_FALSE
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if CUSTOM_TEST_MACRO_FALSE In Skipped Test Suite")
	{
		CHECK(IsEven(2));
	}
#endif

#if CUSTOM_TEST_MACRO_TRUE
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Skipped Test Suite")
	{
		CHECK(IsEven(2));
	}
#endif
}

#pragma endregion

// =====================================================================================================
//
//	TEST_CASES_IN_NAMESPACE
//
#pragma region TEST_CASES_IN_NAMESPACE

namespace UsingCustomMainNamespace
{
	TEST_CASE("[UsingCustomMain] Testing IsEven Always Pass In Namespace")
	{
		CHECK(IsEven(2));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Fail In Namespace")
	{
		CHECK(IsEven(3));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven Always Skip In Namespace" * doctest::skip())
	{
		CHECK(IsEven(3));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Namespace")
	{
		CHECK(IsEven(2));
	}

	TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Namespace")
	{
		CHECK(IsEven(2));
	}

	//TEST_CASE("[UsingCustomMain] Testing IsEven Is Commented Out In Namespace")
	//{
	//	CHECK(IsEven(2));
	//}

	/*
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Commented Out In Namespace")
	{
		CHECK(IsEven(2));
	}
	*/

#if 0
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if 0 In Namespace")
	{
		CHECK(IsEven(2));
	}
#endif

#if false
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if false In Namespace")
	{
		CHECK(IsEven(2));
	}
#endif

#if CUSTOM_TEST_MACRO_FALSE
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if CUSTOM_TEST_MACRO_FALSE In Namespace")
	{
		CHECK(IsEven(2));
	}
#endif

#if CUSTOM_TEST_MACRO_TRUE
	TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Namespace")
	{
		CHECK(IsEven(2));
	}
#endif
}

#pragma endregion

// =====================================================================================================
//
//	TEST_CASES_IN_NESTED_NAMESPACE
//
#pragma region TEST_CASES_IN_NESTED_NAMESPACE

namespace UsingCustomMainNestedNamespaceOne
{
	namespace UsingCustomMainNestedNamespaceTwo
	{
		TEST_CASE("[UsingCustomMain] Testing IsEven Always Pass In Nested Namespace")
		{
			CHECK(IsEven(2));
		}

		TEST_CASE("[UsingCustomMain] Testing IsEven Always Fail In Nested Namespace")
		{
			CHECK(IsEven(3));
		}

		TEST_CASE("[UsingCustomMain] Testing IsEven Always Skip In Nested Namespace" * doctest::skip())
		{
			CHECK(IsEven(3));
		}

		TEST_CASE("[UsingCustomMain] Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Nested Namespace")
		{
			CHECK(IsEven(2));
		}

		TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Nested Namespace")
		{
			CHECK(IsEven(2));
		}

		//TEST_CASE("[UsingCustomMain] Testing IsEven Is Commented Out In Nested Namespace")
		//{
		//	CHECK(IsEven(2));
		//}

		/*
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Commented Out In Nested Namespace")
		{
			CHECK(IsEven(2));
		}
		*/

#if 0
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if 0 In Nested Namespace")
		{
			CHECK(IsEven(2));
		}
#endif

#if false
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if false In Nested Namespace")
		{
			CHECK(IsEven(2));
		}
#endif

#if CUSTOM_TEST_MACRO_FALSE
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if CUSTOM_TEST_MACRO_FALSE In Nested Namespace")
		{
			CHECK(IsEven(2));
		}
#endif

#if CUSTOM_TEST_MACRO_TRUE
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Nested Namespace")
		{
			CHECK(IsEven(2));
		}
#endif
	}
}

#pragma endregion

// =====================================================================================================
//
//	TEST_CASES_IN_NAMESPACE_AND_TEST_SUITE
//
#pragma region TEST_CASES_IN_NESTED_NAMESPACE_AND_TEST_SUITE

namespace UsingCustomMainNamespaceAndTestSuite_Namespace
{
	TEST_SUITE("[UsingCustomMainNamespaceAndTestSuite_TestSuite]")
	{
		TEST_CASE("[UsingCustomMain] Testing IsEven Always Pass In Namespace And Test Suite")
		{
			CHECK(IsEven(2));
		}

		TEST_CASE("[UsingCustomMain] Testing IsEven Always Fail In Namespace And Test Suite")
		{
			CHECK(IsEven(3));
		}

		TEST_CASE("[UsingCustomMain] Testing IsEven Always Skip In Namespace And Test Suite" * doctest::skip())
		{
			CHECK(IsEven(3));
		}

		TEST_CASE("[UsingCustomMain] Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Namespace And Test Suite")
		{
			CHECK(IsEven(2));
		}

		TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Namespace And Test Suite")
		{
			CHECK(IsEven(2));
		}

		//TEST_CASE("[UsingCustomMain] Testing IsEven Is Commented Out In Namespace And Test Suite")
		//{
		//	CHECK(IsEven(2));
		//}

		/*
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Block Commented Out In Namespace And Test Suite")
		{
			CHECK(IsEven(2));
		}
		*/

#if 0
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if 0 In Namespace And Test Suite")
		{
			CHECK(IsEven(2));
		}
#endif

#if false
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if false In Namespace And Test Suite")
		{
			CHECK(IsEven(2));
		}
#endif

#if CUSTOM_TEST_MACRO_FALSE
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled Out #if CUSTOM_TEST_MACRO_FALSE In Namespace And Test Suite")
		{
			CHECK(IsEven(2));
		}
#endif

#if CUSTOM_TEST_MACRO_TRUE
		TEST_CASE("[UsingCustomMain] Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Namespace And Test Suite")
		{
			CHECK(IsEven(2));
		}
#endif
	}
}

#pragma endregion

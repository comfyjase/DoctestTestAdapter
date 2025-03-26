#pragma once

#include <doctest.h>

#define CUSTOM_TEST_MACRO_TRUE true
#define CUSTOM_TEST_MACRO_FALSE false

inline bool IsEven(int number)
{
	return (number % 2 == 0);
}

class UniqueTestsFixture
{
public:
	UniqueTestsFixture() {}
protected:
	inline bool FixtureIsEven(int number)
	{
		return IsEven(number);
	}
};

#define CUSTOM_TEST_CASE_MACRO(name, number)											\
	TEST_CASE("[UsingCustomMain] Testing IsEven From Custom Test Case Macro " name)		\
	{																					\
		CHECK(IsEven(number));															\
	}

#define CUSTOM_TEST_CASE_FIXTURE_MACRO(fixtureClass, name, number)												\
	TEST_CASE_FIXTURE(fixtureClass, "[UsingCustomMain] Testing IsEven From Custom Test Fixture Macro " name)	\
	{																											\
		CHECK(FixtureIsEven(number));																			\
	}

#define CUSTOM_TEST_CASE_TEMPLATE_MACRO(name, templateTypeName, templateType)															\
	TEST_CASE_TEMPLATE("[UsingCustomMain] Testing IsEven From Custom Test Case Template Macro " name, templateTypeName, templateType)	\
	{																																	\
		CHECK(IsEven((templateTypeName)2));																								\
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

TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In No Namespace Or Test Suite")
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

TEST_CASE_FIXTURE(UniqueTestsFixture, "[UsingCustomMain] Testing IsEven Test Case Fixture In No Namespace Or Test Suite")
{
	CHECK(FixtureIsEven(2));
}

TEST_CASE_TEMPLATE("[UsingCustomMain] Testing IsEven Test Case Template In No Namespace Or Test Suite", T, int)
{
	CHECK(IsEven((T)2));
}

CUSTOM_TEST_CASE_MACRO("In No Namespace Or Test Suite", 2);

CUSTOM_TEST_CASE_FIXTURE_MACRO(UniqueTestsFixture, "In No Namespace Or Test Suite", 2);

CUSTOM_TEST_CASE_TEMPLATE_MACRO("In No Namespace Or Test Suite", T, int);

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

	TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Test Suite")
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

	TEST_CASE_FIXTURE(UniqueTestsFixture, "[UsingCustomMain] Testing IsEven Test Case Fixture In Test Suite")
	{
		CHECK(FixtureIsEven(2));
	}

	TEST_CASE_TEMPLATE("[UsingCustomMain] Testing IsEven Test Case Template In Test Suite", T, int)
	{
		CHECK(IsEven((T)2));
	}

	CUSTOM_TEST_CASE_MACRO("In Test Suite", 2);

	CUSTOM_TEST_CASE_FIXTURE_MACRO(UniqueTestsFixture, "In Test Suite", 2);

	CUSTOM_TEST_CASE_TEMPLATE_MACRO("In Test Suite", T, int);
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

	TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Skipped Test Suite")
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

	TEST_CASE_FIXTURE(UniqueTestsFixture, "[UsingCustomMain] Testing IsEven Test Case Fixture In Skipped Test Suite")
	{
		CHECK(FixtureIsEven(2));
	}

	TEST_CASE_TEMPLATE("[UsingCustomMain] Testing IsEven Test Case Template In Skipped Test Suite", T, int)
	{
		CHECK(IsEven((T)2));
	}

	CUSTOM_TEST_CASE_MACRO("In Skipped Test Suite", 2);

	CUSTOM_TEST_CASE_FIXTURE_MACRO(UniqueTestsFixture, "In Skipped Test Suite", 2);

	CUSTOM_TEST_CASE_TEMPLATE_MACRO("In Skipped Test Suite", T, int);
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

	TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Namespace")
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

	TEST_CASE_FIXTURE(UniqueTestsFixture, "[UsingCustomMain] Testing IsEven Test Case Fixture In Namespace")
	{
		CHECK(FixtureIsEven(2));
	}

	TEST_CASE_TEMPLATE("[UsingCustomMain] Testing IsEven Test Case Template In Namespace", T, int)
	{
		CHECK(IsEven((T)2));
	}

	CUSTOM_TEST_CASE_MACRO("In Namespace", 2);

	CUSTOM_TEST_CASE_FIXTURE_MACRO(UniqueTestsFixture, "In Namespace", 2);

	CUSTOM_TEST_CASE_TEMPLATE_MACRO("In Namespace", T, int);
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

		TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Nested Namespace")
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

		TEST_CASE_FIXTURE(UniqueTestsFixture, "[UsingCustomMain] Testing IsEven Test Case Fixture In Nested Namespace")
		{
			CHECK(FixtureIsEven(2));
		}

		TEST_CASE_TEMPLATE("[UsingCustomMain] Testing IsEven Test Case Template In Nested Namespace", T, int)
		{
			CHECK(IsEven((T)2));
		}

		CUSTOM_TEST_CASE_MACRO("In Nested Namespace", 2);

		CUSTOM_TEST_CASE_FIXTURE_MACRO(UniqueTestsFixture, "In Nested Namespace", 2);

		CUSTOM_TEST_CASE_TEMPLATE_MACRO("In Nested Namespace", T, int);
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

		TEST_CASE("[UsingCustomMain] Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Namespace And Test Suite")
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

		TEST_CASE_FIXTURE(UniqueTestsFixture, "[UsingCustomMain] Testing IsEven Test Case Fixture In Namespace And Test Suite")
		{
			CHECK(FixtureIsEven(2));
		}

		TEST_CASE_TEMPLATE("[UsingCustomMain] Testing IsEven Test Case Template In Namespace And Test Suite", T, int)
		{
			CHECK(IsEven((T)2));
		}

		CUSTOM_TEST_CASE_MACRO("In Namespace And Test Suite", 2);

		CUSTOM_TEST_CASE_FIXTURE_MACRO(UniqueTestsFixture, "In Namespace And Test Suite", 2);

		CUSTOM_TEST_CASE_TEMPLATE_MACRO("In Namespace And Test Suite", T, int);
	}
}

#pragma endregion

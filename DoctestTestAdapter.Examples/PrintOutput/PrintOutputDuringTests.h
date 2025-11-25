#pragma once

#include <iostream>
#include <doctest.h>

namespace TestPrintOutput
{
	TEST_CASE("[PrintOutput] - INFO")
	{
		INFO("INFO called for test that will fail");
		CHECK(false);
	}

	TEST_CASE("[PrintOutput] - INFO In Passing Test")
	{
		INFO("INFO should not be called for this test that will pass");
		CHECK(true);
	}

	TEST_CASE("[PrintOutput] - INFO With Variable")
	{
		int variable = 11;
		INFO("INFO called for test that will fail with variable: ", variable);
		CHECK(false);
	}

	TEST_CASE("[PrintOutput] - Multiple INFO With Variables")
	{
		int variable = 11;
		int another_variable = 15;
		INFO("INFO called for test that will fail with variable: ", variable);
		INFO("Another INFO called for test that will fail with another_variable: ", another_variable);
		CHECK(false);
	}
	
	TEST_CASE("[PrintOutput] - MESSAGE")
	{
		MESSAGE("MESSAGE called before check");
		CHECK(true);
		MESSAGE("MESSAGE called after check");
	}
	
	TEST_CASE("[PrintOutput] - MESSAGE With Variable")
	{
		int variable = 38;
		MESSAGE("MESSAGE called before check with variable: ", variable);
		CHECK(true);
		MESSAGE("MESSAGE called after check with variable: ", variable);
	}

	TEST_CASE("[PrintOutput] - Multiple MESSAGEs With Variable")
	{
		int variable = 38;
		int another_variable = 5;
		MESSAGE("MESSAGE called before check with variable: ", variable);
		MESSAGE("Another MESSAGE called before check with another_variable: ", another_variable);
		CHECK(true);
		MESSAGE("MESSAGE called after check with variable: ", variable);
		MESSAGE("Another MESSAGE called after check with another_variable: ", another_variable);
	}
	
	TEST_CASE("[PrintOutput] - CHECK_MESSAGE")
	{
		CHECK_MESSAGE(false, "CHECK_MESSAGE called for failing test.");
	}

	TEST_CASE("[PrintOutput] - REQUIRE_MESSAGE")
	{
		REQUIRE_MESSAGE(false, "REQUIRE_MESSAGE called for failing test");
	}

	TEST_CASE("[PrintOutput] - FAIL")
	{
		FAIL("FAIL called for failing test");
	}

	TEST_CASE("[PrintOutput] - FAIL_CHECK")
	{
		FAIL_CHECK("FAIL_CHECK called for failing test");
	}

	TEST_CASE("[PrintOutput] - Multiple CHECK + INFO")
	{
		INFO("INFO called for test that will fail");
		CHECK(false);

		INFO("Another INFO called for test that will fail");
		CHECK(false);
	}

	TEST_CASE("[PrintOutput] - With Passing SUBCASES")
	{
		MESSAGE("Message from TEST_CASE parent!");

		SUBCASE("[PrintOutput] - Passing SUBCASE 1")
		{
			MESSAGE("Message from SUBCASE 1!");
			CHECK(true);
		}

		SUBCASE("[PrintOutput] - Passing SUBCASE 2")
		{
			MESSAGE("Message from SUBCASE 2!");
			CHECK(true);
		}

		SUBCASE("[PrintOutput] - Nested SUBCASEs")
		{
			MESSAGE("Message from Nested SUBCASE parent!");

			SUBCASE("[PrintOutput] - Nested SUBCASE 1")
			{
				MESSAGE("Message from Nested SUBCASE 1!");
				CHECK(true);
			}

			SUBCASE("[PrintOutput] - Nested SUBCASE 2")
			{
				MESSAGE("Message from Nested SUBCASE 2!");
				CHECK(true);
			}
		}
	}

	TEST_CASE("[PrintOutput] - With Failing SUBCASES")
	{
		INFO("Some info when failing from TEST_CASE parent!");

		SUBCASE("[PrintOutput] - Failing SUBCASE 1")
		{
			INFO("Info from SUBCASE 1!");
			CHECK(false);
		}

		SUBCASE("[PrintOutput] - Failing SUBCASE 2")
		{
			INFO("Info from SUBCASE 2!");
			CHECK(false);
		}

		SUBCASE("[PrintOutput] - Nested SUBCASEs")
		{
			INFO("Info from Nested SUBCASE parent!");

			SUBCASE("[PrintOutput] - Nested SUBCASE 1")
			{
				INFO("Info from Nested SUBCASE 1!");
				CHECK(false);
			}

			SUBCASE("[PrintOutput] - Nested SUBCASE 2")
			{
				INFO("Info from Nested SUBCASE 2!");
				CHECK(false);
			}
		}
	}
}

#pragma once

#include <doctest.h>

inline bool IsEven(int number)
{
	return (number % 2 == 0);
}

TEST_CASE("[SpecialCharactersInFolderPath] - Is Even")
{
	CHECK(IsEven(2));
}

	TEST_CASE("[TabBeforeTestCase] - Is Even")
	{
		CHECK(IsEven(2));
	}

		TEST_CASE("[MultipleTabsBeforeTestCase] - Is Even")
		{
			CHECK(IsEven(2));
		}

 TEST_CASE("[SpaceBeforeTestCase] - Is Even")
 {
 	CHECK(IsEven(2));
 }

   TEST_CASE("[MultipleSpacesBeforeTestCase] - Is Even")
   {
  	   CHECK(IsEven(2));
   }
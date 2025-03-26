# Test Adapter for Doctest

## FAQ
**Test adapter has discovered X amount of tests but I only see Y amount of tests in the test explorer, why?**
This might happen when there are two TEST_CASEs with identical names in the same file, namespace and class. The test explorer can't parse the difference between them so it will end up displaying whatever the last identical TEST_CASE is. In this instance, it would be worth checking if the identical TEST_CASEs should have the exact same names.
![Image of an example in a code base where two TEST_CASEs share an identical name.]()

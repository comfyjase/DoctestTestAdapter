# Test Adapter for Doctest

## FAQ
### **Why has the test adapter discovered X amount of tests but I only see Y amount of tests in the test explorer?**
This might happen when there are TEST_CASEs with identical names in the same file, namespace and class. The test explorer can't parse the difference between them so it will end up displaying whatever the last identical TEST_CASE is. If this happens, it would be worth double checking for any identical TEST_CASEs and seeing if they should have the exact same names.  
![Image of an example in a code base where two TEST_CASEs share an identical name and the test explorer window only displays one of them.](https://github.com/comfyjase/DoctestTestAdapter/blob/8e3d67c4286da29481983a260b843ace6b036540/Assets/Images/faq-test-adapter-discover-more-tests-than-displayed.png)  

### **Why is the Tests window printing "[Warning] No test is available in [filename].dll. Make sure that test discoverer & executors are registered and platform & framework version settings are appropriate and try again." when I have doctest unit tests in [filename].dll?**
If you are loading this `.dll` file in an `.exe` in your project somewhere, then please ignore this warning for now - it shouldn't have any impact on the functionality of this test adapter working for your project. This can print to the Tests window because even though this test adapter supports doctest unit tests in dlls it only discovers `.exe` files so the `.dll` file will remain undiscovered. Any `.exe` files that load your `.dll` file will have your `.dll` doctest unit tests in them in the test explorer window. For example, in the `DoctestTestAdapter.Examples` solution, all of the doctest unit tests from `DLL.dll` will appear under `ExecutableUsingDLL`.

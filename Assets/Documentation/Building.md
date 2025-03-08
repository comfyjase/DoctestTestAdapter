# Test Adapter for Doctest

## Build From Source
This is for people who are working on any tasks/bugs/feature proposals for the test adapter for doctest or if you want to build the test adapter from source directly to produce your own vsix output file.
1. Download the source code for this project.
	* You can use something like GitHub Desktop to download and manage changes for this repository or your own forked repository. You can download GitHub Desktop from [here](https://desktop.github.com/download/).
2. Open `DoctestTestAdapter.sln`.
3. Set `DoctestTestAdapter.VSIX` as the start up project in the solution and then just build and run it. 
	* This will run and install the test adapter in the experimental version of your Visual Studio version so you can test it in a Visual Studio environment but not have to modify your standard environment. E.g. Runs and installs the test adapter in 'Visual Studio Community 2022 Exp', your regular Visual Studio Community 2022 won't have it installed.  
	* If you want to see your own output vsix file: navigate to the bin folder after building e.g. `\DoctestTestAdapter.VSIX\bin\Debug\` and you should be able to find `DoctestTestAdapter.vsix` there to install.

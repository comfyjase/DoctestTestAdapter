# Test Adapter for doctest
Free open source Visual Studio Test Adapter VSIX to search, list, run and debug C++ [doctest](https://github.com/doctest/doctest) unit tests. Implemented using the Microsoft Test Adapter framework [vstest](https://github.com/microsoft/vstest).  

| main branch | dev branch |
|--|--|
| [![CI](https://github.com/comfyjase/DoctestTestAdapter/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/comfyjase/DoctestTestAdapter/actions/workflows/ci.yml) | [![CI](https://github.com/comfyjase/DoctestTestAdapter/actions/workflows/ci.yml/badge.svg?branch=dev)](https://github.com/comfyjase/DoctestTestAdapter/actions/workflows/ci.yml) |  

## Features
* Visual Studio Test Adapter to run and debug C++ doctest unit tests.
* Custom settings for test discovery and execution.
* Exe and DLL support.  

| IDE | Test Discovery | Test Execution | Test Debug |
|---|---|---|---|
| Visual Studio Community 2022 | ✅ | ✅ | ✅ |
| Visual Studio Professional 2022 | ✅ | ✅ | ✅ |
| Visual Studio Enterprise 2022 | ✅ | ✅ | ✅ |  

## Installing Test Adapter for Doctest
Please feel free to install this test adapter by any of the following methods:

### Visual Studio Marketplace
TODO: Step by step instructions for installing via Visual Studio Marketplace.

### Visual Studio Extension Manager
TODO: Step by step instructions for installing via Visual Studio Extension Manager.

### Release
TODO: Step by step instructions for installing via GitHubs releases on this project.

### Developer Builds
This can be useful if you want to use a version of this test adapter built from commits from main or dev branch.
> [!NOTE]  
> Since the dev branch is used for active development there may be stability issues if you use any builds from dev.  
> main branch should always be considered stable.  
> Builds are currently retained for 30 days.  

1. Uninstall any installed versions of this test adapter to avoid any conflicts.
	* You should be able to do this by navigating to the Visual Studio Extension Manager and searching for "Test Adapter for Doctest" and then selecting uninstall.
2. Close down Visual Studio so there are no instances of it running and make sure any uninstall operations finish before continuing.
3. Navigate to the [actions tab](https://github.com/comfyjase/DoctestTestAdapter/actions) for the repo.
4. Find which branch and commit you want to get a build from and click on that workflow run.
	* ![Image of the GitHub repo workflows with a blue arrow pointing to the dev workflow commit name. Blue highlight over the commit hash. Blue highlight over the branch name.]()
5. Scroll down to the artifacts section and click on the download icon for which ever build configuration you want. This should download a zip folder.
	* The name should be formatted like so: `Platform-Configuration-Build Timestamp-Git Branch Name-Shortened Git Commit SHA`. E.g. `Windows-Debug-2025.03.08-13.10.00-dev-cff246a`.
	* You will have to wait for the CI build to finish before the builds will appear (should have a green tick). If the build failed there won't be any artifacts.
	* ![Image of the build artifacts from the development workflow run.]()
6. Extract the folder and you should be able to see the `DoctestTestAdapter.vsix` file. Double click on this to install the test adapter.

## Build From Source
This is for people who are working on any tasks/bugs/feature proposals for the test adapter for doctest or if you want to build the test adapter from source directly to produce your own vsix output file.
1. Download the source code for this project.
	* You can use something like GitHub Desktop to download and manage changes for this repository or your own forked repository. You can download GitHub Desktop from [here](https://desktop.github.com/download/).
2. Open `DoctestTestAdapter.sln`.
3. Set `DoctestTestAdapter.VSIX` as the start up project in the solution and then just build and run it. 
	* This will run and install the test adapter in the experimental version of your Visual Studio version so you can test it in a Visual Studio environment but not have to modify your standard environment. E.g. Runs and installs the test adapter in 'Visual Studio Community 2022 Exp', your regular Visual Studio Community 2022 won't have it installed.  
	* If you want to see your own output vsix file: navigate to the bin folder after building e.g. `\DoctestTestAdapter.VSIX\bin\Debug\` and you should be able to find `DoctestTestAdapter.vsix` there to install.

## Test Coverage
This repository has unit tests in the `DoctestTestAdapter.Tests` project so developers can quickly test their own code changes locally and make sure everything passes before making any commits.
There is also [continuous integration](https://github.com/comfyjase/DoctestTestAdapter/actions/workflows/ci.yml) setup for both main and dev branches as well as pull requests to help ensure stability throughout development.  

## Contributing
* If you notice a bug please report using this form [here](https://github.com/comfyjase/DoctestTestAdapter/issues/new?template=bug_report.yml)
* For suggesting improvements please fill out this form [here](https://github.com/comfyjase/DoctestTestAdapter/issues/new?template=feature_proposal.yml)  

## Thank you!

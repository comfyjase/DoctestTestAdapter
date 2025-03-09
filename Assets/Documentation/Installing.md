# Test Adapter for Doctest

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
	* ![Image of the GitHub repo workflows with a blue arrow pointing to the dev workflow commit name. Blue highlight over the commit hash. Blue highlight over the branch name.](https://github.com/comfyjase/DoctestTestAdapter/blob/21349078505fed4529b9c0120578e127101903e1/Assets/Images/installing-developer-builds-workflow.png)
5. Scroll down to the artifacts section and click on the download icon for which ever build configuration you want. This should download a zip folder.
	* The name should be formatted like so: `Platform-Configuration-Build Timestamp-Git Branch Name-Shortened Git Commit SHA`. E.g. `Windows-Debug-2025.03.08-13.10.00-dev-cff246a`.
	* You will have to wait for the CI build to finish before the builds will appear (should have a green tick). If the build failed there won't be any artifacts.
	* ![Image of the build artifacts from the development workflow run.](https://github.com/comfyjase/DoctestTestAdapter/blob/21349078505fed4529b9c0120578e127101903e1/Assets/Images/installing-developer-builds-artifacts.png)
6. Extract the folder and you should be able to see the `DoctestTestAdapter.vsix` file. Double click on this to install the test adapter.

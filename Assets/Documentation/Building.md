# Test Adapter for Doctest

## Build From Source
This is for people who are working on any tasks/bugs/feature proposals for the test adapter for doctest or if you want to build the test adapter from source directly to produce your own vsix output file.
1. Install python version 3.8+ - either from the [Microsoft Store](https://apps.microsoft.com/detail/9PNRBTZXMB4Z?hl=en-us&gl=GB&ocid=pdpshare) or download any suitable version from the [python website](https://www.python.org/downloads/windows/).
	* Make sure to check *"Add python.exe to PATH"* or remember to manually add the install and scripts folder to your PATH variable.
2. Install pip - In a command prompt window, run `python -m ensurepip --upgrade`.
3. Install scons 4.0+ - In a command prompt window, run `python -m pip install scons`.
4. Download the source code for this project.
	* You can use something like GitHub Desktop to download and manage changes. You can download GitHub Desktop from [here](https://desktop.github.com/download/).
5. Open a command prompt or "cd" into the `DoctestTestAdapter.Examples.Godot` folder and run: `scons platform=windows target=editor dev_build=yes dev_mode=yes vsproj=yes vsproj_gen_only=no`.
	* This may take a few minutes because it's building the godot engine source code.
6. Open a command prompt or "cd" into the `DoctestTestAdapter.Examples.Godot` folder and run `bin\godot.windows.editor.dev.x86_64.console.exe --gdscript-generate-tests modules/gdscript/tests/scripts`.
7. Go into the `DoctestTestAdapter.Examples.Godot` folder and open `godot.sln`.
8. Add a new `.runsettings` file to the solution and copy/paste these settings inside and save:  
```xml
<?xml version="1.0" encoding="utf-8"?>  
<RunSettings>  
	<RunConfiguration>  
		<!-- set default session timeout to 5m -->  
		<TestSessionTimeout>500000</TestSessionTimeout>  
		<TreatNoTestsAsError>true</TreatNoTestsAsError>  
	</RunConfiguration>  
	<Doctest>  
		<GeneralSettings>  
			<CommandArguments>--headless --test</CommandArguments>  
		</GeneralSettings>  
		<DiscoverySettings>  
			<SearchDirectories>  
				<string>modules</string>  
				<string>tests</string>  
			</SearchDirectories>  
		</DiscoverySettings>  
	</Doctest>  
</RunSettings>  
```  
9. Go to *Tests* -> *Configure Run Settings* -> *Select Solution Wide runsettings File* and select the new `.runsettings` file you just created.
	* Doing this during setup so it's prepared for whenever you want to open up the godot example solution for testing any test adapter code changes.
10. Open `DoctestTestAdapter.Godot.sln`.
11. Set `DoctestTestAdapter.VSIX` as the start up project in the solution and then just build and run it.
	* This will run and install the test adapter in the experimental version of your Visual Studio version so you can test it in a Visual Studio environment but not have to modify your standard environment. E.g. Runs and installs the test adapter in 'Visual Studio Community 2022 Exp', your regular Visual Studio Community 2022 won't have it installed.  
	* If you want to see your own output vsix file: navigate to the bin folder after building e.g. `\DoctestTestAdapter.VSIX\bin\Debug\` and you should be able to find `DoctestTestAdapter.vsix` there to install.

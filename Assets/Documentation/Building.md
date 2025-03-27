# Test Adapter for Doctest

## Build From Source
This is for people who are working on any tasks/bugs/feature proposals for the test adapter for doctest or if you want to build the test adapter from source directly to produce your own vsix output file.
1. Install python version 3.8+ - either from the [Microsoft Store](https://apps.microsoft.com/detail/9PNRBTZXMB4Z?hl=en-us&gl=GB&ocid=pdpshare) or download any suitable version from the [python website](https://www.python.org/downloads/windows/).
2. Install pip - In a command prompt window, run `python -m ensurepip --upgrade`.
3. Install scons 4.0+ - In a command prompt window, run `python -m pip install scons`.
4. Download the source code for this project.
	* You can use something like GitHub Desktop to download and manage changes. You can download GitHub Desktop from [here](https://desktop.github.com/download/).
5. Open a command prompt or "cd" into the `DoctestTestAdapter.Examples.Godot` folder and run: `scons platform=windows target=editor dev_build=yes dev_mode=yes vsproj=yes vsproj_gen_only=no`.
	* This may take a few minutes because it's building the godot engine source code.
6. Open a command prompt or "cd" into the `DoctestTestAdapter.Examples.Godot\bin\` folder and run `godot.windows.editor.dev.x86_64.console.exe --gdscript-generate-tests modules/gdscript/tests/scripts`.
7. Open `DoctestTestAdapter.Godot.sln`.
8. Set `DoctestTestAdapter.VSIX` as the start up project in the solution and then just build and run it.
	* This will run and install the test adapter in the experimental version of your Visual Studio version so you can test it in a Visual Studio environment but not have to modify your standard environment. E.g. Runs and installs the test adapter in 'Visual Studio Community 2022 Exp', your regular Visual Studio Community 2022 won't have it installed.  
	* If you want to see your own output vsix file: navigate to the bin folder after building e.g. `\DoctestTestAdapter.VSIX\bin\Debug\` and you should be able to find `DoctestTestAdapter.vsix` there to install.

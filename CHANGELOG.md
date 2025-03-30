# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [0.1.0-Pre-Release] - 2025-03-31

See the [release](https://github.com/comfyjase/DoctestTestAdapter/releases) for binary assets.

### Added

#### Settings
* Custom command arguments setting to be used when discovering/executing discovered executables.
* Print standard output setting to print out more information when the test adapter is performing it's work.
* Search directory setting to only search the listed directories within the solution folder for unit tests.
* Executable override setting to allow users to run a different executable instead of the discovered executable.

#### Discovery
* Discover C++ doctest unit tests for executables and dlls and lists them in the test explorer window.
* Support for finding custom macros that wrap around doctest keywords.
* Jump to tests from the test explorer window.

#### Executor
* Run C++ doctest unit tests for executables and dlls.
* Debug C++ doctest unit tests for executables and dlls.
* Reports duration of tests in the test explorer window.
* Prints any test error messages to the test explorer window.

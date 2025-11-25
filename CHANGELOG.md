# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [0.2.2-Pre-Release] - 2025-11-25

See the [release](https://github.com/comfyjase/DoctestTestAdapter/releases) for binary assets.

### Fixed

#### Discovery
* Fixed issue with solution file not being discovered in subfolders.
* Fixed additional substring errors when searching for keywords.

### Added

#### Settings
* Added new optional RootDirectory setting which can be used when searching for source code files.

## [0.2.1-Pre-Release] - 2025-11-21

See the [release](https://github.com/comfyjase/DoctestTestAdapter/releases) for binary assets.

### Fixed

#### Discovery
* Fixed regex for finding keywords in source code to allow for any leading whitespace character(s).

## [0.2.0-Pre-Release] - 2025-11-17

See the [release](https://github.com/comfyjase/DoctestTestAdapter/releases) for binary assets.

### Fixed

#### Discovery
* Fixed issue missing tests inside of .hpp and .cpp files.
* Fixed substring error when checking for doctest output.

### Removed

#### Discovery
* Removed log output when a new custom TEST_CASE macro is discovered.

### Added

#### Executor
* Support for printing doctest message macro output to the text explorer window.

## [0.1.1-Pre-Release] - 2025-04-29

See the [release](https://github.com/comfyjase/DoctestTestAdapter/releases) for binary assets.

### Fixed 

#### Discovery
* Fixed issues with out of range index for substring for namespace/class keyword checks when searching source files.
* Fixed issue with custom macro keywords checking for custom namespace/class keywords.

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

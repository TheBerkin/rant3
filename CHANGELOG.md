# Rant Changelog

## v3.0.5 - 16 August, 2017

### Features
- Add `RantEngine.GetLoadedPackages()`
- Add `RantEngine.GetLoadedProgramNames()`

### Bugfixes
- Fixed stream not being closed in `RantProgram.SaveToFile` _(Thanks to Bomb Mask on Discord for noticing this)_
- Fixed .rantpgm files not being picked up by RCT's packer _(Thanks Nosteme!)_

## v3.0.4 - 21 July, 2017

### Features
- Add indexer to `RantDictionaryTable` to allow entries to be directly accessed by index
- Add `[hastable]` function to determine if a table exists by name
- Add `[hasclass]` function to determine if a table has a specific class


### Improvements
- Add missing German localization for "exceeded character limit" message


## v3.0.3 - 5 July, 2017

### Bugfixes
- Fixed completely spaghettified subroutine backend that could potentially scramble argument order. Ended up refactoring the whole thing. Good lord that was bad.

### Changes
- Improved memory usage of `RantObject`

## v3.0.2 - 4 July, 2017

### Features
- Rant now suggests closest function name when user tries to call one that doesn't exist.

## v3.0.1 - 16 June, 2017

### Features
- Add `[qexists]` function

### Bugfixes
- Fixed typo in "linsv" function name
- Rename `[qreset]` to its proper name, `[qdel]`

## v3.0.0 - 18 April, 2017

### Features

- Compiled patterns can now be saved as `.rantpgm` files
- Compiler can now emit more than one compiler error
- Dynamic queries
- Plural subtype for queries
- Phrasal complement for queries
- Verbose character literals
- Escape sequences now suppoer surrogate pairs, e.g. emoji
- Patterns can now be passed arguments via `RantProgramArgs` class
- Synchronizers have three new modes: `ping`, `pong`, and `no-repeat`
- `RantFormat` class now supports custom alphabets, number verbalizers, and spaces
- German formatter and localization for error messages
- Brand-new command-line tools for compiling, packaging, and more
- Completely revamped variable system integrated straight into the Rant language
- Block output redirection via `[pipe]` function
- A lot of new string manipulation and formatting functions

### Improvements
- Query class filters are significantly faster
- Packages can now stores all kinds of crazy metadata, like title, description, authors, and more!
- Packages can have dependencies, which are automatically resolved at load time
- Compiler optimizations galore!
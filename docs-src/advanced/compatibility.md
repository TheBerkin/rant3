Since Rant is such a large language with many specialized functions, we created a standard for developers to follow
when creating an implementation of the Rant compiler and runtime. The purpose of this standard is to enable developers
to easily determine the feature set essential to their particular usage case without the confusion of wondering which
features are more integral to the core functionality of the library.

The **Rant Standard Compatibility Levels** (RSCL) lists feature sets, separated into ranks,
that are classified according to their applicability to different usage cases.

## RSCL-10

RSCL-10 is the bare minimum feature set that all Rant implementations should support. It includes the bare essential language features,
basic block behaviors, basic string manipulation, and all block attributes except for synchronizers.

### Requirements

This standard contains basic features of Rant for branching, block attributes, basic formatting, and output manipulation.

* **Comments**
* **Unweighted blocks**
* **Escape sequences**
* **Verbatim strings**
* **Replacers**
* **Function subset:**
    - `[after]`
    - `[before]`
    - `[case]`
    - `[chan]`
    - `[end]`
    - `[ends]`
    - `[even]`
    - `[first]`
    - `[group]`
    - `[item]`
    - `[last]`
    - `[match]`
    - `[middle]`
    - `[notfirst]`
    - `[notlast]`
    - `[notnth]`
    - `[notntho]`
    - `[nth]`
    - `[ntho]`
    - `[num]`
    - `[odd]`
    - `[protect]`
    - `[redirect]`
    - `[rep]`
    - `[repcount]`
    - `[repeach]`
    - `[repelapsed]`
    - `[repnum]`
    - `[repqueued]`
    - `[reprem]`
    - `[rs]`
    - `[sep]`
    - `[start]`
    - `[yield]`

## RSCL-50

RSCL-50 adds basic query functionality, argument support, RNG manipulation, formatting functions, subroutines, and more advanced block behaviors.

### Requirements

**ALL** RSCL-10 features plus the following:

* **Weighted blocks**
* **Subroutines**
* **Query feature subset:**
    - Name, subtype, plural subtype
    - Class filters (positive/negative)
* **Function subset:**
    - `[branch]`
    - `[capsinfer]`
    - `[define]`
    - `[digits]`
    - `[else]`
    - `[endian]`
    - `[ifdef]`
    - `[ifndef]`
    - `[in]`
    - `[merge]`
    - `[numfmt]`
    - `[quot]`
    - `[sync]`
    - `[then]`
    - `[txtfmt]`
    - `[undef]`
    - `[xdel]`
    - `[xpin]`
    - `[xreset]`
    - `[xstep]`
    - `[xunpin]`

## RSCL-75

RSCL-75 adds targets, full query functionality, and more granular character processing.

### Requirements

**ALL** RSCL-50 features plus the following:

* **Verbose characters**
* **ALL query features**
* **Function subset:**
    - `[target]`
    - `[send]`
    - `[sendover]`
    - `[targetval]`
    - `[clrt]`
    - `[char]`
    - `[rev]`
    - `[revx]`
    - `[accent]`

## RSCL-100

RSCL-100 encompasses the full feature set of the Rant language, including all functions, language features,
and constructs.

### Requirements

**ALL** RSCL-75 features plus the following:

* **ALL functions**
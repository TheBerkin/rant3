# Function reference

## abbr

**Overloads:** 1

### [abbr: value]

Abbreviates the specified string.

**Parameters**

|Name|Type|Description|
|---|---|---|
|value|String|The string to abbreviate.|

***
## accent

**Overloads:** 2

### [accent: accent]

Accents the previous character.

**Parameters**

|Name|Type|Description|
|---|---|---|
|accent|Mode|<ul><li><b>acute</b><br/></li><li><b>grave</b><br/></li><li><b>circumflex</b><br/></li><li><b>tilde</b><br/></li><li><b>ring</b><br/></li><li><b>diaeresis</b><br/></li><li><b>caron</b><br/></li><li><b>macron</b><br/></li></ul>|

***
### [accent: character; accent]

Accents the specified character.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|
|accent|Mode|<ul><li><b>acute</b><br/></li><li><b>grave</b><br/></li><li><b>circumflex</b><br/></li><li><b>tilde</b><br/></li><li><b>ring</b><br/></li><li><b>diaeresis</b><br/></li><li><b>caron</b><br/></li><li><b>macron</b><br/></li></ul>|

***
## acute

**Aliases:** `act`<br/>**Overloads:** 1

### [acute: character]

Accents the specified character with an acute (á) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## add

**Overloads:** 1

### [add: a; b]

Prints the num of the specified values.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|The first operand.|
|b|Number|The second operand.|

***
## after

**Overloads:** 1

### [after: after-action]

Sets the postfix pattern for the next block.

**Parameters**

|Name|Type|Description|
|---|---|---|
|after-action|Pattern|The pattern to run after each iteration of the next block.|

***
## and

**Overloads:** 1

### [and: a; b...]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Boolean|*No description*|
|b...|Boolean|*No description*|

***
## arg

**Overloads:** 1

### [arg: name]

Returns the specified argument from the current subroutine.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the argument to retrieve.|

***
## at

**Overloads:** 1

### [at: input; pos]

Prints the character at the specified position in the input. Throws an exception if the position is outside of the string.

**Parameters**

|Name|Type|Description|
|---|---|---|
|input|String|The input string.|
|pos|Number|The position of the character to find.|

***
## b

**Overloads:** 1

### [b]

Prints a bullet character.
***
## before

**Overloads:** 1

### [before: before-action]

Sets the prefix pattern for the next block.

**Parameters**

|Name|Type|Description|
|---|---|---|
|before-action|Pattern|The pattern to run before each iteration of the next block.|

***
## branch

**Overloads:** 2

### [branch: seed]

Branches the internal RNG according to a seed.

**Parameters**

|Name|Type|Description|
|---|---|---|
|seed|String|The seed for the branch.|

***
### [branch: seed; branch-action]

Branches the internal RNG, executes the specified pattern, and then merges the branch.

**Parameters**

|Name|Type|Description|
|---|---|---|
|seed|String|The seed for the branch.|
|branch-action|Pattern|The pattern to run on the branch.|

***
## c

**Overloads:** 1

### [c]

Prints the copyright symbol.
***
## capsinfer

**Overloads:** 1

### [capsinfer: sample]

Infers the capitalization of a given string and sets the capitalization mode to match it.

**Parameters**

|Name|Type|Description|
|---|---|---|
|sample|String|A string that is capitalized in the format to be set.|

***
## caron

**Aliases:** `crn`<br/>**Overloads:** 1

### [caron: character]

Accents the specified character with a caron (č) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## case

**Aliases:** `caps`<br/>**Overloads:** 1

### [case: mode]

Changes the capitalization mode for all open channels.

**Parameters**

|Name|Type|Description|
|---|---|---|
|mode|Mode|The capitalization mode to use.<br/><br/><ul><li><b>none</b><br/>No capitalization.</li><li><b>lower</b><br/>Convert to lowercase.</li><li><b>upper</b><br/>Convert to uppercase.</li><li><b>title</b><br/>Convert to title case.</li><li><b>first</b><br/>Capitalize the first letter.</li><li><b>sentence</b><br/>Capitalize the first letter of every sentence.</li><li><b>word</b><br/>Capitalize the first letter of every word.</li></ul>|

***
## cedilla

**Aliases:** `ced`<br/>**Overloads:** 1

### [cedilla: character]

Accents the specified character with a cedilla (ç) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## chan

**Overloads:** 1

### [chan: channel-name; visibility; pattern]

Opens a channel for writing and executes the specified pattern inside of it.

**Parameters**

|Name|Type|Description|
|---|---|---|
|channel-name|String|*No description*|
|visibility|Mode|<ul><li><b>public</b><br/>Channel outputs to itself and 'main'.</li><li><b>private</b><br/>Channel outputs only to itself.</li><li><b>internal</b><br/>Channel outputs only to itself and all immediate parent channels also set to Internal.</li></ul>|
|pattern|Pattern|*No description*|

***
## chance

**Overloads:** 1

### [chance: chance]

Modifies the likelihood that the next block will execute. Specified in percentage.

**Parameters**

|Name|Type|Description|
|---|---|---|
|chance|Number|The percent probability that the next block will execute.|

***
## char

**Overloads:** 1

### [char: name]

Prints a Unicode character given its official Unicode-designated name (e.g. 'LATIN CAPITAL LETTER R' -> 'R').

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the character to print (case-insensitive).|

***
## chlen

**Overloads:** 1

### [chlen: channel-name]

Prints the current length of the specified channel, in characters.

**Parameters**

|Name|Type|Description|
|---|---|---|
|channel-name|String|The channel for which to retrieve the length.|

***
## circumflex

**Aliases:** `cflex`<br/>**Overloads:** 1

### [circumflex: character]

Accents the specified character with a circumflex (â) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## clrt

**Overloads:** 1

### [clrt: target-name]

Clears the contents of the specified target.

**Parameters**

|Name|Type|Description|
|---|---|---|
|target-name|String|The name of the target to be cleared.|

***
## define

**Overloads:** 1

### [define: flags...]

Defines the specified flags.

**Parameters**

|Name|Type|Description|
|---|---|---|
|flags...|String|The list of flags to define.|

***
## depth

**Overloads:** 1

### [depth]

Prints the number of currently active blocks.
***
## diaeresis

**Aliases:** `dia`<br/>**Overloads:** 1

### [diaeresis: character]

Accents the specified character with a diaeresis (ä) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## digits

**Overloads:** 1

### [digits: format; digits]

Specifies the current digit formatting mode for numbers.

**Parameters**

|Name|Type|Description|
|---|---|---|
|format|Mode|The digit format to use.<br/><br/><ul><li><b>normal</b><br/>Use as many digits as necessary to accomodate the number.</li><li><b>pad</b><br/>Pad numbers to a specific number of digits.</li><li><b>truncate</b><br/>Truncate numbers over a specific number of digits.</li></ul>|
|digits|Number|The digit count to associate with the mode.|

***
## div

**Overloads:** 1

### [div: a; b]

Prints the quotient of the two specified numbers.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|The dividend.|
|b|Number|The divisor.|

***
## else

**Overloads:** 1

### [else: condition-fail-pattern]

Executes a pattern if the current flag condition fails.

**Parameters**

|Name|Type|Description|
|---|---|---|
|condition-fail-pattern|Pattern|*No description*|

***
## em

**Overloads:** 1

### [em]

Prints an emdash.
***
## emoji

**Overloads:** 1

### [emoji: shortcode]

Takes an emoji shortcode and prints the corresponding emoji.

**Parameters**

|Name|Type|Description|
|---|---|---|
|shortcode|String|The emoji shortcode to use, without colons.|

***
## en

**Overloads:** 1

### [en]

Prints an endash.
***
## end

**Overloads:** 1

### [end: end-pattern]

Sets a pattern that will run after the next block.

**Parameters**

|Name|Type|Description|
|---|---|---|
|end-pattern|Pattern|The pattern to run after the next block.|

***
## endian

**Overloads:** 1

### [endian: endianness]

Sets the current endianness for hex and binary formatted numbers.

**Parameters**

|Name|Type|Description|
|---|---|---|
|endianness|Mode|The endianness to use.<br/><br/><ul><li><b>big</b><br/>Big endian.</li><li><b>little</b><br/>Little endian.</li><li><b>default</b><br/>Whatever endianness your system uses.</li></ul>|

***
## ends

**Overloads:** 1

### [ends: action]

Runs a pattern if the current block iteration is either the first or last.

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|Pattern|The pattern to run when the condition is met.|

***
## eq

**Overloads:** 1

### [eq: a; b]

Prints a boolean value indicating whether the two values have equal string representations.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|*No description*|
|b|String|*No description*|

***
## eqi

**Overloads:** 1

### [eqi: a; b]

Prints a boolean value indicating whether the two values have equal string representations, ignoring case.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|*No description*|
|b|String|*No description*|

***
## even

**Overloads:** 1

### [even: action]

Runs a pattern if the current block iteration is an even number.

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|Pattern|The pattern to run when the condition is met.|

***
## first

**Overloads:** 1

### [first: action]

Runs a pattern if the current block iteration is the first.

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|Pattern|The pattern to run when the condition is met.|

***
## ge

**Overloads:** 1

### [ge: a; b]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|*No description*|
|b|Number|*No description*|

***
## grave

**Aliases:** `grv`<br/>**Overloads:** 1

### [grave: character]

Accents the specified character with a grave (à) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## group

**Overloads:** 1

### [group: group-name]

Retrieves and prints the specified group value of the current match from the active replacer.

**Parameters**

|Name|Type|Description|
|---|---|---|
|group-name|String|The name of the match group whose value will be retrieved.|

***
## gt

**Overloads:** 1

### [gt: a; b]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|*No description*|
|b|Number|*No description*|

***
## if

**Overloads:** 2

### [if: condition; body]



**Parameters**

|Name|Type|Description|
|---|---|---|
|condition|Boolean|*No description*|
|body|Pattern|*No description*|

***
### [if: condition; body; else-body]



**Parameters**

|Name|Type|Description|
|---|---|---|
|condition|Boolean|*No description*|
|body|Pattern|*No description*|
|else-body|Pattern|*No description*|

***
## ifdef

**Overloads:** 1

### [ifdef: flags...]

Sets the current flag condition for [then] ... [else] calls to be true if all the specified flags are set.

**Parameters**

|Name|Type|Description|
|---|---|---|
|flags...|String|*No description*|

***
## ifndef

**Overloads:** 1

### [ifndef: flags...]

Sets the current flag condition for [then] ... [else] calls to be true if all the specified flags are unset.

**Parameters**

|Name|Type|Description|
|---|---|---|
|flags...|String|*No description*|

***
## ifnot

**Aliases:** `ifn`<br/>**Overloads:** 2

### [ifnot: condition; body]



**Parameters**

|Name|Type|Description|
|---|---|---|
|condition|Boolean|*No description*|
|body|Pattern|*No description*|

***
### [ifnot: condition; body; else-body]



**Parameters**

|Name|Type|Description|
|---|---|---|
|condition|Boolean|*No description*|
|body|Pattern|*No description*|
|else-body|Pattern|*No description*|

***
## in

**Overloads:** 1

### [in: arg-name]

Prints the value of the specified pattern argument.

**Parameters**

|Name|Type|Description|
|---|---|---|
|arg-name|String|The name of the argument to access.|

***
## index

**Aliases:** `i`<br/>**Overloads:** 1

### [index]

Prints the zero-based index of the block item currently being executed.
***
## index1

**Aliases:** `i1`<br/>**Overloads:** 1

### [index1]

Prints the one-based index of the block item currently being executed.
***
## init

**Overloads:** 1

### [init: index]

Sets the index of the element to execute on the next block. Set to -1 to disable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|index|Number|*No description*|

***
## item

**Overloads:** 2

### [item]

Prints the main output from the current block iteration.
***
### [item: channel]

Prints the specified channel from the current block iteration.

**Parameters**

|Name|Type|Description|
|---|---|---|
|channel|String|The output channel to print from.|

***
## join

**Overloads:** 2

### [join: list-obj]

Joins the specified list into a string.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|The list to join.|

***
### [join: list-obj; delimiter]

Joins the specified list into a string seperated by the delimiter and returns it.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|The list to join.|
|delimiter|String|The delimiter.|

***
## ladd

**Aliases:** `ladds`<br/>**Overloads:** 1

### [ladd: list-obj; values...]

Adds one or more strings to a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|The list to add to.|
|values...|String|The strings to add.|

***
## laddn

**Overloads:** 1

### [laddn: list-obj; values...]

Adds one or more numbers to a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|The list to add to.|
|values...|Number|The numbers to add.|

***
## laddp

**Overloads:** 1

### [laddp: list-obj; values...]

Adds one or more patterns to a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|The list to add to.|
|values...|Pattern|The patterns to add.|

***
## laddv

**Overloads:** 1

### [laddv: list-obj; values...]

Adds one or more variables to a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|The list to add to.|
|values...|RantObject|The variables to add.|

***
## last

**Overloads:** 1

### [last: action]

Runs a pattern if the current block iteration is the last.

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|Pattern|The pattern to run when the condition is met.|

***
## lclone

**Overloads:** 1

### [lclone: list-obj; variable]

Clones a list to another variable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|variable|String|*No description*|

***
## lclr

**Overloads:** 1

### [lclr: list-obj]

Clears the specified list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|

***
## lcpy

**Overloads:** 1

### [lcpy: list-obj; index; variable]

Copies an item from a list into a variable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|variable|String|*No description*|

***
## le

**Overloads:** 1

### [le: a; b]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|*No description*|
|b|Number|*No description*|

***
## len

**Overloads:** 1

### [len: str]

Gets the length of the specified string.

**Parameters**

|Name|Type|Description|
|---|---|---|
|str|String|The string to measure.|

***
## lfilter

**Overloads:** 2

### [lfilter: list-name; varname; condition]

Filters out elements of a list when the condition returns false. Mutates list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-name|String|The name of the list object to filter.|
|varname|String|The name of the variable that will contain the current item within the condition.|
|condition|Pattern|The condition that will be checked for each item.|

***
### [lfilter: list-name; output-list-name; varname; condition]

Filters out elements of a list when the condition returns false. Creates new list with results.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-name|String|The name of the list object to filter.|
|output-list-name|String|The name of the list that will contain the filtered result.|
|varname|String|The name of the variable that will contain the current item within the condition.|
|condition|Pattern|The condition that will be checked for each item.|

***
## lfind

**Overloads:** 1

### [lfind: list-obj; value]

Searches a list for the specified value and prints the index if found. Otherwise, prints -1.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|value|String|*No description*|

***
## lfindi

**Overloads:** 1

### [lfindi: list-obj; value]

Searches a list for the specified value, ignoring case, and prints the index if found. Otherwise, prints -1.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|value|String|*No description*|

***
## lfindv

**Overloads:** 1

### [lfindv: list-obj; value]

Searches a list for the specified variable and prints the index if found. Otherwise, prints -1.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|value|RantObject|*No description*|

***
## lget

**Overloads:** 1

### [lget: list-obj; index]

Prints a list item from the specified index.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|

***
## lins

**Overloads:** 1

### [lins: list-obj; index; value]

Inserts a string at the specified index in a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|value|String|*No description*|

***
## linsn

**Overloads:** 1

### [linsn: list-obj; index; value]

Inserts a number at the specified index in a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|value|Number|*No description*|

***
## linsp

**Overloads:** 1

### [linsp: list-obj; index; value]

Inserts a pattern at the specified index in a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|value|Pattern|*No description*|

***
## linsv

**Overloads:** 1

### [linsv: list-obj; index; value]

Inserts a variable at the specified index in a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|value|RantObject|*No description*|

***
## lmap

**Overloads:** 2

### [lmap: list-name; varname; body]

Replaces each item in the input list with its value when run through body.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-name|String|The name of the list object to map.|
|varname|String|The name of the variable that will contain the current item within the body.|
|body|Pattern|The body that will be run for each item.|

***
### [lmap: list-name; output-list-name; varname; body]

Runs each item in the input list through the body and adds results to output list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-name|String|The name of the list object to map.|
|output-list-name|String|The name of the list that will contain the mapped result.|
|varname|String|The name of the variable that will contain the current item within the body.|
|body|Pattern|The body that will be run for each item.|

***
## lpop

**Overloads:** 1

### [lpop: list-obj]

Removes the last item from a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|

***
## lpopf

**Overloads:** 1

### [lpopf: list-obj]

Removes the first item from a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|

***
## lpre

**Aliases:** `lpres`<br/>**Overloads:** 1

### [lpre: list-obj; value]

Prepends a string to a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|value|String|*No description*|

***
## lpren

**Overloads:** 1

### [lpren: list-obj; value]

Prepends a number to a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|value|Number|*No description*|

***
## lprep

**Overloads:** 1

### [lprep: list-obj; value]

Prepends a pattern to a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|value|Pattern|*No description*|

***
## lrand

**Overloads:** 1

### [lrand: obj]

Returns a random value from the specified list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|obj|RantObject|The list to pick from.|

***
## lset

**Overloads:** 1

### [lset: list-obj; index; value]

Sets the item at a specified index in a list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|value|String|*No description*|

***
## lsetn

**Overloads:** 1

### [lsetn: list-obj; index; value]

Sets the item at a specified index in a list to a number.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|value|Number|*No description*|

***
## lsetp

**Overloads:** 1

### [lsetp: list-obj; index; value]

Sets the item at a specified index in a list to a pattern.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|value|Pattern|*No description*|

***
## lsetv

**Overloads:** 1

### [lsetv: list-obj; index; value]

Sets the item at a specified index in a list to a variable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-obj|RantObject|*No description*|
|index|Number|*No description*|
|value|RantObject|*No description*|

***
## lt

**Overloads:** 1

### [lt: a; b]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|*No description*|
|b|Number|*No description*|

***
## macron

**Aliases:** `mcn`<br/>**Overloads:** 1

### [macron: character]

Accents the specified character with a macron (c̄) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## match

**Overloads:** 1

### [match]

Retrieves and prints the current match string of the active replacer.
***
## maybe

**Overloads:** 1

### [maybe]


***
## merge

**Overloads:** 1

### [merge]

Merges the topmost branch of the internal RNG, if it has been branched at least once.
***
## middle

**Overloads:** 1

### [middle: action]

Runs a pattern if the current block iteration is neither the first nor last.

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|Pattern|The pattern to run when the condition is met.|

***
## mod

**Overloads:** 1

### [mod: a; b]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|*No description*|
|b|Number|*No description*|

***
## mul

**Overloads:** 1

### [mul: a; b]

Prints the product of the specified numbers.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|The first operand.|
|b|Number|The second operand.|

***
## nand

**Overloads:** 1

### [nand: a; b...]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Boolean|*No description*|
|b...|Boolean|*No description*|

***
## ne

**Overloads:** 1

### [ne: a; b]

Prints a boolean value indicating whether the two values do not have equal string representations.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|*No description*|
|b|String|*No description*|

***
## nei

**Overloads:** 1

### [nei: a; b]

Prints a boolean value indicating whether two values do not have equal string representations, ignoring case.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|*No description*|
|b|String|*No description*|

***
## not

**Overloads:** 1

### [not: a]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Boolean|*No description*|

***
## notfirst

**Overloads:** 1

### [notfirst: action]

Runs a pattern if the current block iteration is not the first.

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|Pattern|The pattern to run when the condition is met.|

***
## notlast

**Overloads:** 1

### [notlast: action]

Runs a pattern if the current block iteration is not the last.

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|Pattern|The pattern to run when the condition is met.|

***
## notnth

**Overloads:** 1

### [notnth: interval; pattern]

Runs a pattern if the current block iteration is not a multiple of the specified number.

**Parameters**

|Name|Type|Description|
|---|---|---|
|interval|Number|The interval at which the pattern should not be run.|
|pattern|Pattern|The pattern to run when the condition is satisfied.|

***
## notntho

**Overloads:** 1

### [notntho: interval; offset; pattern]

Runs a pattern if the current block iteration is not a multiple of the specified number offset by a specific amount.

**Parameters**

|Name|Type|Description|
|---|---|---|
|interval|Number|The interval at which the pattern should not be run.|
|offset|Number|The number of iterations to offset the interval by.|
|pattern|Pattern|The pattern to run when the condition is satisfied.|

***
## nth

**Overloads:** 1

### [nth: interval; pattern]

Runs a pattern if the current block iteration is a multiple of the specified number.

**Parameters**

|Name|Type|Description|
|---|---|---|
|interval|Number|The interval at which the pattern should be run.|
|pattern|Pattern|The pattern to run when the condition is satisfied.|

***
## ntho

**Overloads:** 1

### [ntho: interval; offset; pattern]

Runs a pattern if the current block iteration is a multiple of the specified number offset by a specific amount.

**Parameters**

|Name|Type|Description|
|---|---|---|
|interval|Number|The interval at which the pattern should be run.|
|offset|Number|The number of iterations to offset the interval by.|
|pattern|Pattern|The pattern to run when the condition is satisfied.|

***
## num

**Aliases:** `n`<br/>**Overloads:** 2

### [num: input]

Formats an input string using the current number format settings and prints the result.

**Parameters**

|Name|Type|Description|
|---|---|---|
|input|String|The string to convert into a number.|

***
### [num: min; max]

Prints a random number between the specified minimum and maximum bounds.

**Parameters**

|Name|Type|Description|
|---|---|---|
|min|Number|The minimum value of the number to generate.|
|max|Number|The maximum value of the number to generate.|

***
## numfmt

**Overloads:** 2

### [numfmt: format]

Sets the current number formatting mode.

**Parameters**

|Name|Type|Description|
|---|---|---|
|format|Mode|The number format to use.<br/><br/><ul><li><b>normal</b><br/>No special formatting.</li><li><b>group</b><br/>Group digits with the system's digit separator.</li><li><b>group-commas</b><br/>Group digits by commas.</li><li><b>group-dots</b><br/>Group digits by dots.</li><li><b>roman</b><br/>Uppercase Roman numerals.</li><li><b>roman-upper</b><br/>Uppercase Roman numerals.</li><li><b>roman-lower</b><br/>Lowercase Roman numerals.</li><li><b>verbal</b><br/>Number verbalization. Only works with integers.</li><li><b>hex</b><br/>Uppercase hexadecimal.</li><li><b>hex-upper</b><br/>Uppercase hexadecimal.</li><li><b>hex-lower</b><br/>Lowercase hexadecimal.</li><li><b>binary</b><br/>Robot language.</li></ul>|

***
### [numfmt: format; range-action]

Runs the specified pattern under a specific number formatting mode.

**Parameters**

|Name|Type|Description|
|---|---|---|
|format|Mode|The number format to use.<br/><br/><ul><li><b>normal</b><br/>No special formatting.</li><li><b>group</b><br/>Group digits with the system's digit separator.</li><li><b>group-commas</b><br/>Group digits by commas.</li><li><b>group-dots</b><br/>Group digits by dots.</li><li><b>roman</b><br/>Uppercase Roman numerals.</li><li><b>roman-upper</b><br/>Uppercase Roman numerals.</li><li><b>roman-lower</b><br/>Lowercase Roman numerals.</li><li><b>verbal</b><br/>Number verbalization. Only works with integers.</li><li><b>hex</b><br/>Uppercase hexadecimal.</li><li><b>hex-upper</b><br/>Uppercase hexadecimal.</li><li><b>hex-lower</b><br/>Lowercase hexadecimal.</li><li><b>binary</b><br/>Robot language.</li></ul>|
|range-action|Pattern|The pattern to run.|

***
## odd

**Overloads:** 1

### [odd: action]

Runs a pattern if the current block iteration is an odd number.

**Parameters**

|Name|Type|Description|
|---|---|---|
|action|Pattern|The pattern to run when the condition is met.|

***
## or

**Overloads:** 1

### [or: a; b...]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Boolean|*No description*|
|b...|Boolean|*No description*|

***
## pipe

**Overloads:** 1

### [pipe: redirect-callback]

Redirects the output from the next block into the specified callback. Access block output with [item].

**Parameters**

|Name|Type|Description|
|---|---|---|
|redirect-callback|Pattern|The callback to redirect block output to.|

***
## plural

**Aliases:** `pl`<br/>**Overloads:** 1

### [plural: word]

Infers and prints the plural form of the specified word.

**Parameters**

|Name|Type|Description|
|---|---|---|
|word|String|*No description*|

***
## protect

**Overloads:** 1

### [protect: pattern]

Spawns a new block attribute context for the specified callback so any blocks therein will not consume the current attributes.

**Parameters**

|Name|Type|Description|
|---|---|---|
|pattern|Pattern|The callback to protect.|

***
## qcc

**Overloads:** 1

### [qcc: id; component-id; component-type]

Adds a carrier component to a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|component-id|String|The ID to assign to the carrier component.|
|component-type|Mode|The component type.<br/><br/><ul><li><b>match</b><br/></li><li><b>dissociative</b><br/></li><li><b>match-dissociative</b><br/></li><li><b>associative</b><br/></li><li><b>match-associative</b><br/></li><li><b>divergent</b><br/></li><li><b>match-divergent</b><br/></li><li><b>relational</b><br/></li><li><b>match-relational</b><br/></li><li><b>unique</b><br/></li><li><b>match-unique</b><br/></li><li><b>rhyme</b><br/></li></ul>|

***
## qcf

**Overloads:** 1

### [qcf: id; classes...]

Adds positive class filters to a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|classes...|String|The names of the classes that the returned entry must belong to.|

***
## qcfn

**Overloads:** 1

### [qcfn: id; classes...]

Adds negative class filters to a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|classes...|String|The names of the classes that the returned entry must not belong to.|

***
## qdel

**Overloads:** 1

### [qdel: id]

Removes all stored data associated with the specified constructed query ID.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|

***
## qexists

**Overloads:** 1

### [qexists: id]

Prints a boolean value indicating whether a constructed query with the specified ID exists.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|

***
## qhas

**Overloads:** 1

### [qhas: id; regex-pattern; options]

Adds a positive regex filter to a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|regex-pattern|String|The regex pattern for the filter.|
|options|String|The regex option string for the filter.|

***
## qhasno

**Overloads:** 1

### [qhasno: id; regex-pattern; options]

Adds a positive regex filter to a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|regex-pattern|String|The regex pattern for the filter.|
|options|String|The regex option string for the filter.|

***
## qname

**Overloads:** 1

### [qname: id; name]

Sets the table name for a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|name|String|The name of the table.|

***
## qphr

**Overloads:** 1

### [qphr: id; complement]

Adds a phrasal complement to a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|complement|Pattern|The phrasal complement pattern.|

***
## qsub

**Overloads:** 1

### [qsub: id; subtype]

Sets the subtype for a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|subtype|String|The subtype of the term to select from the returned entry.|

***
## qsubp

**Overloads:** 1

### [qsubp: id; plural-subtype]

Sets the plural subtype for a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|plural-subtype|String|The subtype of the term to select from the returned entry, if the plural flag is set.|

***
## qsyl

**Overloads:** 2

### [qsyl: id; syllables]

Adds an syllable count range filter to a constructed query that defines an absolute syllable count.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|syllables|Number|The number of syllables.|

***
### [qsyl: id; min-syllables; max-syllables]

Adds a syllable count range filter to a constructed query.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|min-syllables|Number|The minimum syllable count.|
|max-syllables|Number|The maximum syllable count.|

***
## qsylmax

**Overloads:** 1

### [qsylmax: id; max-syllables]

Adds a syllable count range filter to a constructed query that defines only a maximum bound.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|max-syllables|Number|The maximum syllable count.|

***
## qsylmin

**Overloads:** 1

### [qsylmin: id; min-syllables]

Adds a syllable count range filter to a constructed query that defines only a minimum bound.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|
|min-syllables|Number|The minimum syllable count.|

***
## query

**Aliases:** `q`<br/>**Overloads:** 2

### [query]

Runs the last-accessed constructed query.
***
### [query: id]

Runs the constructed query with the specified identifier.

**Parameters**

|Name|Type|Description|
|---|---|---|
|id|String|The ID string for the constructed query.|

***
## quote

**Aliases:** `quot`<br/>**Overloads:** 1

### [quote: quote-action]

Surrounds the specified pattern in quotes. Nested quotes use the secondary quotes defined in the format settings.

**Parameters**

|Name|Type|Description|
|---|---|---|
|quote-action|Pattern|The pattern to run whose output will be surrounded in quotes.|

***
## rcc

**Overloads:** 1

### [rcc: ids...]

Resets the specified carrier components.

**Parameters**

|Name|Type|Description|
|---|---|---|
|ids...|String|The list of carrier component identifiers to reset.|

***
## reg

**Overloads:** 1

### [reg]

Prints the registered trademark symbol.
***
## rep

**Aliases:** `r`<br/>**Overloads:** 1

### [rep: times]

Sets the repetition count for the next block.

**Parameters**

|Name|Type|Description|
|---|---|---|
|times|Number|The number of times to repeat the next block.|

***
## repcount

**Aliases:** `rc`<br/>**Overloads:** 1

### [repcount]

Prints the repetition count of the current block.
***
## repeach

**Overloads:** 1

### [repeach]

Sets the repetition count to the number of items in the next block.
***
## repelapsed

**Aliases:** `re`<br/>**Overloads:** 1

### [repelapsed]

Prints the number of iterations remaining to be performed on the current block.
***
## repnum

**Aliases:** `rn`<br/>**Overloads:** 1

### [repnum]

Prints the iteration number of the current block.
***
## repqueued

**Aliases:** `rq`<br/>**Overloads:** 1

### [repqueued]

Prints the number of repetitions remaining to be completed on the current block.
***
## reprem

**Aliases:** `rr`<br/>**Overloads:** 1

### [reprem]

Prints the number of remaining repetitions queued after the current one.
***
## require

**Overloads:** 1

### [require: name]

Loads and runs a pattern from cache or file.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name or path of the pattern to load.|

***
## rev

**Overloads:** 1

### [rev: input]

Reverses the specified string and prints it to the output.

**Parameters**

|Name|Type|Description|
|---|---|---|
|input|String|The string to reverse.|

***
## revx

**Overloads:** 1

### [revx: input]

Reverses the specified string and inverts common brackets and quotation marks, then prints the result to the output.

**Parameters**

|Name|Type|Description|
|---|---|---|
|input|String|The string to reverse.|

***
## rhyme

**Overloads:** 1

### [rhyme: flags]

Sets the current rhyming mode for queries.

**Parameters**

|Name|Type|Description|
|---|---|---|
|flags|Flags|The rhyme types to use.<br/><br/><ul><li><b>perfect</b><br/>Everything after the first stressed vowel matches in pronunciation (picky / icky).</li><li><b>weak</b><br/>The penultimate syllable is stressed and the final syllable rhymes (coffin / raisin).</li><li><b>syllabic</b><br/>The final syllable rhymes (senator / otter).</li><li><b>semirhyme</b><br/>The words would rhyme if not for the final syllable (broom / broomstick).</li><li><b>forced</b><br/>The words might rhyme if you really pushed it.</li><li><b>slant-rhyme</b><br/>The ending consonants are the same (rant / ant).</li><li><b>pararhyme</b><br/>All the consonants match (tuna / teen).</li><li><b>alliteration</b><br/>All consonants up to the first vowel rhyme (dog / dude).</li></ul>|

***
## ring

**Overloads:** 1

### [ring: character]

Accents the specified character with a ring (å) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## rs

**Overloads:** 1

### [rs: times; separator]

Sets the repetitions and separator for the next block. A combination of rep and sep.

**Parameters**

|Name|Type|Description|
|---|---|---|
|times|Number|The number of times to repeat the next block.|
|separator|Pattern|The separator pattern to run between iterations of the next block.|

***
## rvl

**Overloads:** 1

### [rvl: var-names...]

Rotates the values of a list of variables once to the left.

**Parameters**

|Name|Type|Description|
|---|---|---|
|var-names...|String|The list of the names of variables whose values will be rotated in order.|

***
## rvr

**Overloads:** 1

### [rvr: var-names...]

Rotates the values of a list of variables once to the right.

**Parameters**

|Name|Type|Description|
|---|---|---|
|var-names...|String|The list of the names of variables whose values will be rotated in order.|

***
## send

**Overloads:** 1

### [send: target-name; value]

Appends a string to the specified target's contents.

**Parameters**

|Name|Type|Description|
|---|---|---|
|target-name|String|The name of the target to send to.|
|value|String|The string to send to the target.|

***
## sendover

**Overloads:** 1

### [sendover: target-name; value]

Overwrites the specified target's contents with the provided value.

**Parameters**

|Name|Type|Description|
|---|---|---|
|target-name|String|The name of the target to send to.|
|value|String|The string to send to the target.|

***
## sep

**Aliases:** `s`<br/>**Overloads:** 4

### [sep]


***
### [sep: separator]

Sets the separator pattern for the next block.

**Parameters**

|Name|Type|Description|
|---|---|---|
|separator|Pattern|The separator pattern to run between iterations of the next block.|

***
### [sep: separator; conjunction]

Flags the next block as a series and sets the separator and conjunction patterns.

**Parameters**

|Name|Type|Description|
|---|---|---|
|separator|Pattern|The separator pattern to run between items.|
|conjunction|Pattern|The conjunction pattern to run before the last item.|

***
### [sep: separator; oxford; conjunction]

Sets the separator, Oxford comma, and conjunction patterns for the next series.

**Parameters**

|Name|Type|Description|
|---|---|---|
|separator|Pattern|The separator pattern to run between items.|
|oxford|Pattern|The Oxford comma pattern to run before the last item.|
|conjunction|Pattern|The conjunction pattern to run before the last item in the series.|

***
## split

**Overloads:** 2

### [split: list-name; input]

Splits the specified string into a list of chars.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-name|String|The name of the variable that will contain the output list.|
|input|String|The string to split.|

***
### [split: list-name; delimiter; input]

Splits the specified string by the given delimiter.

**Parameters**

|Name|Type|Description|
|---|---|---|
|list-name|String|The name of the variable that will contain the output list.|
|delimiter|String|The delimiter.|
|input|String|The string to split.|

***
## ss

**Overloads:** 1

### [ss]

Prints an eszett (ß).
***
## start

**Overloads:** 1

### [start: before-pattern]

Sets a pattern that will run before the next block.

**Parameters**

|Name|Type|Description|
|---|---|---|
|before-pattern|Pattern|The pattern to run before the next block.|

***
## sub

**Overloads:** 1

### [sub: a; b]

Prints the difference of the specified values.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Number|The first operand.|
|b|Number|The second operand.|

***
## swap

**Overloads:** 1

### [swap: a; b]

Swaps the values of the variables with the two specified names.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|The name of the first variable.|
|b|String|The name of the second variable.|

***
## switch

**Overloads:** 1

### [switch: input; case-pairs...]



**Parameters**

|Name|Type|Description|
|---|---|---|
|input|String|*No description*|
|case-pairs...|Pattern|*No description*|

***
## sync

**Aliases:** `x`<br/>**Overloads:** 1

### [sync: name; type]

Creates and applies a synchronizer with the specified name and type.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the synchronizer.|
|type|Mode|The synchronization type to use.<br/><br/><ul><li><b>none</b><br/>A random element is selected each time.</li><li><b>forward</b><br/>Executes from left to right.</li><li><b>reverse</b><br/>Executes from right to left.</li><li><b>deck</b><br/>Shuffles items and executes in order. Re-shuffled each time all items are used up.</li><li><b>cdeck</b><br/>Shuffles items and executes in order. The same order is reused for each traversal.</li><li><b>locked</b><br/>Chosen randomly, the same element is selected each time.</li><li><b>ping</b><br/>Starting at the first item, iterates through all elements in order and then reverses without repeating boundary elements.</li><li><b>pong</b><br/>Starting at the last item, iterates through all elements backwards and then reverses without repeating boundary elements.</li><li><b>no-repeat</b><br/>The same element will never be chosen twice in a row, as long as the block contains at least two elements.</li></ul>|

***
## target

**Aliases:** `t`<br/>**Overloads:** 1

### [target: target-name]

Places a target with the specified name at the current write position.

**Parameters**

|Name|Type|Description|
|---|---|---|
|target-name|String|The name of the target.|

***
## targetval

**Overloads:** 1

### [targetval: target-name]

Prints the current value of the specified target. This function will not spawn a target.

**Parameters**

|Name|Type|Description|
|---|---|---|
|target-name|String|The name of the target whose value to print.|

***
## then

**Overloads:** 1

### [then: condition-pass-pattern]

Executes a pattern if the current flag condition passes.

**Parameters**

|Name|Type|Description|
|---|---|---|
|condition-pass-pattern|Pattern|*No description*|

***
## tilde

**Aliases:** `tld`<br/>**Overloads:** 1

### [tilde: character]

Accents the specified character with a tilde (ã) accent.

**Parameters**

|Name|Type|Description|
|---|---|---|
|character|String|*No description*|

***
## tm

**Overloads:** 1

### [tm]

Prints the trademark symbol.
***
## toggle

**Overloads:** 1

### [toggle: flags...]

Toggles the specified flags.

**Parameters**

|Name|Type|Description|
|---|---|---|
|flags...|String|*No description*|

***
## txtfmt

**Overloads:** 1

### [txtfmt: format]

Sets the text conversion format for all open channels.

**Parameters**

|Name|Type|Description|
|---|---|---|
|format|Mode|The conversion mode to use.<br/><br/><ul><li><b>none</b><br/>No conversion.</li><li><b>fullwidth</b><br/>Fullwidth characters.</li><li><b>cursive</b><br/>Cursive script.</li><li><b>bold-cursive</b><br/>Bold cursive script.</li></ul>|

***
## typeof

**Overloads:** 1

### [typeof: name]

Gets the type of the specified variable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|*No description*|

***
## undef

**Overloads:** 1

### [undef: flags...]

Undefines the specified flags.

**Parameters**

|Name|Type|Description|
|---|---|---|
|flags...|String|The list of flags to undefine.|

***
## v

**Overloads:** 1

### [v: name]

Prints the value of the specified variable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the variable to retrieve.|

***
## vadd

**Overloads:** 1

### [vadd: a; b]

Adds a number to the specified variable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|The name of the variable to add to.|
|b|Number|The value to add.|

***
## vb

**Overloads:** 1

### [vb: name; value]

Creates a new string variable with the specified name and value.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the variable.|
|value|Boolean|The value of the variable.|

***
## vcpy

**Overloads:** 1

### [vcpy: a; b]

Copies the value of the variable with the first name to the variable with the second name.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|The variable to copy from.|
|b|String|The variable to copy to.|

***
## vdiv

**Overloads:** 1

### [vdiv: a; b]

Divides the specified variable by a number.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|The name of the variable to divide.|
|b|Number|The divisor.|

***
## veq

**Overloads:** 1

### [veq: a; b]

Prints a boolean value indicating whether the variables with the two specified names are equal to each other.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|RantObject|*No description*|
|b|RantObject|*No description*|

***
## vexists

**Overloads:** 1

### [vexists: name]

Prints a boolean value indicating whether a variable with the specified name exists.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the variable to check.|

***
## vl

**Overloads:** 2

### [vl: name]

Creates a new list.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the list.|

***
### [vl: name; length]

Creates a new list with a specified length.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the list.|
|length|Number|The length of the list.|

***
## vlen

**Overloads:** 1

### [vlen: obj]

Gets the length of the specified variable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|obj|RantObject|*No description*|

***
## vmod

**Overloads:** 1

### [vmod: a; b]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|*No description*|
|b|Number|*No description*|

***
## vmul

**Overloads:** 1

### [vmul: a; b]

Multiplies the specified variable by a number.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|The name of the variable to multiply.|
|b|Number|The value to multiply by.|

***
## vn

**Overloads:** 2

### [vn: name; value]

Creates a new number variable with the specified name and value.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the variable.|
|value|Number|The value of the variable.|

***
### [vn: name; min; max]

Creates a new number variable with a random value between the specified minimum and maximum bounds.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the variable.|
|min|Number|The minimum bound of the value.|
|max|Number|The maximum bound of the value.|

***
## vne

**Overloads:** 1

### [vne: a; b]

Prints a boolean value indicating whether the variables with the two specified names are not equal to each other.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|RantObject|*No description*|
|b|RantObject|*No description*|

***
## vnot

**Overloads:** 1

### [vnot: a]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|*No description*|

***
## vp

**Overloads:** 1

### [vp: name; value]

Creates a new pattern variable with the specified callback.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the variable.|
|value|Pattern|The value of the variable.|

***
## vs

**Overloads:** 1

### [vs: name; value]

Creates a new string variable with the specified name and value.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the variable.|
|value|String|The value of the variable.|

***
## vsub

**Overloads:** 1

### [vsub: a; b]

Subtracts a number from the specified variable.

**Parameters**

|Name|Type|Description|
|---|---|---|
|a|String|The name of the variable to subtract from.|
|b|Number|The value to subtract.|

***
## while

**Aliases:** `loop`<br/>**Overloads:** 1

### [while: condition; body]

Runs the body over and over while condition remains true.

**Parameters**

|Name|Type|Description|
|---|---|---|
|condition|Pattern|The condition to check each iteration.|
|body|Pattern|The body of the loop.|

***
## xdel

**Overloads:** 1

### [xdel: name]

Deletes a synchronizer.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the synchronizer to delete.|

***
## xor

**Overloads:** 1

### [xor: a; b...]



**Parameters**

|Name|Type|Description|
|---|---|---|
|a|Boolean|*No description*|
|b...|Boolean|*No description*|

***
## xpin

**Overloads:** 1

### [xpin: name]

Pins a synchronizer.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the synchronizer to pin.|

***
## xreset

**Overloads:** 1

### [xreset: name]

Resets a synchronizer to its initial state.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the synchronizer to reset.|

***
## xstep

**Overloads:** 1

### [xstep: name]

Iterates a synchronizer.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the synchronizer to iterate.|

***
## xunpin

**Overloads:** 1

### [xunpin: name]

Unpins a synchronizer.

**Parameters**

|Name|Type|Description|
|---|---|---|
|name|String|The name of the synchronizer to unpin.|

***
## yield

**Overloads:** 1

### [yield]

Yields the currenty written output.
***

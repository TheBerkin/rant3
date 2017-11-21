**Escape sequences** allow Rant to print characters that are reserved by the language or unable to be easily typed.

An escape sequence is constructed with a backwards slash (`\`) followed by one character (or more than one, in the case of `\u` or with a quantifier added).

**Any** character can be escaped.
This includes brackets `\{ \[ \<`, slashes `\\ \/`, quotes `\" \'`, number signs `\#` and everything else. 

```rant

\[This sentence is inside brackets.\]

```

## Types

There are two types of escape sequence characters:

### Static escape sequences

Static escape sequences always print the same value.

|Escape sequence|Description|Unicode character code|
|----|----|----|
|\n|Line feed|0x000a|
|\r|Carriage return|0x000d|
|\v|Vertical tab|0x000b|
|\b|Backspace|0x0008|
|\f|Form feed|0x000c|
|\s|Space|0x0020|
|\t|Horizontal tab|0x0009|
|\uxxxx|Unicode character|\u0052\u0041\u004e\u0054 = "RANT"|
|\Uxxxxxxxx|Surrogate pair|\U0001f602 = &#128514;|

### Dynamic escape sequences

Dynamic escape sequences either print a random selection from a list of characters,
or print certain context-sensitive strings (\a and \N).

|Escape sequence|Description|
|-----|-----|
|\a|English indefinite article|
|\c|Lowercase letter|
|\C|Uppercase letter|
|\d|Digit|
|\D|Nonzero digit|
|\N|System-specific line separator|
|\x|Lowercase hexadecimal digit|
|\X|Uppercase hexadecimal digit|
|\w|Lowercase alphanumeric character|
|\W|Uppercase alphanumeric character|

## Quantifiers

Escape sequences can print more than one character using a **quantifier**.
A quantifier is added by inserting a positive, nonzero integer after the backward slash, separated from the escape character by a comma.

The number specifies how many times to repeat the character. If the escape character is dynamic, each repetition will be randomized.

```
\16,X   # 16-character uppercase hexadecimal number
\32,c   # 32 random lowercase letters
\100,n  # 100 line feeds
```
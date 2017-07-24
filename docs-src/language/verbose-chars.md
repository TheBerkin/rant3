Verbose characters allow you to enter any character by using its Unicode name.
Rant will convert all verbose literals to their corresponding characters when the pattern is compiled.

All Unicode 9.0 characters are supported.

## Syntax

A verbose character can be entered using the following structure:

```rant
\@CHAR NAME@
```

The name is not case-sensitive, and spaces may be substituted for underscores or hyphens without consequence.

## Usage

A common usage of these literals is to type accented characters. For example, the ä character can be entered using
the following:

```rant
\@LATIN SMALL LETTER A WITH DIAERESIS@
```

It can also be used with emoji:

```rant
\@THUMBS UP SIGN@
```
```rant
\@FACE WITH TEARS OF JOY@
```
```rant
\@OK HAND SIGN@
```

### Procedural characters

To enter characters procedurally, use the [`char`](functions#char) function.

## Character names

There are many online resources for looking up Unicode character names. Here are a few you can use:

- [unicode.org](http://unicode.org/charts/)
- [unicode-table.com](https://unicode-table.com)
- [fileformat.info](http://www.fileformat.info/info/unicode/char/a.htm)

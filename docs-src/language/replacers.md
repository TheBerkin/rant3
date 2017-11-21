**Replacers** are based on a special type of function that scans an input string for substrings matching a regular expression,
and then replaces each match with a string returned by a callback.

## Syntax

The syntax is similar to a function call. The difference is that the function name is replaced by a regular expression,
and there are exactly two parameters: the first passes in the string to transform, and the second is a callback
that is executed whenever a match is found. The text printed within this callback becomes the replacement text for that
specific match.

```rant
[`search-pattern`:
    string to search;
    replacement callback
]
```

## Accessing match and group values

Use [`[match]`](/language/functions#match) within the replacement callback to print the current match value.

Use [`[group]`](/language/functions#group) within the replacement callback to print the value of a named capturing group.

Comments allow you to annotate your patterns with meaningless rambling.
They do not affect the behavior of the pattern unless you comment out part of the code.

A comment can be inserted by adding the `#` character anywhere on a line.
Everything in front of it on that line will be turned into a comment.
The only exception is if you [escape](/language/escape-sequences) the comment character, because then it just prints the `#`.

```rant
# Define subroutine for concatenating two strings
[$[concat:a;b]:
    [arg:a][arg:b] # Print both arguments
]

# Nobody would actually do this with a subroutine. Shhhh
[$concat:Hello\s;World!]
```
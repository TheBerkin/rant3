Rant features a robust formatting engine that includes multiple configurable options for automatically and manually formatting output.
These options include multilingual number verbalization, capitalization, number formatting, and conversion to other character forms like fullwidth.

This article gives a brief summary of each of the major features of the formatting engine, as well as examples of how to use them.

## Captalization

Output can be automatically converted to one of numerous capitalization types using the [`[case]`](/language/functions#case) function.
When the function is called, all printed text after the function call will be formatted according to the specified mode.

To illustrate this effect, see the below example, which contains three uncapitalized sentences. The last two are separated from the first 
by a call to `[case]` that changes the capitalization for the second and third sentences.

```rant
this sentence is in lowercase and will not be formatted.\n
[case:sentence]
you can see this sentence starts with a capital letter. this one, too!
```

This produces the following output:
```
this sentene is in lowercase and will not be formatted.
You can see this sentence starts with a capital letter. This one, too!
```

## Number formatting

Rant is also able to automatically format numeric outputs from functions. To do this, the [`[numfmt]`](/language/functions#numfmt) function
is used. It can format numbers to hexadecimal, Roman numerals, binary, and more. Below is an example demonstrating just a few of the posssibilities:

```rant
[numfmt:verbal][rs:10;,\s]{[rn]}\n
[numfmt:roman-upper][rs:10;,\s]{[rn]}\n
[numfmt:binary][rs:10;,\s]{[rn]}\n
```

Each of these lines counts to ten in a different format, resulting in the following output:

```
one, two, three, four, five, six, seven, eight, nine, ten
I, II, III, IV, V, VI, VII, VIII, IX, X
1, 10, 11, 100, 101, 110, 111, 1000, 1001, 1010
```

The `[numfmt]` function also has another overload for manual formatting that only operates on a specified input.

```rant
[numfmt:verbal;[n:10]] = [n:10]
```
```
ten = 10
```

## Character format

Alphanumeric characters can be automatically converted to other forms using the [`[txtfmt]`](/language/functions#txtfmt) function
to change the character formatting mode. Here is an example that converts a string to fullwidth:

```rant
[case:upper]
[txtfmt:fullwidth]
aesthetic
```
```
ＡＥＳＴＨＥＴＩＣ
```
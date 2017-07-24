This article describes other query filters with specialized functionality.

## Regex filters

Regex filters ensure that the returned term matches, or does not match, a specified regular expression:

```rant
<name.subtype ? /regex/>    # Term must match regex
<name.subtype ?! /regex/>   # Term must not match regex
```

As the official Rant runtime runs on the .NET Framework, Rant uses .NET's Regex flavor.

### Regex options

* `/regex/i`: Case insensitive
* `/regex/m`: Multiline

## Phrasal complement

Phrasal complements are typically used in phrasal verbs, such as "turn _ inside-out" or "set _ on fire".
A phrasal complement may be specified in a query by enclosing a pattern in square brackets anywhere after
the name/subtype specifiers.

```rant
<name> <verb.ed-transitive [ <name> ]> <adv>.
```

!!! note
    If the returned term does not use a complement, the default behavior is for Rant to
    insert the complement after the term.

## Syllable limit

Syllable limits ensure a specific number or range of syllables in the returned term, and are typically used
in combination with rhyming carriers to create poetry.

```rant
<noun.subtype(x)>   # Exactly x syllables
<noun.subtype(x-)>  # At least x syllables
<noun.subtype(-x)>  # At most x syllables
<noun.subtype(x-y)> # Syllable count must be between x and y
```
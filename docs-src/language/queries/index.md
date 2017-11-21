**Queries** interface with the dictionary loaded by the current Rant context
and retrieve (usually randomized) entries according to user-specified filters.

All query examples in this documentation use [Rantionary](https://github.com/RantLang/Rantionary).

## Query structure

Queries are enclosed in a pair of angle brackets.

At minimum, a query requires the name of the table it will fetch from. Only specifying a name
will fetch a random entry from the table and return its first term.

```rant
# Fetch a random noun in singular form
<noun> 
```

!!! note
    The table name will always be the first item in a query.

## Subtype

To fetch a specific term, a subtype of the term must be specified following the name of the table.
The table name and subtype are separated by a period.

```rant
# Fetch a plural noun
<noun.pl>
```

### Plural subtype

You can also specify an alternate subtype to make the query plural-sensitive. This is particularly
useful for automatically pluralizing a noun according to its preceding quantity. Rant automatically keeps track
of the last-written number and will use the plural subtype if the last number is not equal to one.

The plural subtype is indicated by separating with two periods instead of one. It may also be used in combination with
a regular subtype, to which it will fall back if the plural isn't triggered.

```rant
[n:1;3] <noun..pl>
```
```rant
[n:1;3] <noun.sg..pl>
```
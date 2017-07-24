**Class filters** allow queries to narrow their search to entries with or without specific classes.

Filters are specified after the table name and subtype.

## Syntax

A **positive** class filter filters for entries with a specific class.
To write one, insert a hyphen `-` followed by the name of the class to filter by.
If you'd like Rant to select one of several classes to filter by randomly,
separate them with vertical pipes (`|`).

```rant
<noun-animal>               # Animal
<noun -animal -red>         # Animal that is red
<noun -animal -red|yellow>  # Animal that is red or yellow
```

### Negative filters

Negative filters do the opposite of positive filters: they filter for entries
without a specific class. This is indicated by an exclamation point before the class
name.

```rant
<noun..pl -!animal>   # Any plural noun that isn't an animal
```

## Exclusivity

In some rare cases, a user might want to filter for entries that **only** contain specific
classes. This behavior is called _exclusivity_. A query can be made exclusive by placing a
dollar sign `$` before the class filters.

```rant
<noun$-animal>  # Any noun that only contains the "animal" class
```
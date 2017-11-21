If you're a writer working inside software that already integrates Rant, you're in the right place.
Let's get you writing some neat stuff straight away.

## Automatic formatting

Rant can do a lot of tedious work for you.
It can capitalize your sentences and titles for you, format your numbers in several different bases, 
insert primary and secondary quotation marks, intelligently insert 'a' or 'an' depending on the following word,
and a lot more.

### Capitalize sentences

For this we need the `[case]` function.
```rant
[case:sentence]
this is a sentence. this is another sentence.
```

With sentence capitalization enabled, Rant will automatically fix sentence capitalization for you.

```
This is a sentence. This is another sentence.
```

### Spell out numbers

The `[numfmt]` function controls number formatting. To spell numbers out, you need to use the `verbal` mode.

```rant
[numfmt:verbal][rs:5;,\s]{[n:1;100]}
```

This pattern produces a list of five numbers in English.

```
sixty-five, seventy-seven, fourteen, sixty-three, thirty-three
```

## Comma-separated lists

Rant makes writing lists of things easy. It also supports generating lists both with and without an Oxford comma.
This feature can be accessed through two specific overloads of the `[sep]` function.

### List with an Oxford comma

```rant
[case:first][rep:3][sep:,;,;and]{\a <animal::!a>} walk into a bar.
```

The output will show a very nice list of animals.

```
An octopus, a deer, and a silverback gorilla walk into a bar.
```

### List without an Oxford comma

```rant
[case:first][rep:3][sep:,;and]{\a <animal::!a>} walk into a bar.
```

Assuming the same seed is used, the output is only slightly different; the Oxford comma is not there.

```
An octopus, a deer and a silverback gorilla walk into a bar.
```

## Reusing a word from a query

Queries are useful for fetching random words under specific search criteria, but sometimes you may want to use that word more than once.
Rant has a feature specifically for doing this: the **match carrier**.
Carriers are a query feature that allows you to make query results depend on previous queries; you can read more about them [here](/language/queries/index.md).

With a dictionary like [Rantionary](http://github.com/RantLang/Rantionary) loaded, try running this pattern:

```rant
One <noun-animal::=a>! Two <noun.pl::=a>! THREE <noun.pl::=a>!
```

All three queries will always provide the same word.

```rant
One donkey! Two donkeys! THREE donkeys!
```

You only need to specify your filters on the first query.
The subtype can be changed without consequence.
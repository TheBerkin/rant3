A Rant dictionary is a collection of lists called **tables**, which separate words by their type.
Each table contains a list of **entries**, which consist of equal numbers of **terms**. Terms are the actual strings
that Rant queries fetch.

A dictionary is contained within a folder and consists of two major components:
the first is the tables files, which have the extension `.table`. The second is a file
called `rantpkg.json` which contains information that allows Rant to create
a package from the dictionary.

This page will discuss the .table format. To learn more about rantpkg.json and creating packages,
see the [packages](packages.md) page.

## Terminology

Tables are split into two main sections: the **header** and the **lexicon**.

The header comes first and describes the table's name, layout, and any special properties it may have.
It consists of a number of **directives**. Directives are one-line instructions that start with an `@` symbol.
Directives can also appear in the lexicon, but we'll discuss header directives first.

Directives can take arguments, which are separated by commas. You may also use escape sequences like `\n`, `\uxxxx`, and `\Uxxxxxxx`.

## Comments

Table comments are just like Rant comments, starting with `#` and taking up the rest of the line. They may not appear within a quoted argument.

```rtbl
@name adj
@sub regular    # Normal form
@sub com        # Comparative form
@sub sup        # Superlative form
```

## Header structure

### Name

At minimum, a table expects a name. This is specified with the `@name` directive.

```rtbl
@name noun

# ...
```


### Subtypes

Subtypes are strings that point to specific term numbers within an entry and can be used to access different variants of a word, such as
verb conjugations, noun forms, adjective forms, and other useful information.

You can add a subtype using the `@sub` directive. By default, Rant adds one subtype to your table, so you don't need one if your entries
only have one form.

```rtbl hl_lines="2 3"
@name noun
@sub singular, sg
@sub plural, pl
```

Each @sub directive assigns subtypes to a specific term, which corresponds to the number of the `@sub` directive.
In the above case, the first term has the subtypes `singular` and `sg`, while the second term has the subtypes `plural` and `pl`.
A term can have as many subtypes as you want, as long as it has at least one.

If you'd like to sort your `@sub` directives independent of the term order, you can indicate the term index before the subtype name
to override the default behavior.

This header is equivalent to the one above:
```rtbl
@name noun
@sub 1, plural, pl
@sub 0, singular, sg
```

### Hidden classes

If you want specific classes to be excluded from the query results unless explicitly requested, you can indicate these classes
using the `@hide` directive. Rant always implicitly hides the `nsfw` class for you.

```rtbl hl_lines="11"
@name verb
@sub present, imperative
@sub gerund, ing
@sub simple_past, ed
@sub third_person, third_person_present, s
@sub agent, er
@sub past_participle, pp
@sub nominalization, nom
@sub plural_nominalization, noms

@hide visual, speech
```

## Corpus structure

The main component of the corpus is, of course, its entries.
An entry occupies a single line and starts with the `>` symbol, followed by comma-separated terms.
Escape sequences are permitted in the terms, and boundary whitespace is ignored.

### Entries

Terms should be written in the order of the subtypes. In the below example,
singular comes first, and then plural.

```rtbl hl_lines="5 6 7 8 9 10 11"
@name noun
@sub singular, sg
@sub plural, pl

> chickadee, chickadees
> chicken, chickens
> crow, crows
> dove, doves
> eagle, eagles
> finch, finches
> flamingo, flamingos

# ...
```

### Templates

You can reuse other entries to create new entries that expand on that information.
Entries used in this way are called **templates**.

There are two kinds of templates: _entry templates_ and _dummy templates_.

#### Entry templates

Entry templates exist as actual entries in the table and are assigned an identifier by which they can
be referred to by other entries looking to expand on them.

This identifier is assigned using the `@id` directive just before the entry.

```rtbl
@id penguin
> penguin, penguins
    - class bird, penguin, flightless
```

#### Dummy templates

Dummy templates do not exist as entries, and only serve as a basis on which to build other entries.
To mark an entry template as a dummy, insert the `@dummy` directive before the entry.

```rtbl
@id penguin
@dummy
> penguin, penguins
    - class bird, penguin, flightless
```

### Using templates

To use a template in another entry, one of two methods can be used.
The first is to use the `@using` directive before the entry, passing in the name of the template.

```rtbl
@using penguin
> erect-crested [], erect-crested []
```

You will notice something strange going on in the terms: a pair of brackets has appeared after the term text.
This pair of brackets inserts the text from the template's corresponding term into the new term. 

The above is equivalent to the following entry:

```rtbl
> erect-created penguin, erect-created penguins
    - class bird, penguin, flightless
```
!!! note
    When you use `@using`, the new entry also inherits all of the template's properties.

### Other ways to use templates

The other way to use templates is to reference them by name from inside the terms. This is useful especially
if you need more than one template, or you want to use a term that doesn't correspond with the inheriting term's
subtype. You don't need a `@using` directive to do this.

The following template reference structures may be used anywhere inside a term:

```rtbl
[template-id]           # Inserts template's corresponding term
[template-id.index]     # Inserts tempalte's term corresponding with the index
[template-id.subtype]   # Inserts template's term corresponding with the subtype
```

### Phrasals

The position of the phrasal complement can be indicated using the `+` symbol in a term.

```rtbl
> set + on fire, setting + on fire, set + on fire, sets + on fire, ...
```

### Entry properties

Entries can be assigned properties by creating a line below the entry starting with `-`, followed
by a key-value pair separated with a space. There are several properties you can assign to entries.
A simple property that adds a class looks like this:

```rtbl
> dog, dogs
    - class mammal
```

#### class

The `class` property assigns classes to an entry. You can assign multiple classes separated by commas;
for example, the noun `spear` might have an entry that looks like this:

```rtbl
> spear, spears
    - class weapon, sharp, long
```

!!! note
    Class names may only contain letters, numbers, and underscores.

There is another way to assign classes using a special directive, which is discussed further on.

#### weight

The `weight` property affects the weight of an entry, such that higher weight values make the entry
more likely to be chosen by a query (as long as the entry passes all filters on the query).
The weight value must be a decimal number greater than zero.

An entry with no `weight` property is given a weight of 1 by default.

```rtbl
@name name

> John
> Bob
> Paul
> Gary
> David
    - weight 0.1 #Appears 10% as often as the others
```

#### pron

The `pron` property assigns pronunciation data to the terms and requires the same number of values as terms.
This data is used by the rhyming engine.

Pronunciation data is expected in X-SAMPA format.

```rtbl
> baseball, baseballs
    - class ball
    - pron b\"eIs b\"Ol, b\"eIs b\"Olz
```

#### Metadata properties

You can also assign a property of any name to add special metadata not covered by the built-in properties.
Rant will store this information as a key/value pair in the entry. Multiple values are stored as an array.

```rtbl
> dog, dogs
    - class mammal
    - species Canis lupus
> cat, cats
    - class mammal
    - species Felis catus
```

### Class ranges

It is possible to assign one or more classes to a section of entries using the `@class` and `@endclass` directives to create a *class range*.

The `@class` directive begins a class range and is passed the classes you want to apply to the entries within the range.

The `@endclass` directive takes no arguments and marks the end of the range.

```rtbl
@class penguin, bird, flightless
    > adelie penguin, adelie penguins
    > African penguin, African penguins
    > chinstrap penguin, chinstrap penguins
    > emperor penguin, emperor penguins
    > erect-crested penguin, erect-crested penguins
    > fairy penguin, fairy penguins
    > gentoo penguin, gentoo penguins
    > Humboldt penguin, Humboldt penguins
    > king penguin, king penguins
    > macaroni penguin, macaroni penguins
    > Magellanic penguin, Magellanic penguins
    > penguin, penguins
    > rockhopper penguin, rockhopper penguins
    > yellow-eyed penguin, yellow-eyed penguins
@endclass
```

!!! tip
    Class ranges can also be nested to create even more specific classifications.
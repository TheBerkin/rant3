A **carrier** is an optional query argument that allows different queries to interact with each other.
There are many different kinds of carriers that enable different types of behaviors, which are detailed
in this article.

## Carrier syntax

The carrier always goes last in a query and is separated from the filters by a double-colon `::`.

A carrier consists of one or more **carrier components**, each of which consists of a symbol
identifying the component type, followed by an identifier.

```rant
# Simple carrier that causes several queries to return the same word
A <noun:: =A> is a <noun:: =A> is a <noun:: =A>.
```

### Component types

#### Match

**Symbol:** `=`

A **match** component causes a query to return the same entry as the first query that used a match
with the same identifier. For example, two queries with a match component that each use the carrier `=x`
will return the same entry, but `=x` and `=y` will be (most likely) different.

```rant
<name-male> was \a <adj::=a1>, <adj::=a1> man.
```
```
Jim was an awful, awful man.
```

#### Unique

**Symbol:** `!`

A **unique** component causes a query to return a different entry than every other query that used
a unique component with the same identifier.

If Rant runs out of entries, `[No Match]` will be returned.

#### Match-unique

**Symbol:** `!=`

A **match-unique** component returns an entry that is different from the entry returned by the match
component with the specified identifier.

#### Associative

**Symbol:** `@`

An **associative** component, or **association**, returns entries that have exactly
the same classes.

#### Match-associative

**Symbol:** `@=`

A **match-associative** component, or **match association**, returns entries that have exactly
the same classes as the entry returned by the match with the specified identifier.

#### Dissociative

**Symbol:** `@!`

A **dissociative** component, or **dissociation**, returns entries that have no classes in common.

#### Match-dissociative

**Symbol:** `@!=`

A **match-dissociative** component, or **match dissociation**, returns entries that have no classes in
common with the entry returned by the match with the specified identifier.

#### Relational

**Symbol:** `@?`

A **relational** component, or **relation**, returns entries that have at least one class in common.

#### Match-relational

**Symbol:** `@?=`

A **match-relational** component, or **match relation**, returns entries that have at least one class
in common with the entry returned by the match with the specified identifier.

#### Integral

**Symbol:** `@*`

An **integral** component, or **integration**, returns entries that include all of their classes
into the last entry returned by the same integration. The last entry may contain other classes.

#### Divergent

**Symbol:** `@+`

A **divergent** component, or **diversion**, returns entries that have at least one differing, or extra class.

#### Match-divergent

**Symbol:** `@+=`

A **match-divergent** component, or **match diversion**, returns entries that have at least one differing, or extra
class than the entry returned by the match with the specified identifier.

#### Rhyme

**Symbol:** `&`

A **rhyme** component returns entries that rhyme with each other according to the current rhyming mode.

You can set the current rhyming mode using the [`[rhyme]`](/language/functions/#rhyme) function.

## Reference table

Refer to the following table to find the carrier component with the desired class association behavior:

|             |1+ classes|All classes |
|-------------|----------|------------|
|**Matching** |Relation  |Association |
|**Different**|Diversion |Dissociation|
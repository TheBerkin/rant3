Regular queries have no dynamic parts except for the complement. Sometimes, however,
you may want to vary the parameters in a query. Fortunately, this can be achieved with a
specialized set of functions included in Rant's framework, known as "query-builder" 
functions.

Everything that can be done with regular queries can be done with query-builder functions,
but with added benefits: you can vary query parameters and store queries for later use in
the same pattern. Queries built this way are called **dynamic queries**.

## Table of functions

|Function Name                                |Description/Equivalent              |
|---------------------------------------------|------------------------------------|
|`[query]`, `[query:id]`                      |Runs stored query.                  |
|`[qexists:id]`                               |Check if query ID exists            |
|`[qdel:id]`                                  |Delete query                        |
|`[qcc:id;cc-name;cc-type]`                   |Carrier component                   |
|`[qname:id;name]`, `[qname:id;name;subtype]` |`<name ...>`, `<name.subtype ...>`  |
|`[qcf:id;class]`                             |`-class`                            |
|`[qcfn:id;class]`                            |`-!class`                           |
|`[qhas:id;regex;opt]`                        |```? `regex`opt```                  |
|`[qhasno:id;regex;opt]`                      |```?! `regex`opt```                 |
|`[qsylmin:id;x]`                             |`(x-)`                              |
|`[qsylmax:id;x]`                             |`(-x)`                              |
|`[qsyl:id;x]`, `[qsyl:id;x;y]`               |`(x)`, `(x-y)`                      |
|`[qphr:id;phrasal]`                          |`[phrasal]`                         |

## How to use

As a gentle introduction to this very different approach to queries, let's first
look at a case where a dynamic query would be useful.

Suppose we want to print an adjective followed by a noun and have them start with the same letter.
We might start out with something like this:

```rant
<adj-appearance?`^a`i> <noun.pl?`^a`i>
```

This gets us part of the way. Both words are guaranteed to start with A! But what if we want to randomize
which letter they start with? This cannot be accomplished with the conventional syntax.

This is one example of a task that is easy to perform with a dynamic query. Let's convert this query
to its equivalent query-builder functions so we can figure out how to solve the problem:

```rant
# Create a dynamic query named word1 for the adjective
[qname:word1;adj]       # Table name
[qcf:word1;appearance]  # Filter for appearance-related adjectives
[qhas:word1;^a;i]       # Regex filter

# Create another one called word2 for the noun
[qname:word2;noun]      # Table name
[qsub:word2;pl]         # Subtype
[qhas:word2;^a;i]       # Regex filter

# Run the queries
[q:word1] [q:word2]
```

Now that we've broken down the problem with query-building, the regex filter is now supplied as a pattern
to a function! Let's change the argument so that it returns a random, but matching letter to both queries' filters.

```rant
# Create a dynamic query named word1 for the adjective
[qname:word1;adj]                   # Table name
[qcf:word1;appearance]              # Filter for appearance-related adjectives
[qhas:word1;^[branch:ml;\c];i]      # Regex filter

# Create another one called word2 for the noun
[qname:word2;noun]                  # Table name
[qsub:word2;pl]                     # Subtype
[qhas:word2;^[branch:ml;\c];i]      # Regex filter

# Run the queries
[q:word1] [q:word2]
```

When we run this, the two words will now always start with the same letter.

```
dry daisies
```
```
bearded butlers
```
```
humongous hemorrhoids
```

This is just one possibility out of many for dynamic queries. Here is a non-exhaustive list of things they let you do:

- Use variables in queries
- Randomize class filters
- Dynamically choose the contents and number of carrier components
- Conditionally include/exclude filters
- Store queries for multiple uses

## Some more examples

Below is a list of conversions between static and dynamic queries to help you further in
understanding how the functions work.

|Static Query|Dynamic Query|
|------------|-------------|
|`<noun>`|`[qname:test;noun][q:test]`|
|`<noun.pl>`|`[qname:test;noun][qsub:test;pl][q:test]`|
|`<verb-transitive>`|`[qname:test;verb][qcf:test;transitive][q:test]`|
|`<adj::=a>`|`[qname:test;adj][qcc:test;a;match][q:test]`|
|`<noun-person|animal|tool>`|`[qname:test;noun][qcf:test;{person|animal|tool}][q:test]`|
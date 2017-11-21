**Functions** are instructions that allow the Rant language to interact with the underlying engine and framework,
controlling all aspects of generation from configuration of automatic formatting to block attributes to channel creation.
These provide the vast majority of useful functionality to Rant.

If you are already familiar with the syntax, skip over to the [function reference](/language/functions.md) for a full list of available functions.

## Function calls

Functions are called upon by using a pair of square brackets with the function name inside.
Some functions require arguments, which are placed after the name in a specific way.

If a function requires arguments, they are separated from the function name by a colon. Individual arguments are then separated from one another by semicolons.

```rant
# No arguments
[repeach]

# One argument
[rep: 10]

# Two arguments
[rs: 10; ,\s]

# Three arguments. Notice how you can also use functions inside of args!
[chan: shush; private; s[r:[n:2;10]]{h}...]

# Etc etc etc

```

## Argument types

Functions that take arguments can handle arguments in two different ways:

### Greedy arguments

An argument can be immediately interpreted to a string before the function is called. This is called a **greedy** argument.

!!! note

    An example of a greedy argument is the repetition count on the `[rep]` function.
    Since it is only needed once, it's interpreted immediately.

### Lazy arguments

An argument can also be stored as code and is only run when accessed by the engine. This is called a **lazy** argument.
Lazy arguments may be run more than once, depending on the function.

!!! note

    An example of a lazy argument is the separator pattern on the `[sep]` function.    
    Since the separator may be dynamic, it is allowed to run multiple times.

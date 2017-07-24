# Subroutines

Subroutines are blocks of code that can be executed from anywhere after the point at which they are declared.
They can be configured to accept arguments from the user to customize their behavior and output.

A basic subroutine is structured as follows:

```rant
[$[hello:name]: Hello, [arg:name]!]
```

In the above example, a subroutine named `hello` is declared with a single parameter, `name`.
The subroutine body prints a basic greeting that fetches and inserts the value passed to `name` using the `[arg]` function.

Subroutines are used the same way as functions, but the function name is the subroutine name prefixed with `$`.

```rant
[$hello:David]
```
```
Hello, David!
```

## Parameter types

There are two different kinds of parameters that subroutines can accept.
The first, demonstrated in the main example, is a *value parameter*.
Another name for a value parameter is a "greedy" parameter.

Value parameters are represented by a simple identifier. When a pattern
is passed to a value parameter, it is interpreted and the resulting output
is stored as the argument value. This means that the code passed to the parameter
is guaranteed to run just once.

The second type of parameter is a _lazy parameter_. The code passed to a lazy parameter
is run every time the argument value is retrieved using `[arg]`. This means that the code
passed to the parameter can be run more than once, and the value may therefore change. It is
also possible that it might not be run at all, if the subroutine body never accesses it.

Lazy parameters are represented by an identifier prefixed with `@`. A basic example
of how a lazy parameter might be useful is when a subroutine needs a callback, or the user
would like to repeat the code multiple times.

Here is an example of a subroutine with a lazy parameter that repeats a pattern three times:

```rant
[$[three:@pattern]:
    [rep:3]{[arg:pattern]}
]
```

## Subroutine scopes

If a subroutine is defined within a block, it can only be accessed inside of the block.
That block becomes the _scope_ of the subroutine. Subroutines defined in the global scope
(outside of all blocks) can be accessed from other patterns, once declared.

```rant
{
    [$[test]: This is an example subroutine.]
    [$test] # Works
}
[$test] # Error
```
Serial patterns are used for returning multiple sequential outputs from a single program execution.
The most obvious use case for this feature is generating large amounts of related data,
such as names for randomized NPCs in a game, or separately outputting lines of a procedural dialogue script.

## Yielding in Rant

With serial patterns, an extra step is required before printed output can be sent back:
the output must be **yielded**, which means that all existing output is returned to the caller and a new,
blank output takes its place in the VM, ready to be written to. For this, we use the [`[yield]`](/language/functions#yield)
function.

To demonstrate how this works, here is a very simple example of a serial pattern that returns two separate outputs in order:

```rant
Marco!
[yield]
Polo!
[yield]
```

In this example, the first output will read `Marco!`, and the second will read `Polo!`.

***

Yielding also works in blocks and places with varying amounts of iterations:

```
# Random number of reps between 10 and 20
[rep:[num:10;20]]
{
    # Print the current iteration number
    [numfmt:verbal;[rn]]
    # Yield it!
    [yield]
}
```

## Running serial patterns in C&#35;

To run a pattern in serial mode, you must use the special `RantEngine.DoSerial` method,
which returns an `IEnumerable<RantOutput>` instead of a `RantOutput`. This means that
Rant can take advantage of the lazy evaluation behavior of enumerators, allowing your code
to process one output at a time without waiting for the whole program to finish.

Here is a simple code snippet to get you started. 
It runs a serial pattern that counts to ten, where each number is yielded separately.

```csharp
var rant = new RantEngine();
var pgm = new RantProgram.CompileString(@"[r:10]{[rn][yield]}");
foreach(var output in rant.DoSerial(pgm))
{
    Console.WriteLine(output);
}
```

The output should look like this:
```
1
2
3
4
5
6
7
8
9
10
```
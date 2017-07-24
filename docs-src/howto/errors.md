There are two types of errors that can occur when using Rant programs:

## Compiler errors

Compiler errors are raised by the compiler when it finds something in a pattern
that it is unable to make any sense of. It will return a message explaining the
problem, as well as where in the code (line, col, index) it found it.

In Rant, compiler errors are represented by the `RantCompilerException` class,
and this exception can be thrown by any method where compilation takes place.
To handle these errors, you will need to use a try/catch clause and examine
the exception's properties.

``` csharp
try
{
    var rant = new RantEngine();
    var program = RantProgram.CompileString(@"[rep:10;10;10][sep:,\s]]{<noun->}");
    Console.WriteLine(rant.Do(program));
}
catch (RantCompilerException ex)
{
    Console.WriteLine(ex.Message);
}

```

The `Message` property of `RantCompilerException` contains every single error
found in the pattern, as well as where each is located.

```
3 compiler errors found:
    1. (Pattern: Ln 1, Col 2) Function 'rep' has no overload that takes 3 argument(s).
    2. (Pattern: Ln 1, Col 24) Unexpected token found: ']'
    3. (Pattern: Ln 1, Col 32) Expected class filter rule.
```

Additionally, if you would rather examine each error message individually,
the `RantCompilerException` class also provides a `GetErrors()` method,
which enumerates the `RantCompilerMessage` objects stored in the instance.

```csharp
try
{
    // ...
}
catch (RantCompilerException ex)
{
    Console.WriteLine("Compilation failed!");
    foreach(RantCompilerMessage msg in ex.GetErrors())
    {
        Console.WriteLine(msg.Message);
        Console.WriteLine("  - Line: {0}", msg.Line);
        Console.WriteLine("  - Column: {0}", msg.Column);
        Console.WriteLine("  - Index: {0}", msg.Index);
    }
}
```

## Runtime errors

A runtime error occurs when a Rant program encounters a problem while it is running,
which makes further execution impossible. It will then throw a `RantRuntimeException`.

One example of a runtime error is a stack overflow, demonstrated below.

```csharp
try
{
    var rant = new RantEngine();
    // This pattern creates a subroutine that continuously calls itself
    var program = RantProgram.CompileString(@"[$[recurse]:[$recurse]][$recurse]");
    rant.Do(program); // No output is generated, so don't even bother capturing it
}
catch (RantRuntimeException ex)
{
    Console.WriteLine(ex.Message);
}
```

The exception message will contain a string explaining what caused the runtime error.

```
(Pattern: Ln 1, Col 15) Exceeded the maximum stack size (64).
```

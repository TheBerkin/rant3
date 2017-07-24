Global variables created by Rant programs can be accessed and manipulated
through the Rant API. On this page is a simple example of how to create a
Rant variable, read it from your C# code, and then modify it through `RantEngine`.

## Setting a variable in Rant

First, a string variable is created by running the following pattern:

```rant
[vs:example;Hello World!]
```

This can be done with the following C# code:

```csharp
var rant = new RantEngine();
var pgmSetVar = RantProgram.CompileString(@"[vs:example;Hello World!]");
rant.Do(pgmSetVar);
```

Another pattern can then be used to retrieve the value of the variable:

```csharp
var pgmPrintVar = RantProgram.CompileString(@"[v:example]");
Console.WriteLine(rant.Do(pgmPrintVar).Main);
// Prints: "Hello World!"
```

## Retrieving the variable in C&#35;

To retrieve the value of the variable in C#, use the `RantEngine` instance's indexer
to retrieve the `RantObject` instance associated with the variable name.

```csharp
Console.WriteLine(rant["example"].Value);
// Prints: "Hello World!"
```

## Setting the variable in C&#35;

The value of the variable can be changed through the same indexer.

```csharp
rant["example"] = new RantObject("Hello from C#!");
```

If you run `pgmPrintVar` again, you will find that the value of `example` has changed to "Hello from C#!".
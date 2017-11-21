If you are a developer who would like to integrate Rant into your C# project with as little effort as possible, this guide is for you.

## Installing Rant

You can easily install Rant in your project by adding **Rant.dll** as a reference.
Rant has no external dependencies, so don't worry about that.

However, if you don't want to manually mess around with DLLs, you can use NuGet instead.
To install the NuGet package for Rant, enter this command into your Package Manager console:

```none
PM> Install-Package Rant
```

Kaboom! Rant is installed.

## Creating an engine context

Before you can use Rant, you must create a `RantEngine` instance. 
The `RantEngine` class is the main class in Rant, and provides access to the runtime.

```csharp
var rant = new RantEngine();
```

Now that you have an instance of `RantEngine`, you can compile and run patterns.

## Running a pattern

Let's start with a very simple program that counts to ten:
```rant
[numfmt:verbal][rep:10][sep:,\s]{[rn]}
```

To run this pattern, you will first need to compile it to a `RantProgram` and then use the `RantEngine.Do` method to run it.

```csharp
// Create the program
var pgm = RantProgram.CompileString(@"[numfmt:verbal][rep:10][sep:,\s]{[rn]}");
// Run the program
var output = rant.Do(pgm);
// Display the output in the console
Console.WriteLine(output.Main);
```

If everything went well, you should see this in the console:

```
one, two, three, four, five, six, seven, eight, nine, ten
```

## Handling multiple outputs

`RantEngine.Do()` can also return multiple strings at once, which are stored in a `RantOutput` object.
The main output is stored in the `Main` property, which might be most often what you'll be using.
However, when multiple channel outputs are returned, they must be accessed using either an iterator
or by indexing the entry you're interested in.

Suppose we run the following:
```csharp
var rant = new RantEngine();
var program = RantProgram.CompileString(@"[chan:a;private;Value A][chan:b;private;Value B]");
var output = rant.Do(program);
foreach (RantOutputEntry entry in output)
{
    Console.WriteLine("{0}: '{1}'", entry.Name, entry.Value);
}
```

This creates a new Rant context, compiles a pattern that writes to two channels: `a` and `b`, 
then runs it and prints the name and value of each channel's output.

If the program runs successfully, it will show the following output in the console:
```
a: 'Value A'
b: 'Value B'
```

If you would rather retrieve the channel outputs by name, simply use the `RantOutput` indexer instead:
```csharp
string a = output["a"]?.Value; // "Value A"
string b = output["b"]?.Value; // "Value B"
```

## Serial programs

But wait, there's more. Rant has _two_ ways of providing us with multiple outputs:

The first way, shown above, is to write to multiple channels and return them in a single `RantOutput` object.

The second way can also write to multiple channels, but it has the added ability of returning multiple `RantOutput` objects from a single program execution.
This is quite useful if you need to return a series of outputs in a specific order. 
Patterns that do this are called **serial patterns**. Likewise, programs compiled from serial patterns are called **serial programs**.

Serial programs work by using the `[yield]` function to dump the current output into a `RantOutput` object, which is then provided to the user code via an iterator.
The program can then continue to run and generate more outputs, and this process can be repeated as many times as needed.

Running a serial program requires slightly different code. You can technically run a serial program with `RantEngine.Do`, but the outputs will be merged into one.
To properly use a serial program, you must use the `RantEngine.DoSerial` method instead.

```csharp
var rant = new RantEngine();
var program = RantProgram.CompileString(@"[rep:10]{[rn][yield]}");
foreach (RantOutput output in rant.DoSerial(program))
{
    Console.WriteLine(output.Main);
}
```

Running this code will generate ten `RantOutput` instances.

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
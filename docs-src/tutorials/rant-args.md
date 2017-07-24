When running a Rant program, you can pass named arguments that can be used like temporary variables within the scope of the pattern.
This can be utilized in games with Rant integration to pass game state information, like player health, quest states, or the weather,
straight to the pattern so that its behavior can adjust accordingly.

Arguments are created using the `RantProgramArgs` class. These arguments can then be accessed via the [`[in]`](/language/functions#in) function.

## Method 1: Adding key/value pairs

The least-involved method for creating arguments is to simply instantiate a new `RantProgramArgs` instance,
then pass it to the `RantEngine.Do` method.

```csharp
var rant = new RantEngine();
var pgm = RantProgram.CompileString(@"Hello, [in:name]!");
var rargs = new RantProgramArgs();
rargs["name"] = "Nicholas";
var output = rant.Do(pgm, args: rargs);

Console.WriteLine(output);
// Output: "Hello, Nicholas!"
```

## Method 2: Object with RantArgAttributes

A `RantProgramArgs` instance can also be created from another object, which will search for string properties and
turn them into arguments, using the property names as the argument names.

Class properties can be decorated with the `RantArgAttribute` to customize the argument name taken from the property.

The following is an example of how this feature is used to turn an instance of a custom class into a set of arguments,
which is then passed to Rant and accessed via `[in]`. 

**Class**
```csharp
class Person
{
    [RantArg("first-name")]
    public string FirstName { get; }
    [RantArg("last-name")]
    public string LastName { get; }
    [RantArg("age")]
    public int Age { get; }

    public Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}
```

**arg-example.rant**
```rant
My name is [in:first-name] [in:last-name], and I'm\s
[if:[lt:[in:age];100];[in:age];ridiculously old].
```

**Program**
```csharp
var rant = new RantEngine();
var pgm = RantProgram.CompileFile("arg-example.rant");
var person1Args = new RantProgramArgs(new Person("Jimmy", "Dipstick", 33));
var person2Args = new RantProgramArgs(new Person("Old", "Jenkins", 102));
Console.WriteLine(rant.Do(pgm, args: person1Args));
Console.WriteLine(rant.Do(pgm, args: person2Args));

// Output:
// My name is Jimmy Dipstick, and I'm 33.
// My name is Old Jenkins, and I'm ridiculously old.
```

## Method 3: Anonymous object

Alternatively, anonymous objects can be used to define the argument values. This is the most concise way
to pass arguments.

**addition.rant**
```rant
[case:sentence]
[numfmt:verbal;[n:[in:a]]]\s
plus [numfmt:verbal;[n:[in:b]]]\s
equals [numfmt:verbal;[add:[in:a];[in:b]]].
```

**Program**
```csharp
var rant = new RantEngine();
var pgm = RantProgram.CompileFile("addition.rant");
Console.WriteLine(rant.Do(pgm, args: new RantProgramArgs(new{ a = 12, b = 26 })));

// Output:
// Twelve plus twenty-six equals thirty-eight.
```
## Carrier class (Rant.Vocabulary.Querying)
**Namespace:** Rant.Vocabulary.Querying

**Inheritance:** Object → Carrier

Represents information that can be used to synchronize query selections based on certain criteria.

```csharp
public sealed class Carrier
```
### Constructors
#### Carrier()
Creates an empty carrier.

```csharp
public Carrier()
```
### Methods
#### AddComponent(CarrierComponentType, params string[])
Adds a component of the specified type and name to the current instance.

```csharp
public void AddComponent(CarrierComponentType type, params string[] values)
```
**Parameters**

- `type`: The type of carrier to add.
- `values`: The names to assign to the component type.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetComponentsOfType(CarrierComponentType)
Iterates through the current instances's components of the specified type.

```csharp
public System.Collections.Generic.IEnumerable<string> GetComponentsOfType(CarrierComponentType type)
```
**Parameters**

- `type`: The type of component to iterate through.
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetTotalCount()
Retreives the total amount of all components.

```csharp
public int GetTotalCount()
```
#### Returns
The total amount of all components.

#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetTypeCount(CarrierComponentType)
Returns how many of a certain carrier component type are assigned to the current instance.

```csharp
public int GetTypeCount(CarrierComponentType type)
```
**Parameters**

- `type`: 
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## CarrierComponentType enum (Rant.Vocabulary.Querying)
**Namespace:** Rant.Vocabulary.Querying

**Inheritance:** Object → ValueType → Enum → CarrierComponentType

Defines carrier types for queries.

```csharp
public enum CarrierComponentType
```
### Fields
#### Associative
Classes must exactly match.

```csharp
public const CarrierComponentType Associative = 3;
```
#### Dissociative
Share no classes.

```csharp
public const CarrierComponentType Dissociative = 1;
```
#### Divergent
Have at least one different class.

```csharp
public const CarrierComponentType Divergent = 5;
```
#### Match
Select the same entry every time.

```csharp
public const CarrierComponentType Match = 0;
```
#### MatchAssociative
Classes must exactly match those of a match carrier entry.

```csharp
public const CarrierComponentType MatchAssociative = 4;
```
#### MatchDissociative
Share no classes with a match carrier entry.

```csharp
public const CarrierComponentType MatchDissociative = 2;
```
#### MatchDivergent
Have at least one different class than a match carrier entry.

```csharp
public const CarrierComponentType MatchDivergent = 6;
```
#### MatchRelational
Share at least one class with a match carrier entry.

```csharp
public const CarrierComponentType MatchRelational = 8;
```
#### MatchUnique
Choose an entry that is different from a match carrier entry.

```csharp
public const CarrierComponentType MatchUnique = 10;
```
#### Relational
Share at least one class.

```csharp
public const CarrierComponentType Relational = 7;
```
#### Rhyme
Choose terms that rhyme.

```csharp
public const CarrierComponentType Rhyme = 11;
```
#### Unique
Never choose the same entry twice.

```csharp
public const CarrierComponentType Unique = 9;
```
### Methods
#### CompareTo(object)
_No Summary_

```csharp
public override int CompareTo(object target)
```
**Parameters**

- `target`: _No Description_
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetTypeCode()
_No Summary_

```csharp
public override System.TypeCode GetTypeCode()
```
#### HasFlag(Enum)
_No Summary_

```csharp
public override bool HasFlag(Enum flag)
```
**Parameters**

- `flag`: _No Description_
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### ToString(string)
_No Summary_

```csharp
public override string ToString(string format)
```
**Parameters**

- `format`: _No Description_
#### ToString(IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString().

_No Summary_

```csharp
public override string ToString(IFormatProvider provider)
```
**Parameters**

- `provider`: _No Description_
#### ToString(string, IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString(String).

_No Summary_

```csharp
public override string ToString(string format, IFormatProvider provider)
```
**Parameters**

- `format`: _No Description_
- `provider`: _No Description_
## ChannelVisibility enum (Rant.Core.Output)
**Namespace:** Rant.Core.Output

**Inheritance:** Object → ValueType → Enum → ChannelVisibility

Provides visibility settings for output channels.

```csharp
public enum ChannelVisibility
```
### Fields
#### Internal
Channel outputs only to itself and any parent channels also set to Internal.

```csharp
public const ChannelVisibility Internal = 2;
```
#### Private
Channel outputs only to itself.

```csharp
public const ChannelVisibility Private = 1;
```
#### Public
Channel outputs to itself and 'main'.

```csharp
public const ChannelVisibility Public = 0;
```
### Methods
#### CompareTo(object)
_No Summary_

```csharp
public override int CompareTo(object target)
```
**Parameters**

- `target`: _No Description_
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetTypeCode()
_No Summary_

```csharp
public override System.TypeCode GetTypeCode()
```
#### HasFlag(Enum)
_No Summary_

```csharp
public override bool HasFlag(Enum flag)
```
**Parameters**

- `flag`: _No Description_
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### ToString(string)
_No Summary_

```csharp
public override string ToString(string format)
```
**Parameters**

- `format`: _No Description_
#### ToString(IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString().

_No Summary_

```csharp
public override string ToString(IFormatProvider provider)
```
**Parameters**

- `provider`: _No Description_
#### ToString(string, IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString(String).

_No Summary_

```csharp
public override string ToString(string format, IFormatProvider provider)
```
**Parameters**

- `format`: _No Description_
- `provider`: _No Description_
## ClassFilterRule class (Rant.Vocabulary.Querying)
**Namespace:** Rant.Vocabulary.Querying

**Inheritance:** Object → ClassFilterRule

Defines a query filter for a single dictionary entry class.

```csharp
public sealed class ClassFilterRule
```
### Constructors
#### ClassFilterRule(string)
Initializes a new ClassFilterRule that checks for a positive match to the specified class.

```csharp
public ClassFilterRule(string className)
```
**Parameters**

- `className`: The name of the class to search for.
#### ClassFilterRule(string, bool)
Initializes a new ClassFilterRule that checks for a positive or negative match to the specified class.

```csharp
public ClassFilterRule(string className, bool shouldMatch)
```
**Parameters**

- `className`: The name of the class to search for.
- `shouldMatch`: Determines whether the filter item expects a positive or negative match for the class.
### Properties
#### Class
The name of the class to search for.

```csharp
public string Class
{
    get;
    set;
}
```
#### ShouldMatch
Determines whether the filter item expects a positive or negative match for the class.

```csharp
public bool ShouldMatch
{
    get;
    set;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## EnglishNumberVerbalizer class (Rant.Formats)
**Namespace:** Rant.Formats

**Inheritance:** Object → NumberVerbalizer → EnglishNumberVerbalizer

Represents a number verbalizer for English (US).

```csharp
public sealed class EnglishNumberVerbalizer : Rant.Formats.NumberVerbalizer
```
### Constructors
#### EnglishNumberVerbalizer()
_No Summary_

```csharp
public EnglishNumberVerbalizer()
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### Verbalize(long)
Verbalizes the specified value.

```csharp
public virtual string Verbalize(long number)
```
**Parameters**

- `number`: The number to verbalize.
## EnglishPluralizer class (Rant.Formats)
**Namespace:** Rant.Formats

**Inheritance:** Object → Pluralizer → EnglishPluralizer

Pluralizer for English nouns.

```csharp
public sealed class EnglishPluralizer : Rant.Formats.Pluralizer
```
### Constructors
#### EnglishPluralizer()
_No Summary_

```csharp
public EnglishPluralizer()
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### Pluralize(string)
Determines the plural form of the specified English noun.

```csharp
public virtual string Pluralize(string input)
```
**Parameters**

- `input`: The singular form of the noun to pluralize.
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## GermanNumberVerbalizer class (Rant.Formats)
**Namespace:** Rant.Formats

**Inheritance:** Object → NumberVerbalizer → GermanNumberVerbalizer

Represents a number verbalizer for Standard German.

```csharp
public sealed class GermanNumberVerbalizer : Rant.Formats.NumberVerbalizer
```
### Constructors
#### GermanNumberVerbalizer()
_No Summary_

```csharp
public GermanNumberVerbalizer()
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### Verbalize(long)
Verbalizes the specified value.

```csharp
public virtual string Verbalize(long number)
```
**Parameters**

- `number`: The number to verbalize.
## IRantFunction interface (Rant.Metadata)
**Namespace:** Rant.Metadata

**Inheritance:** Object → IRantFunction

Provides access to metadata for a Rant function overload.

```csharp
public interface IRantFunction
```
### Properties
#### Description
Gets the description for the function overload.

```csharp
public abstract virtual string Description
{
    get;
}
```
#### HasParamArray
Indicates whether the last parameter accepts multiple values.

```csharp
public abstract virtual bool HasParamArray
{
    get;
}
```
#### Name
Gets the name of the function.

```csharp
public abstract virtual string Name
{
    get;
}
```
#### ParamCount
Gets the number of parameters accepted by the function overload.

```csharp
public abstract virtual int ParamCount
{
    get;
}
```
### Methods
#### GetParameters()
Enumerates the parameters for the function overload.

```csharp
public abstract virtual System.Collections.Generic.IEnumerable<Rant.Metadata.IRantParameter> GetParameters()
```
## IRantFunctionGroup interface (Rant.Metadata)
**Namespace:** Rant.Metadata

**Inheritance:** Object → IRantFunctionGroup

Provides access to metadata for a group of overloads for a specific Rant function.

```csharp
public interface IRantFunctionGroup
```
### Properties
#### Name
Gets the name of the function.

```csharp
public abstract virtual string Name
{
    get;
}
```
#### Overloads
Gets the available overloads for the function.

```csharp
public abstract virtual IEnumerable<Rant.Metadata.IRantFunction> Overloads
{
    get;
}
```
## IRantModeValue interface (Rant.Metadata)
**Namespace:** Rant.Metadata

**Inheritance:** Object → IRantModeValue

Provides information on Rant's mode values, like number formats and synchronizer types.

```csharp
public interface IRantModeValue
```
### Properties
#### Description
Gets the description for the value.

```csharp
public abstract virtual string Description
{
    get;
}
```
#### Name
Gets the name of the value.

```csharp
public abstract virtual string Name
{
    get;
}
```
## IRantParameter interface (Rant.Metadata)
**Namespace:** Rant.Metadata

**Inheritance:** Object → IRantParameter

Provides access to metadata for a Rant function parameter.

```csharp
public interface IRantParameter
```
### Properties
#### Description
Gets the description for the parameter.

```csharp
public abstract virtual string Description
{
    get;
}
```
#### IsParams
Indicates whether the parameter accepts multiple values.

```csharp
public abstract virtual bool IsParams
{
    get;
}
```
#### Name
Gets the name of the parameter.

```csharp
public abstract virtual string Name
{
    get;
}
```
#### RantType
Gets the data type accepted by the parameter.

```csharp
public abstract virtual RantFunctionParameterType RantType
{
    get;
}
```
### Methods
#### GetEnumValues()
Enumerates all possible values for flag and mode parameters.

```csharp
public abstract virtual System.Collections.Generic.IEnumerable<Rant.Metadata.IRantModeValue> GetEnumValues()
```
## NumberVerbalizer class (Rant.Formats)
**Namespace:** Rant.Formats

**Inheritance:** Object → NumberVerbalizer

The base class for all number verbalizers.

```csharp
public abstract class NumberVerbalizer
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### Finalize()
_No Summary_

```csharp
protected override void Finalize()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### MemberwiseClone()
_No Summary_

```csharp
protected override object MemberwiseClone()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### Verbalize(long)
Verbalizes the specified value.

```csharp
public abstract virtual string Verbalize(long number)
```
**Parameters**

- `number`: The number to verbalize.
## Pluralizer class (Rant.Formats)
**Namespace:** Rant.Formats

**Inheritance:** Object → Pluralizer

The base class for pluralizers, which infer the plural form of a given noun.

```csharp
public abstract class Pluralizer
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### Finalize()
_No Summary_

```csharp
protected override void Finalize()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### MemberwiseClone()
_No Summary_

```csharp
protected override object MemberwiseClone()
```
#### Pluralize(string)
Converts the specified input noun to a plural version.

```csharp
public abstract virtual string Pluralize(string input)
```
**Parameters**

- `input`: The noun to convert.
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## QuotationMarks class (Rant.Formats)
**Namespace:** Rant.Formats

**Inheritance:** Object → QuotationMarks

Represents a configuration for quotation marks.

```csharp
public sealed class QuotationMarks
```
### Constructors
#### QuotationMarks()
Initializes a new instance of the QuotationFormat class with the default configuration.

```csharp
public QuotationMarks()
```
#### QuotationMarks(char, char, char, char)
Initializes a new instance of the QuotationFormat class with the specified quotation marks.

```csharp
public QuotationMarks(char openPrimary, char closePrimary, char openSecondary, char closeSecondary)
```
**Parameters**

- `openPrimary`: The opening primary quote.
- `closePrimary`: The closing primary quote.
- `openSecondary`: The opening secondary quote.
- `closeSecondary`: The closing secondary quote.
### Properties
#### ClosingPrimary
The closing primary quotation mark.

```csharp
public char ClosingPrimary
{
    get;
}
```
#### ClosingSecondary
The closing secondary quotation mark.

```csharp
public char ClosingSecondary
{
    get;
}
```
#### OpeningPrimary
The opening primary quotation mark.

```csharp
public char OpeningPrimary
{
    get;
}
```
#### OpeningSecondary
The opening secondary quotation mark.

```csharp
public char OpeningSecondary
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
Returns a string representation of the configuration.

```csharp
public virtual string ToString()
```
## RantArgAttribute class (Rant)
**Namespace:** Rant

**Inheritance:** Object → Attribute → RantArgAttribute

Attribute used to change the name of an argument pulled from a field or property.

```csharp
public sealed class RantArgAttribute : System.Attribute, System.Runtime.InteropServices._Attribute
```
### Constructors
#### RantArgAttribute(string)
Creates a new RantArgAttribute with the specified name.

```csharp
public RantArgAttribute(string name)
```
**Parameters**

- `name`: The new name to assign to the argument.
### Properties
#### Name
The new name to assign to the argument.

```csharp
public string Name
{
    get;
}
```
#### TypeId
_No Summary_

```csharp
public override object TypeId
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### IsDefaultAttribute()
_No Summary_

```csharp
public override bool IsDefaultAttribute()
```
#### Match(object)
_No Summary_

```csharp
public override bool Match(object obj)
```
**Parameters**

- `obj`: _No Description_
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantCompilerException class (Rant)
**Namespace:** Rant

**Inheritance:** Object → Exception → RantCompilerException

Represents an error raised by Rant during pattern compilation.

```csharp
public sealed class RantCompilerException : System.Exception, System.Runtime.Serialization.ISerializable, System.Runtime.InteropServices._Exception
```
### Properties
#### Data
_No Summary_

```csharp
public override IDictionary Data
{
    get;
}
```
#### ErrorCount
Gets the number of errors returned by the compiler.

```csharp
public int ErrorCount
{
    get;
}
```
#### HelpLink
_No Summary_

```csharp
public override string HelpLink
{
    get;
    set;
}
```
#### HResult
_No Summary_

```csharp
public override int HResult
{
    get;
    protected set;
}
```
#### InnerException
_No Summary_

```csharp
public override Exception InnerException
{
    get;
}
```
#### InternalError
Indicates whether the exception is the result of an internal engine error.

```csharp
public bool InternalError
{
    get;
}
```
#### Message
_No Summary_

```csharp
public override string Message
{
    get;
}
```
#### Source
_No Summary_

```csharp
public override string Source
{
    get;
    set;
}
```
#### SourceName
The name of the source pattern on which the error occurred.

```csharp
public string SourceName
{
    get;
}
```
#### StackTrace
_No Summary_

```csharp
public override string StackTrace
{
    get;
}
```
#### TargetSite
_No Summary_

```csharp
public override MethodBase TargetSite
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetBaseException()
_No Summary_

```csharp
public override System.Exception GetBaseException()
```
#### GetErrors()
Enumerates the errors collected from the compiler.

```csharp
public System.Collections.Generic.IEnumerable<Rant.RantCompilerMessage> GetErrors()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetObjectData(SerializationInfo, StreamingContext)
_No Summary_

```csharp
public override void GetObjectData(SerializationInfo info, StreamingContext context)
```
**Parameters**

- `info`: _No Description_
- `context`: _No Description_
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantCompilerMessage class (Rant)
**Namespace:** Rant

**Inheritance:** Object → RantCompilerMessage

Represents a message emitted by the Rant compiler while performing a job.

```csharp
public sealed class RantCompilerMessage
```
### Properties
#### Column
The column on which the message was generated.

```csharp
public int Column
{
    get;
}
```
#### Index
The character index on which the message was generated.

```csharp
public int Index
{
    get;
}
```
#### Length
The length, in characters, of the code snippet to which the message pertains.

```csharp
public int Length
{
    get;
}
```
#### Line
The line on which the message was generated.

```csharp
public int Line
{
    get;
}
```
#### Message
The message text.

```csharp
public string Message
{
    get;
}
```
#### Source
The source path of the pattern being compiled when the message was generated.

```csharp
public string Source
{
    get;
}
```
#### Type
The type of message.

```csharp
public RantCompilerMessageType Type
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
Generates a string representation of the message.

```csharp
public virtual string ToString()
```
## RantCompilerMessageType enum (Rant)
**Namespace:** Rant

**Inheritance:** Object → ValueType → Enum → RantCompilerMessageType

Defines message types used by the Rant compiler.

```csharp
public enum RantCompilerMessageType
```
### Fields
#### Error
Indicates a problem that made compilation impossible, usually a syntax error.

```csharp
public const RantCompilerMessageType Error = 1;
```
#### Warning
Indicates a problem that did not interfere with compilation.

```csharp
public const RantCompilerMessageType Warning = 0;
```
### Methods
#### CompareTo(object)
_No Summary_

```csharp
public override int CompareTo(object target)
```
**Parameters**

- `target`: _No Description_
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetTypeCode()
_No Summary_

```csharp
public override System.TypeCode GetTypeCode()
```
#### HasFlag(Enum)
_No Summary_

```csharp
public override bool HasFlag(Enum flag)
```
**Parameters**

- `flag`: _No Description_
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### ToString(string)
_No Summary_

```csharp
public override string ToString(string format)
```
**Parameters**

- `format`: _No Description_
#### ToString(IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString().

_No Summary_

```csharp
public override string ToString(IFormatProvider provider)
```
**Parameters**

- `provider`: _No Description_
#### ToString(string, IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString(String).

_No Summary_

```csharp
public override string ToString(string format, IFormatProvider provider)
```
**Parameters**

- `format`: _No Description_
- `provider`: _No Description_
## RantDependencyResolver class (Rant.Resources)
**Namespace:** Rant.Resources

**Inheritance:** Object → RantDependencyResolver

Default class for package depdendency resolving.

```csharp
public class RantDependencyResolver
```
### Constructors
#### RantDependencyResolver()
_No Summary_

```csharp
public RantDependencyResolver()
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### Finalize()
_No Summary_

```csharp
protected override void Finalize()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### MemberwiseClone()
_No Summary_

```csharp
protected override object MemberwiseClone()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### TryResolvePackage(RantPackageDependency, out RantPackage&)
_No Summary_

```csharp
public virtual bool TryResolvePackage(RantPackageDependency depdendency, out RantPackage& package)
```
**Parameters**

- `depdendency`: _No Description_
- `package`: _No Description_
## RantDictionary class (Rant.Vocabulary)
**Namespace:** Rant.Vocabulary

**Inheritance:** Object → RantDictionary

Represents a dictionary that can be loaded and queried by Rant.

```csharp
public sealed class RantDictionary
```
### Constructors
#### RantDictionary()
Initializes a new instance of the  class with no tables.

```csharp
public RantDictionary()
```
#### RantDictionary(IEnumerable<Rant.Vocabulary.RantDictionaryTable>)
Initializes a new instance of the  class with the specified set of tables.

```csharp
public RantDictionary(IEnumerable<Rant.Vocabulary.RantDictionaryTable> tables)
```
**Parameters**

- `tables`: The tables to store in the dictionary.
### Properties
#### EnableWeighting
Determines whether tables will favor weighted distribution, if available.
            Weighted distribution has a significantl impact on performance.

```csharp
public bool EnableWeighting
{
    get;
    set;
}
```
#### IncludedHiddenClasses
Gets all currently exposed hidden classes.

```csharp
public IEnumerable<string> IncludedHiddenClasses
{
    get;
}
```
### Indexers
#### this[string name]
Gets the table with the specified name.

```csharp
public RantDictionaryTable this[string name]
{
    get;
}
```
### Methods
#### AddTable(RantDictionaryTable)
Adds a new  object to the dictionary.

```csharp
public void AddTable(RantDictionaryTable table)
```
**Parameters**

- `table`: The table to add.
#### ClassExposed(string)
Determines whether the specified class has been manually exposed (overriding hidden status).

```csharp
public bool ClassExposed(string className)
```
**Parameters**

- `className`: The name of the class to check.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### ExcludeHiddenClass(string)
Unexposes a hidden class from query results.

```csharp
public void ExcludeHiddenClass(string hiddenClassName)
```
**Parameters**

- `hiddenClassName`: The name of the hidden class to unexpose.
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetTables()
Enumerates the tables contained in the current RantDictionary instance.

```csharp
public System.Collections.Generic.IEnumerable<Rant.Vocabulary.RantDictionaryTable> GetTables()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### IncludeHiddenClass(string)
Exposes a hidden class to query results.

```csharp
public void IncludeHiddenClass(string hiddenClassName)
```
**Parameters**

- `hiddenClassName`: The name of the hidden class to expose.
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantDictionaryEntry class (Rant.Vocabulary)
**Namespace:** Rant.Vocabulary

**Inheritance:** Object → RantDictionaryEntry

Stores information about a dictionary entry.

```csharp
public sealed class RantDictionaryEntry
```
### Constructors
#### RantDictionaryEntry(RantDictionaryTerm[])
Creates a new instance of the  object from the specified term array.

```csharp
public RantDictionaryEntry(RantDictionaryTerm[] terms)
```
**Parameters**

- `terms`: The terms in the entry.
#### RantDictionaryEntry(string[], IEnumerable<string>, float)
Creates a new  object from the specified term array, classes, and weight.

```csharp
public RantDictionaryEntry(string[] terms, IEnumerable<string> classes, float weight = 1f)
```
**Parameters**

- `terms`: The terms in the entry.
- `classes`: The classes associated with the entry.
- `weight`: The weight of the entry.
#### RantDictionaryEntry(IEnumerable<Rant.Vocabulary.RantDictionaryTerm>, IEnumerable<string>, float)
Creates a new  object from the specified term collection, classes, and weight.

```csharp
public RantDictionaryEntry(IEnumerable<Rant.Vocabulary.RantDictionaryTerm> terms, IEnumerable<string> classes, float weight = 1f)
```
**Parameters**

- `terms`: The terms in the entry.
- `classes`: The classes associated with the entry.
- `weight`: The weight of the entry.
### Properties
#### ClassCount
Gets the number of classes in the current entry.

```csharp
public int ClassCount
{
    get;
}
```
#### HasClasses
Returns whether or not the entry has classes.

```csharp
public bool HasClasses
{
    get;
}
```
#### TermCount
Gets the number of terms stored in the current entry.

```csharp
public int TermCount
{
    get;
}
```
#### Weight
Gets the weight value of the entry.

```csharp
public float Weight
{
    get;
    set;
}
```
### Indexers
#### this[int index]
Gets or sets the term at the specified index.

```csharp
public RantDictionaryTerm this[int index]
{
    get;
    set;
}
```
### Methods
#### AddClass(string, bool)
Adds the specified class to the current entry.

```csharp
public void AddClass(string className, bool optional = False)
```
**Parameters**

- `className`: The name of the class.
- `optional`: Specifies whether the class is optional in carrier associations.
#### ContainsClass(string)
Returns a boolean valie indicating whether the current entry contains the specified class.

```csharp
public bool ContainsClass(string className)
```
**Parameters**

- `className`: The class to search for.
#### ContainsMetadataKey(string)
Determines if the entry contains metadata attached to the specified key.

```csharp
public bool ContainsMetadataKey(string key)
```
**Parameters**

- `key`: The key to search for.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetClasses()
Returns a collection of classes assigned to the current entry.

```csharp
public System.Collections.Generic.IEnumerable<string> GetClasses()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetMetadata(string)
Locates and returns the metadata value associated with the specified key. Returns Null if not found.

```csharp
public object GetMetadata(string key)
```
**Parameters**

- `key`: The key of the metadata to retrieve.
#### GetMetadataKeys()
Enumerates all the metadata keys contained in the entry.

```csharp
public System.Collections.Generic.IEnumerable<string> GetMetadataKeys()
```
#### GetOptionalClasses()
Returns a collection of the optional classes assigned to the current entry.

```csharp
public System.Collections.Generic.IEnumerable<string> GetOptionalClasses()
```
#### GetRequiredClasses()
Returns a collection of required (non-optional) classes assigned to the current entry.

```csharp
public System.Collections.Generic.IEnumerable<string> GetRequiredClasses()
```
#### GetTerms()
Enumerates the terms stored in the current entry.

```csharp
public System.Collections.Generic.IEnumerable<Rant.Vocabulary.RantDictionaryTerm> GetTerms()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### RemoveClass(string)
Removes the class with the specified name from the current entry.

```csharp
public void RemoveClass(string className)
```
**Parameters**

- `className`: The name of the class to remove.
#### RemoveMetadata(string)
Removes the metadata with the specified key from the entry.

```csharp
public bool RemoveMetadata(string key)
```
**Parameters**

- `key`: The key of the metadata entry to remove.
#### SetMetadata(string, object)
Sets a metadata value under the specified key in the entry.

```csharp
public void SetMetadata(string key, object value)
```
**Parameters**

- `key`: The key to store the data under.
- `value`: The value to store.
#### ToString()
Returns a string representation of the current  instance.

```csharp
public virtual string ToString()
```
## RantDictionaryTable class (Rant.Vocabulary)
**Namespace:** Rant.Vocabulary

**Inheritance:** Object → RantResource → RantDictionaryTable

Represents a named collection of dictionary entries.

```csharp
public sealed class RantDictionaryTable : Rant.Resources.RantResource
```
### Constructors
#### RantDictionaryTable(string, int, HashSet<string>)
Initializes a new instance of the RantDictionaryTable class with the specified name and term count.

```csharp
public RantDictionaryTable(string name, int termsPerEntry, HashSet<string> hidden = null)
```
**Parameters**

- `name`: The name of the table.
- `termsPerEntry`: The number of terms to store in each entry.
- `hidden`: Collection of hidden classes.
### Properties
#### CacheNeedsRebuild
Indicates whether the cache needs to be rebuilt.

```csharp
public bool CacheNeedsRebuild
{
    get;
}
```
#### EnableWeighting
Determines whether weights are enabled on this table.

```csharp
public bool EnableWeighting
{
    get;
    set;
}
```
#### EntryCount
Gets the number of entries stored in the table.

```csharp
public int EntryCount
{
    get;
}
```
#### HiddenClasses
Gets the hidden classes of the table.

```csharp
public IEnumerable<string> HiddenClasses
{
    get;
}
```
#### Language
Gets the language code associated with the table (not yet used).

```csharp
public string Language
{
    get;
    set;
}
```
#### Name
Gets the name of the table.

```csharp
public string Name
{
    get;
    set;
}
```
#### TermsPerEntry
Gets the number of terms required for entries contained in the current table.

```csharp
public int TermsPerEntry
{
    get;
    set;
}
```
### Indexers
#### this[int index]
Gets the entry at the specified index in the current  object.

```csharp
public RantDictionaryEntry this[int index]
{
    get;
}
```
### Methods
#### AddEntry(RantDictionaryEntry)
Adds the specified entry to the table.

```csharp
public bool AddEntry(RantDictionaryEntry entry)
```
**Parameters**

- `entry`: The entry to add to the table.
#### Returns
True if successfully added; otherwise, False.

#### AddSubtype(string, int)
Adds a subtype of the specified name to the table.
            If a subtype with the name already exists, it will be overwritten.
            Subtypes are case insensitive.
            If the name is not a valid identifier string, it will not be accepted.

```csharp
public bool AddSubtype(string subtypeName, int index)
```
**Parameters**

- `subtypeName`: The name of the subtype to add.
- `index`: The term index to associate with the name.
#### Returns
FALSE if the name was not a valid identifier or the index was out of range. TRUE if the operation was
            successful.

#### ContainsClass(string)
Returns a boolean value indicating whether the current  instance contains one or more entries containing the specified class name.

```csharp
public bool ContainsClass(string clName)
```
**Parameters**

- `clName`: The class name to search for.
#### ContainsEntry(RantDictionaryEntry)
Checks if the table contains the specified entry.

```csharp
public bool ContainsEntry(RantDictionaryEntry entry)
```
**Parameters**

- `entry`: The entry to search for.
#### Returns
True if found, False if not.

#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### FromStream(string, Stream)
Loads a table from the specified stream.

```csharp
public static Rant.Vocabulary.RantDictionaryTable FromStream(string origin, Stream stream)
```
**Parameters**

- `origin`: The origin of the stream. This will typically be a file path or package name.
- `stream`: The stream to load the table from.
#### GetClasses()
Searches entries in the current table and enumerates every single distinct class found.

```csharp
public System.Collections.Generic.IEnumerable<string> GetClasses()
```
#### GetEntries()
Enumerates the entries stored in the table.

```csharp
public System.Collections.Generic.IEnumerable<Rant.Vocabulary.RantDictionaryEntry> GetEntries()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetSubtypeIndex(string)
Retrieves the term index assigned to the specified subtype.
            If the subtype is not found, the method will return -1.
            If the subtype is a null, whitespace, or an empty string, the method will return 0.

```csharp
public int GetSubtypeIndex(string subtype)
```
**Parameters**

- `subtype`: The subtype to look up.
#### GetSubtypes()
Enumerates the subtypes contained in the current table.

```csharp
public System.Collections.Generic.IEnumerable<string> GetSubtypes()
```
#### GetSubtypesForIndex(int)
Enumerates the subtypes associated with the specified term index.

```csharp
public System.Collections.Generic.IEnumerable<string> GetSubtypesForIndex(int index)
```
**Parameters**

- `index`: The index to get subtypes for.
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### HideClass(string)
Hides the specified class.

```csharp
public bool HideClass(string className)
```
**Parameters**

- `className`: The name of the class to hide.
#### IsClassHidden(string)
Determines whether the specified class is hidden by the table.

```csharp
public bool IsClassHidden(string className)
```
**Parameters**

- `className`: The name of the class to check.
#### Merge(RantDictionaryTable)
Adds another table's entries to the current table, given that they share the same name and term count.

```csharp
public bool Merge(RantDictionaryTable other)
```
**Parameters**

- `other`: The table whose entries will be added to the current instance.
#### Returns
True if merge succeeded; otherwise, False.

#### RebuildCache()
Optimizes the table. Call this after writing items to the table or removing items from a table.
            If you're writing or removing multiple items, call this after all the actions have been performed.

```csharp
public void RebuildCache()
```
#### RemoveEntry(RantDictionaryEntry)
Removes the specified entry from the table.

```csharp
public bool RemoveEntry(RantDictionaryEntry entry)
```
**Parameters**

- `entry`: The entry to remove from the table.
#### Returns
True if successfully removed; otherwise, False.

#### RemoveSubtype(string)
Removes the specified subtype from the table, if it exists.
            Subtypes are case insensitive.

```csharp
public bool RemoveSubtype(string subtypeName)
```
**Parameters**

- `subtypeName`: The name of the subtype to remove.
#### Returns
TRUE if the subtype was found and removed. FALSE if the subtype was not found.

#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### UnhideClass(string)
Unhides the specified class.

```csharp
public bool UnhideClass(string className)
```
**Parameters**

- `className`: The name of the class to unhide.
## RantDictionaryTerm class (Rant.Vocabulary)
**Namespace:** Rant.Vocabulary

**Inheritance:** Object → RantDictionaryTerm

Represents a single term of a dictionary entry.

```csharp
public sealed class RantDictionaryTerm
```
### Constructors
#### RantDictionaryTerm(string, int)
Intializes a new instance of the  class with the specified value string.

```csharp
public RantDictionaryTerm(string value, int splitIndex = -1)
```
**Parameters**

- `value`: The value of the term.
- `splitIndex`: The split index of the term value. Specify -1 for no split.
#### RantDictionaryTerm(string, string)
Intializes a new instance of the  class with the specified value and pronunciation
            strings.

```csharp
public RantDictionaryTerm(string value, string pronunciation)
```
**Parameters**

- `value`: The value of the term.
- `pronunciation`: The pronunciation of the term value.
#### RantDictionaryTerm(string, string, int, int)
Intializes a new instance of the  class with the specified value, pronunciation, and
            split indices.

```csharp
public RantDictionaryTerm(string value, string pronunciation, int valueSplitIndex, int pronSplitIndex)
```
**Parameters**

- `value`: The value of the term.
- `pronunciation`: The pronunciation of the term value.
- `valueSplitIndex`: The split index of the term value. Specify -1 for no split.
- `pronSplitIndex`: The split index of the term pronunciation string. Specify -1 for no split. Must be
            positive if the value is split and pronunciation data is present.
### Properties
#### IsSplit
Determines whether the term is a split word.

```csharp
public bool IsSplit
{
    get;
}
```
#### LeftSide
Gets the term value substring on the left side of the split.

```csharp
public string LeftSide
{
    get;
}
```
#### Pronunciation
The pronunciation of the term.

```csharp
public string Pronunciation
{
    get;
    set;
}
```
#### PronunciationSplitIndex
Gets the split index of the term pronunciation string.

```csharp
public int PronunciationSplitIndex
{
    get;
    set;
}
```
#### RightSide
Gets the term value substring on the right side of the split.

```csharp
public string RightSide
{
    get;
}
```
#### SyllableCount
The number of syllables in the pronunciation string.

```csharp
public int SyllableCount
{
    get;
}
```
#### Syllables
An array containing the individual syllables of the pronunciation string.

```csharp
public string[] Syllables
{
    get;
}
```
#### Value
The value string of the term.

```csharp
public string Value
{
    get;
    set;
}
```
#### ValueSplitIndex
Gets the split index of the term value.

```csharp
public int ValueSplitIndex
{
    get;
    set;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantEngine class (Rant)
**Namespace:** Rant

**Inheritance:** Object → RantEngine

The central class of the Rant engine that allows the execution of patterns.

```csharp
public sealed class RantEngine
```
### Constructors
#### RantEngine()
Creates a new RantEngine object without a dictionary.

```csharp
public RantEngine()
```
#### RantEngine(RantDictionary)
Creates a new RantEngine object with the specified vocabulary.

```csharp
public RantEngine(RantDictionary dictionary)
```
**Parameters**

- `dictionary`: The vocabulary to load in this instance.
### Properties
#### DependencyResolver
Gets or sets the depdendency resolver used for packages.

```csharp
public RantDependencyResolver DependencyResolver
{
    get;
    set;
}
```
#### Dictionary
The vocabulary associated with this instance.

```csharp
public RantDictionary Dictionary
{
    get;
    set;
}
```
#### Format
The current formatting settings for the engine.

```csharp
public RantFormat Format
{
    get;
    set;
}
```
#### MaxStackSize
Gets or sets the maximum stack size allowed for a pattern.

```csharp
public static int MaxStackSize
{
    get;
    set;
}
```
#### PreserveCarrierState
Specifies whether to preserve carrier states between patterns.

```csharp
public bool PreserveCarrierState
{
    get;
    set;
}
```
### Indexers
#### this[string name]
Accesses global variables.

```csharp
public RantObject this[string name]
{
    get;
    set;
}
```
### Fields
#### Flags
The currently set flags.

```csharp
public readonly HashSet<string> Flags;
```
### Methods
#### Do(string, int, double, RantProgramArgs)

!!! warning
    **This item is deprecated.**
    Use an overload of Do() that accepts a RantProgram instead of a string.

Compiles the specified string into a pattern, executes it, and returns the resulting output.

```csharp
public Rant.RantOutput Do(string input, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The input string to execute.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### Do(RantProgram, int, double, RantProgramArgs)
Executes the specified pattern and returns the resulting output.

```csharp
public Rant.RantOutput Do(RantProgram input, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The pattern to execute.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### Do(string, long, int, double, RantProgramArgs)

!!! warning
    **This item is deprecated.**
    Use an overload of Do() that accepts a RantProgram instead of a string.

Compiles the specified string into a pattern, executes it using a custom seed, and returns the resulting output.

```csharp
public Rant.RantOutput Do(string input, long seed, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The input string to execute.
- `seed`: The seed to generate output with.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### Do(string, RNG, int, double, RantProgramArgs)

!!! warning
    **This item is deprecated.**
    Use an overload of Do() that accepts a RantProgram instead of a string.

Compiles the specified string into a pattern, executes it using a custom RNG, and returns the resulting output.

```csharp
public Rant.RantOutput Do(string input, RNG rng, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The input string to execute.
- `rng`: The random number generator to use when generating output.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### Do(RantProgram, long, int, double, RantProgramArgs)
Executes the specified pattern using a custom seed and returns the resulting output.

```csharp
public Rant.RantOutput Do(RantProgram input, long seed, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The pattern to execute.
- `seed`: The seed to generate output with.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### Do(RantProgram, RNG, int, double, RantProgramArgs)
Executes the specified pattern using a custom random number generator and returns the resulting output.

```csharp
public Rant.RantOutput Do(RantProgram input, RNG rng, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The pattern to execute.
- `rng`: The random number generator to use when generating output.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoFile(string, int, double, RantProgramArgs)

!!! warning
    **This item is deprecated.**
    Use an overload of Do() that accepts a RantProgram instead of a string.

Loads the file located at the specified path and executes it, returning the resulting output.

```csharp
public Rant.RantOutput DoFile(string path, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `path`: The path to the file to execute.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoFile(string, long, int, double, RantProgramArgs)
Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.

```csharp
public Rant.RantOutput DoFile(string path, long seed, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `path`: The path to the file to execute.
- `seed`: The seed to generate output with.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoFile(string, RNG, int, double, RantProgramArgs)
Loads the file located at the specified path and executes it using a custom seed, returning the resulting output.

```csharp
public Rant.RantOutput DoFile(string path, RNG rng, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `path`: The path to the file to execute.
- `rng`: The random number generator to use when generating output.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoName(string, int, double, RantProgramArgs)
Executes a pattern that has been loaded from a package and returns the resulting output.

```csharp
public Rant.RantOutput DoName(string patternName, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `patternName`: The name of the pattern to execute.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoName(string, long, int, double, RantProgramArgs)
Executes a pattern that has been loaded from a package and returns the resulting output.

```csharp
public Rant.RantOutput DoName(string patternName, long seed, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `patternName`: The name of the pattern to execute.
- `seed`: The seed to generate output with.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoName(string, RNG, int, double, RantProgramArgs)
Executes a pattern that has been loaded from a package using a custom random number generator and returns the resulting
            output.

```csharp
public Rant.RantOutput DoName(string patternName, RNG rng, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `patternName`: The name of the pattern to execute.
- `rng`: The random number generator to use when generating output.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoSerial(RantProgram, int, double, RantProgramArgs)
Executes the specified pattern and returns a series of outputs.

```csharp
public System.Collections.Generic.IEnumerable<Rant.RantOutput> DoSerial(RantProgram input, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The pattern to execute.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoSerial(string, int, double, RantProgramArgs)

!!! warning
    **This item is deprecated.**
    Use an overload of DoSerial() that accepts a RantProgram instead of a string.

Executes the specified pattern and returns a series of outputs.

```csharp
public System.Collections.Generic.IEnumerable<Rant.RantOutput> DoSerial(string input, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The pattern to execute.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoSerial(RantProgram, long, int, double, RantProgramArgs)
Executes the specified pattern and returns a series of outputs.

```csharp
public System.Collections.Generic.IEnumerable<Rant.RantOutput> DoSerial(RantProgram input, long seed, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The patten to execute.
- `seed`: The seed to generate output with.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoSerial(RantProgram, RNG, int, double, RantProgramArgs)
Executes the specified pattern and returns a series of outputs.

```csharp
public System.Collections.Generic.IEnumerable<Rant.RantOutput> DoSerial(RantProgram input, RNG rng, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The pattero to execute.
- `rng`: The random number generator to use when generating output.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoSerial(string, long, int, double, RantProgramArgs)

!!! warning
    **This item is deprecated.**
    Use an overload of DoSerial() that accepts a RantProgram instead of a string.

Executes the specified pattern and returns a series of outputs.

```csharp
public System.Collections.Generic.IEnumerable<Rant.RantOutput> DoSerial(string input, long seed, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The pattern to execute.
- `seed`: The seed to generate output with.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### DoSerial(string, RNG, int, double, RantProgramArgs)

!!! warning
    **This item is deprecated.**
    Use an overload of DoSerial() that accepts a RantProgram instead of a string.

Executes the specified pattern and returns a series of outputs.

```csharp
public System.Collections.Generic.IEnumerable<Rant.RantOutput> DoSerial(string input, RNG rng, int charLimit = 0, double timeout = -1d, RantProgramArgs args = null)
```
**Parameters**

- `input`: The pattern to execute.
- `rng`: The random number generator to use when generating output.
- `charLimit`: The maximum number of characters that can be printed. An exception will be thrown if the limit
            is exceeded. Set to zero or below for unlimited characters.
- `timeout`: The maximum number of seconds that the pattern will execute for.
- `args`: The arguments to pass to the pattern.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### LoadPackage(RantPackage)
Loads the specified package into the engine.

```csharp
public void LoadPackage(RantPackage package)
```
**Parameters**

- `package`: The package to load.
#### LoadPackage(string)
Loads the package at the specified file path into the engine.

```csharp
public void LoadPackage(string path)
```
**Parameters**

- `path`: The path to the package to load.
#### ProgramNameLoaded(string)
Returns a boolean value indicating whether a program by the specified name has been loaded from a package.

```csharp
public bool ProgramNameLoaded(string patternName)
```
**Parameters**

- `patternName`: The name of the program to check.
#### ResetCarrierState()
Deletes all state data in the engine's persisted carrier state, if available.

```csharp
public void ResetCarrierState()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantFormat class (Rant.Formats)
**Namespace:** Rant.Formats

**Inheritance:** Object → RantFormat

Describes language-specific formatting instructions for localizing interpreter output.

```csharp
public sealed class RantFormat
```
### Constructors
#### RantFormat()
Creates a new RantFormat instance with default values.

```csharp
public RantFormat()
```
#### RantFormat(CultureInfo, WritingSystem, IEnumerable<string>, Pluralizer, NumberVerbalizer)
Creates a new RantFormat instance with the specified configuration data.

```csharp
public RantFormat(CultureInfo culture, WritingSystem writingSystem, IEnumerable<string> titleCaseExclusions, Pluralizer pluralizer, NumberVerbalizer numVerbalizer)
```
**Parameters**

- `culture`: The culture to associate with the format.
- `writingSystem`: The writing system to use.
- `titleCaseExclusions`: A collection of words to exclude from title case capitalization.
- `pluralizer`: The pluralizer to use.
- `numVerbalizer`: The number verbalizer to use.
### Properties
#### Culture
The culture to format output strings with.

```csharp
public CultureInfo Culture
{
    get;
}
```
#### NumberVerbalizer
The number verbalizer for the current format.

```csharp
public NumberVerbalizer NumberVerbalizer
{
    get;
}
```
#### Pluralizer
The pluralizer used by the [plural] function to infer plural nouns.

```csharp
public Pluralizer Pluralizer
{
    get;
}
```
#### WritingSystem
The writing system for the current format.

```csharp
public WritingSystem WritingSystem
{
    get;
}
```
### Fields
#### English
English formatting.

```csharp
public static readonly RantFormat English;
```
#### German
German formatting.

```csharp
public static readonly RantFormat German;
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantFunctionParameterType enum (Rant.Core.Framework)
**Namespace:** Rant.Core.Framework

**Inheritance:** Object → ValueType → Enum → RantFunctionParameterType

Defines parameter types for Rant functions.

```csharp
public enum RantFunctionParameterType
```
### Fields
#### Boolean
Parameter is a boolean.

```csharp
public const RantFunctionParameterType Boolean = 6;
```
#### Flags
Parameter uses combinable flags.

```csharp
public const RantFunctionParameterType Flags = 4;
```
#### Mode
Parameter describes a mode, which is one of a specific set of allowed values.

```csharp
public const RantFunctionParameterType Mode = 3;
```
#### Number
Parameter is numeric.

```csharp
public const RantFunctionParameterType Number = 2;
```
#### Pattern
Parameter is a lazily evaluated pattern.

```csharp
public const RantFunctionParameterType Pattern = 1;
```
#### RantObject
Parameter is a RantObject.

```csharp
public const RantFunctionParameterType RantObject = 5;
```
#### String
Parameter is a static string.

```csharp
public const RantFunctionParameterType String = 0;
```
### Methods
#### CompareTo(object)
_No Summary_

```csharp
public override int CompareTo(object target)
```
**Parameters**

- `target`: _No Description_
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetTypeCode()
_No Summary_

```csharp
public override System.TypeCode GetTypeCode()
```
#### HasFlag(Enum)
_No Summary_

```csharp
public override bool HasFlag(Enum flag)
```
**Parameters**

- `flag`: _No Description_
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### ToString(string)
_No Summary_

```csharp
public override string ToString(string format)
```
**Parameters**

- `format`: _No Description_
#### ToString(IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString().

_No Summary_

```csharp
public override string ToString(IFormatProvider provider)
```
**Parameters**

- `provider`: _No Description_
#### ToString(string, IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString(String).

_No Summary_

```csharp
public override string ToString(string format, IFormatProvider provider)
```
**Parameters**

- `format`: _No Description_
- `provider`: _No Description_
## RantInternalException class (Rant)
**Namespace:** Rant

**Inheritance:** Object → Exception → RantInternalException

Represents an error that has been caused by a problem inside the Rant engine. This typically indicates the presence of
            a bug.

```csharp
public sealed class RantInternalException : System.Exception, System.Runtime.Serialization.ISerializable, System.Runtime.InteropServices._Exception
```
### Properties
#### Data
_No Summary_

```csharp
public override IDictionary Data
{
    get;
}
```
#### HelpLink
_No Summary_

```csharp
public override string HelpLink
{
    get;
    set;
}
```
#### HResult
_No Summary_

```csharp
public override int HResult
{
    get;
    protected set;
}
```
#### InnerException
_No Summary_

```csharp
public override Exception InnerException
{
    get;
}
```
#### Message
_No Summary_

```csharp
public override string Message
{
    get;
}
```
#### Source
_No Summary_

```csharp
public override string Source
{
    get;
    set;
}
```
#### StackTrace
_No Summary_

```csharp
public override string StackTrace
{
    get;
}
```
#### TargetSite
_No Summary_

```csharp
public override MethodBase TargetSite
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetBaseException()
_No Summary_

```csharp
public override System.Exception GetBaseException()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetObjectData(SerializationInfo, StreamingContext)
_No Summary_

```csharp
public override void GetObjectData(SerializationInfo info, StreamingContext context)
```
**Parameters**

- `info`: _No Description_
- `context`: _No Description_
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantObject class (Rant.Core.ObjectModel)
**Namespace:** Rant.Core.ObjectModel

**Inheritance:** Object → RantObject

Represents a Rant variable.

```csharp
public sealed class RantObject
```
### Constructors
#### RantObject()
Creates a null object.

```csharp
public RantObject()
```
#### RantObject(object)
Initializes a new instance of the  class from the specified object.

```csharp
public RantObject(object o)
```
**Parameters**

- `o`: The object to store in the  instance.
#### RantObject(RantObjectType)
Creates a new RantObject with the specified object type and a default value.

```csharp
public RantObject(RantObjectType type)
```
**Parameters**

- `type`: The type of object to create.
### Properties
#### Length
Gets the length of the object. For strings, this is the character count. For lists, this is the item count. For all other types, -1 is returned.

```csharp
public int Length
{
    get;
}
```
#### Type
The type of the object.

```csharp
public RantObjectType Type
{
    get;
    set;
}
```
#### Value
The value of the object.

```csharp
public object Value
{
    get;
}
```
### Indexers
#### this[int index]
Gets or sets the object at the specified index in the object.
            Only works with list objects.

```csharp
public RantObject this[int index]
{
    get;
    set;
}
```
### Fields
#### False
False

```csharp
public static readonly RantObject False;
```
#### Null
Null

```csharp
public static readonly RantObject Null;
```
#### True
True

```csharp
public static readonly RantObject True;
```
### Methods
#### Clone()
Returns another RantObject instance with the exact same value as the current instance.

```csharp
public Rant.Core.ObjectModel.RantObject Clone()
```
#### ConvertTo(RantObjectType)
Converts the current object to a RantObject of the specified type and returns it.

```csharp
public Rant.Core.ObjectModel.RantObject ConvertTo(RantObjectType targetType)
```
**Parameters**

- `targetType`: The object type to convert to.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
Returns a string representation of the current RantObject.

```csharp
public virtual string ToString()
```
## RantObjectType enum (Rant.Core.ObjectModel)
**Namespace:** Rant.Core.ObjectModel

**Inheritance:** Object → ValueType → Enum → RantObjectType

Defines object types used by Rant.

```csharp
public enum RantObjectType
```
### Fields
#### Action
Represents a VM action.

```csharp
public const RantObjectType Action = 4;
```
#### Boolean
Represents a boolean value.

```csharp
public const RantObjectType Boolean = 2;
```
#### List
Represents a resizable set of values.

```csharp
public const RantObjectType List = 3;
```
#### Null
Represents a lack of a value.

```csharp
public const RantObjectType Null = 6;
```
#### Number
Represents a decimal number.

```csharp
public const RantObjectType Number = 0;
```
#### String
Represents a series of Unicode characters.

```csharp
public const RantObjectType String = 1;
```
#### Subroutine
Represents a subroutine.

```csharp
public const RantObjectType Subroutine = 5;
```
#### Undefined
Represents a lack of any variable at all.

```csharp
public const RantObjectType Undefined = 7;
```
### Methods
#### CompareTo(object)
_No Summary_

```csharp
public override int CompareTo(object target)
```
**Parameters**

- `target`: _No Description_
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetTypeCode()
_No Summary_

```csharp
public override System.TypeCode GetTypeCode()
```
#### HasFlag(Enum)
_No Summary_

```csharp
public override bool HasFlag(Enum flag)
```
**Parameters**

- `flag`: _No Description_
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### ToString(string)
_No Summary_

```csharp
public override string ToString(string format)
```
**Parameters**

- `format`: _No Description_
#### ToString(IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString().

_No Summary_

```csharp
public override string ToString(IFormatProvider provider)
```
**Parameters**

- `provider`: _No Description_
#### ToString(string, IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString(String).

_No Summary_

```csharp
public override string ToString(string format, IFormatProvider provider)
```
**Parameters**

- `format`: _No Description_
- `provider`: _No Description_
## RantOutput class (Rant)
**Namespace:** Rant

**Inheritance:** Object → RantOutput

Represents a collection of strings generated by a pattern.

```csharp
public sealed class RantOutput : System.Collections.Generic.IEnumerable<Rant.RantOutputEntry>, System.Collections.IEnumerable
```
### Properties
#### BaseGeneration
The generation at which the RNG was initially set before the pattern was run.

```csharp
public long BaseGeneration
{
    get;
}
```
#### Main
The main output string.

```csharp
public string Main
{
    get;
}
```
#### Seed
The seed used to generate the output.

```csharp
public long Seed
{
    get;
}
```
### Indexers
#### this[string channel]
Gets the output of the channel with the specified name.

```csharp
public string this[string channel]
{
    get;
}
```
#### this[params string[] channels]
Gets an array containing the values of the specified channels, in the order they appear.

```csharp
public string[] this[params string[] channels]
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetEnumerator()
Returns an enumerator that iterates through the outputs in the collection.

```csharp
public virtual System.Collections.Generic.IEnumerator<Rant.RantOutputEntry> GetEnumerator()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
Returns the output from the "main" channel.

```csharp
public virtual string ToString()
```
## RantOutputEntry class (Rant)
**Namespace:** Rant

**Inheritance:** Object → RantOutputEntry

Represents the output of a single channel.

```csharp
public sealed class RantOutputEntry
```
### Properties
#### Name
Gets the name of the channel.

```csharp
public string Name
{
    get;
}
```
#### Value
Gets the value of the channel.

```csharp
public string Value
{
    get;
}
```
#### Visiblity
The visibility of the channel that created the output entry.

```csharp
public ChannelVisibility Visiblity
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantPackage class (Rant.Resources)
**Namespace:** Rant.Resources

**Inheritance:** Object → RantPackage

Represents a collection of patterns and tables that can be exported to an archive file.

```csharp
public sealed class RantPackage
```
### Constructors
#### RantPackage()
_No Summary_

```csharp
public RantPackage()
```
### Properties
#### Authors
The authors of the package.

```csharp
public string[] Authors
{
    get;
    set;
}
```
#### Description
The description for the package.

```csharp
public string Description
{
    get;
    set;
}
```
#### ID
The ID of the package.

```csharp
public string ID
{
    get;
    set;
}
```
#### Tags
The tags associated with the package.

```csharp
public string[] Tags
{
    get;
    set;
}
```
#### Title
The display name of the package.

```csharp
public string Title
{
    get;
    set;
}
```
#### Version
The package version.

```csharp
public RantPackageVersion Version
{
    get;
    set;
}
```
### Methods
#### AddDependency(RantPackageDependency)
Adds the specified dependency to the package.

```csharp
public void AddDependency(RantPackageDependency dependency)
```
**Parameters**

- `dependency`: The dependency to add.
#### AddDependency(string, string)
Adds the specified dependency to the package.

```csharp
public void AddDependency(string id, string version)
```
**Parameters**

- `id`: The ID of the package.
- `version`: The package version to target.
#### AddResource(RantResource)
Adds the specified resource to the package.

```csharp
public bool AddResource(RantResource resource)
```
**Parameters**

- `resource`: The resource to add.
#### ClearDependencies()
Removes all dependencies from the package.

```csharp
public void ClearDependencies()
```
#### ContainsResource(RantResource)
Determines whether the package contains the specified resource.

```csharp
public bool ContainsResource(RantResource resource)
```
**Parameters**

- `resource`: The resource to search for.
#### DependsOn(RantPackageDependency)
Determines whether the package has the specified dependency.

```csharp
public bool DependsOn(RantPackageDependency dependency)
```
**Parameters**

- `dependency`: The dependency to check for.
#### DependsOn(string, string)
Determines whether the package depends on the specified package.

```csharp
public bool DependsOn(string id, string version)
```
**Parameters**

- `id`: The ID of the package to check for.
- `version`: The version of the package to check for.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetDependencies()
Enumerates the package's dependencies.

```csharp
public System.Collections.Generic.IEnumerable<Rant.Resources.RantPackageDependency> GetDependencies()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetResources()
Enumerates all resources in the package.

```csharp
public System.Collections.Generic.IEnumerable<Rant.Resources.RantResource> GetResources()
```
#### GetResources\<TResource\>()
Enumerates all resources in the package.

```csharp
public System.Collections.Generic.IEnumerable<TResource> GetResources<TResource>()
```
**Type Parameters**

- `TResource`: (No Description)
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### Load(string)
Loads a package from the specified path and returns it as a RantPackage object.

```csharp
public static Rant.Resources.RantPackage Load(string path)
```
**Parameters**

- `path`: The path to the package file to load.
#### Load(Stream)
Loads a package from the specified stream and returns it as a RantPackage object.

```csharp
public static Rant.Resources.RantPackage Load(Stream source)
```
**Parameters**

- `source`: The stream to load the package data from.
#### RemoveDependency(RantPackageDependency)
Removes the specified dependency from the package.

```csharp
public bool RemoveDependency(RantPackageDependency dependency)
```
**Parameters**

- `dependency`: The dependency to remove.
#### RemoveDependency(string, string)
Removes the specified dependency from the package.

```csharp
public bool RemoveDependency(string id, string version)
```
**Parameters**

- `id`: The ID of the dependency to remove.
- `version`: The version of the dependency to remove.
#### RemoveResource(RantResource)
Removes the specified resource from the package.

```csharp
public bool RemoveResource(RantResource resource)
```
**Parameters**

- `resource`: The resource to remove.
#### Save(string, bool)
Saves the package to the specified file path.

```csharp
public void Save(string path, bool compress = True)
```
**Parameters**

- `path`: The path to the file to create.
- `compress`: Specifies whether to compress the package contents.
#### ToString()
Returns a string containing the title and version of the package.

```csharp
public virtual string ToString()
```
## RantPackageDependency class (Rant.Resources)
**Namespace:** Rant.Resources

**Inheritance:** Object → RantPackageDependency

Represents a dependency for a Rant package.

```csharp
public sealed class RantPackageDependency
```
### Constructors
#### RantPackageDependency(string, string)
Initializes a new RantPackageDependency object.

```csharp
public RantPackageDependency(string id, string version)
```
**Parameters**

- `id`: The ID of the package.
- `version`: The targeted version of the package.
#### RantPackageDependency(string, RantPackageVersion)
Initializes a new RantPackageDependency object.

```csharp
public RantPackageDependency(string id, RantPackageVersion version)
```
**Parameters**

- `id`: The ID of the package.
- `version`: The targeted version of the package.
### Properties
#### AllowNewer
Specifies whether the dependency will accept a package newer than the one given.

```csharp
public bool AllowNewer
{
    get;
    set;
}
```
#### ID
The ID of the package.

```csharp
public string ID
{
    get;
    set;
}
```
#### Version
The targeted version of the package.

```csharp
public RantPackageVersion Version
{
    get;
    set;
}
```
### Methods
#### CheckVersion(RantPackageVersion)
Checks if the specified version is compatible with the current dependency.

```csharp
public bool CheckVersion(RantPackageVersion version)
```
**Parameters**

- `version`: The version to check.
#### Create(RantPackage)
Creates a dependency for the specified package.

```csharp
public static Rant.Resources.RantPackageDependency Create(RantPackage package)
```
**Parameters**

- `package`: The package to create the dependency for.
#### Equals(object)
Determines whether the current RantPackageDependency is shares an ID with the specified object.

```csharp
public virtual bool Equals(object obj)
```
**Parameters**

- `obj`: The object to compare to.
#### GetHashCode()
Gets the hash code for the instance.

```csharp
public virtual int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
Returns a string representation of the current dependency.

```csharp
public virtual string ToString()
```
## RantPackageVersion class (Rant.Resources)
**Namespace:** Rant.Resources

**Inheritance:** Object → RantPackageVersion

Represents a version number for a Rant package.

```csharp
public sealed class RantPackageVersion
```
### Constructors
#### RantPackageVersion(int, int, int)
Initializes a new RantPackageVersion instance with the specified values.

```csharp
public RantPackageVersion(int major, int minor, int revision)
```
**Parameters**

- `major`: The major version.
- `minor`: The minor version.
- `revision`: The revision number.
#### RantPackageVersion()
Initializes a new RantPackageVersion instance with all values set to zero.

```csharp
public RantPackageVersion()
```
### Properties
#### Major
The major version.

```csharp
public int Major
{
    get;
    set;
}
```
#### Minor
The minor version.

```csharp
public int Minor
{
    get;
    set;
}
```
#### Revision
The revision number.

```csharp
public int Revision
{
    get;
    set;
}
```
### Methods
#### Equals(object)
Determines whether the current version is equal to the specified object.

```csharp
public virtual bool Equals(object obj)
```
**Parameters**

- `obj`: The object to compare to.
#### GetHashCode()
_No Summary_

```csharp
public virtual int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### Parse(string)
Attempts to parse a version string and returns the equivalent RantPackageVersion.

```csharp
public static Rant.Resources.RantPackageVersion Parse(string version)
```
**Parameters**

- `version`: The version string to parse.
#### ToString()
Returns a string representation of the current version.

```csharp
public virtual string ToString()
```
#### TryParse(string, out RantPackageVersion&)
_No Summary_

```csharp
public static bool TryParse(string version, out RantPackageVersion& result)
```
**Parameters**

- `version`: _No Description_
- `result`: _No Description_
## RantProgram class (Rant)
**Namespace:** Rant

**Inheritance:** Object → RantResource → RantProgram

Represents a compiled pattern that can be executed by the engine. It is recommended to use this class when running the
            same pattern multiple times.

```csharp
public sealed class RantProgram : Rant.Resources.RantResource
```
### Properties
#### Code
The pattern from which the program was compiled.

```csharp
public string Code
{
    get;
}
```
#### Name
Gets or sets the name of the source code.

```csharp
public string Name
{
    get;
    set;
}
```
#### Type
Describes the origin of the program.

```csharp
public RantProgramOrigin Type
{
    get;
}
```
### Methods
#### CompileFile(string)
Loads the file located at the specified path and compiles a program from its contents.

```csharp
public static Rant.RantProgram CompileFile(string path)
```
**Parameters**

- `path`: The path to the file to load.
#### CompileString(string)
Compiles a program from the specified pattern.

```csharp
public static Rant.RantProgram CompileString(string code)
```
**Parameters**

- `code`: The pattern to compile.
#### CompileString(string, string)
Compiles a program from a pattern with the specified name.

```csharp
public static Rant.RantProgram CompileString(string name, string code)
```
**Parameters**

- `name`: The name to give the source.
- `code`: The pattern to compile.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### LoadFile(string)
Loads a compiled Rant program from the file at the specified path.

```csharp
public static Rant.RantProgram LoadFile(string path)
```
**Parameters**

- `path`: The path to load the program from.
#### LoadStream(string, Stream)
Loads a compiled Rant program from the specified stream.

```csharp
public static Rant.RantProgram LoadStream(string programName, Stream stream)
```
**Parameters**

- `programName`: The name to give to the program.
- `stream`: The stream to load the program from.
#### SaveToFile(string)
Saves the compiled program to the file at the specified path.

```csharp
public void SaveToFile(string path)
```
**Parameters**

- `path`: The path to save the program to.
#### SaveToStream(Stream)
Saves the compiled program to the specified stream.

```csharp
public void SaveToStream(Stream stream)
```
**Parameters**

- `stream`: The stream to save the program to.
#### ToString()
Returns a string describing the pattern.

```csharp
public virtual string ToString()
```
## RantProgramArgs class (Rant)
**Namespace:** Rant

**Inheritance:** Object → RantProgramArgs

Represents a set of arguments that can be passed to a pattern.

```csharp
public sealed class RantProgramArgs
```
### Constructors
#### RantProgramArgs()
Create a new, empty RantPatternArgs instance.

```csharp
public RantProgramArgs()
```
### Indexers
#### this[string key]
Gets or sets an argument of the specified name.

```csharp
public string this[string key]
{
    get;
    set;
}
```
### Methods
#### Clear()
Clears all values.

```csharp
public void Clear()
```
#### Contains(string)
Determines whether an argument by the specified name exists in the current list.

```csharp
public bool Contains(string key)
```
**Parameters**

- `key`: The name of the argument to search for.
#### CreateFrom(object)
Creates a RantPatternArgs instance from the specified object.
            Works with anonymous types!

```csharp
public static Rant.RantProgramArgs CreateFrom(object value)
```
**Parameters**

- `value`: The object to create an argument set from.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### Remove(string)
Removes the specified argument.

```csharp
public bool Remove(string key)
```
**Parameters**

- `key`: The name of the argument to remove.
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantProgramOrigin enum (Rant)
**Namespace:** Rant

**Inheritance:** Object → ValueType → Enum → RantProgramOrigin

Indicates the manner in which a referenced code source was created.

```csharp
public enum RantProgramOrigin
```
### Fields
#### File
Source was loaded from a file.

```csharp
public const RantProgramOrigin File = 0;
```
#### String
Source was loaded from a string.

```csharp
public const RantProgramOrigin String = 1;
```
### Methods
#### CompareTo(object)
_No Summary_

```csharp
public override int CompareTo(object target)
```
**Parameters**

- `target`: _No Description_
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetTypeCode()
_No Summary_

```csharp
public override System.TypeCode GetTypeCode()
```
#### HasFlag(Enum)
_No Summary_

```csharp
public override bool HasFlag(Enum flag)
```
**Parameters**

- `flag`: _No Description_
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
#### ToString(string)
_No Summary_

```csharp
public override string ToString(string format)
```
**Parameters**

- `format`: _No Description_
#### ToString(IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString().

_No Summary_

```csharp
public override string ToString(IFormatProvider provider)
```
**Parameters**

- `provider`: _No Description_
#### ToString(string, IFormatProvider)

!!! warning
    **This item is deprecated.**
    The provider argument is not used. Please use ToString(String).

_No Summary_

```csharp
public override string ToString(string format, IFormatProvider provider)
```
**Parameters**

- `format`: _No Description_
- `provider`: _No Description_
## RantResource class (Rant.Resources)
**Namespace:** Rant.Resources

**Inheritance:** Object → RantResource

The base class for Rant resources that can be included in a package.

```csharp
public abstract class RantResource
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### Finalize()
_No Summary_

```csharp
protected override void Finalize()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### MemberwiseClone()
_No Summary_

```csharp
protected override object MemberwiseClone()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantRuntimeException class (Rant)
**Namespace:** Rant

**Inheritance:** Object → Exception → RantRuntimeException

Represents a runtime error raised by the Rant engine.

```csharp
public sealed class RantRuntimeException : System.Exception, System.Runtime.Serialization.ISerializable, System.Runtime.InteropServices._Exception
```
### Properties
#### Code
The source of the error.

```csharp
public string Code
{
    get;
}
```
#### Column
The column on which the error occurred.

```csharp
public int Column
{
    get;
}
```
#### Data
_No Summary_

```csharp
public override IDictionary Data
{
    get;
}
```
#### HelpLink
_No Summary_

```csharp
public override string HelpLink
{
    get;
    set;
}
```
#### HResult
_No Summary_

```csharp
public override int HResult
{
    get;
    protected set;
}
```
#### Index
The character index on which the error occurred.

```csharp
public int Index
{
    get;
}
```
#### InnerException
_No Summary_

```csharp
public override Exception InnerException
{
    get;
}
```
#### Line
The line on which the error occurred.

```csharp
public int Line
{
    get;
}
```
#### Message
_No Summary_

```csharp
public override string Message
{
    get;
}
```
#### RantStackTrace
The stack trace from the pattern.

```csharp
public string RantStackTrace
{
    get;
}
```
#### Source
_No Summary_

```csharp
public override string Source
{
    get;
    set;
}
```
#### StackTrace
_No Summary_

```csharp
public override string StackTrace
{
    get;
}
```
#### TargetSite
_No Summary_

```csharp
public override MethodBase TargetSite
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetBaseException()
_No Summary_

```csharp
public override System.Exception GetBaseException()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetObjectData(SerializationInfo, StreamingContext)
_No Summary_

```csharp
public override void GetObjectData(SerializationInfo info, StreamingContext context)
```
**Parameters**

- `info`: _No Description_
- `context`: _No Description_
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
Returns a string representation of the runtime error, including the message and stack trace.

```csharp
public virtual string ToString()
```
## RantTableLoadException class (Rant.Vocabulary)
**Namespace:** Rant.Vocabulary

**Inheritance:** Object → Exception → RantTableLoadException

Thrown when Rant encounters an error while loading a dictionary table.

```csharp
public sealed class RantTableLoadException : System.Exception, System.Runtime.Serialization.ISerializable, System.Runtime.InteropServices._Exception
```
### Properties
#### Column
Gets the column on which the error occurred.

```csharp
public int Column
{
    get;
}
```
#### Data
_No Summary_

```csharp
public override IDictionary Data
{
    get;
}
```
#### HelpLink
_No Summary_

```csharp
public override string HelpLink
{
    get;
    set;
}
```
#### HResult
_No Summary_

```csharp
public override int HResult
{
    get;
    protected set;
}
```
#### InnerException
_No Summary_

```csharp
public override Exception InnerException
{
    get;
}
```
#### Line
Gets the line number on which the error occurred.

```csharp
public int Line
{
    get;
}
```
#### Message
_No Summary_

```csharp
public override string Message
{
    get;
}
```
#### Origin
Gets a string describing where the table was loaded from. For tables loaded from disk, this will be the file path.

```csharp
public string Origin
{
    get;
}
```
#### Source
_No Summary_

```csharp
public override string Source
{
    get;
    set;
}
```
#### StackTrace
_No Summary_

```csharp
public override string StackTrace
{
    get;
}
```
#### TargetSite
_No Summary_

```csharp
public override MethodBase TargetSite
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetBaseException()
_No Summary_

```csharp
public override System.Exception GetBaseException()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetObjectData(SerializationInfo, StreamingContext)
_No Summary_

```csharp
public override void GetObjectData(SerializationInfo info, StreamingContext context)
```
**Parameters**

- `info`: _No Description_
- `context`: _No Description_
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RantUtils class (Rant)
**Namespace:** Rant

**Inheritance:** Object → RantUtils

Contains miscellaneous utility methods that provide information about the Rant engine.

```csharp
public static class RantUtils
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### FunctionExists(string)
Determines whether a function with the specified name is defined in the current engine version.

```csharp
public static bool FunctionExists(string functionName)
```
**Parameters**

- `functionName`: The name of the function to search for. Argument is not case-sensitive.
#### GetFunction(string)
Returns the function with the specified name. The return value will be null if the function is not found.

```csharp
public static Rant.Metadata.IRantFunctionGroup GetFunction(string functionName)
```
**Parameters**

- `functionName`: The name of the function to retrieve.
#### GetFunctionAliases(string)
Enumerates the aliases assigned to the specified function name.

```csharp
public static System.Collections.Generic.IEnumerable<string> GetFunctionAliases(string functionName)
```
**Parameters**

- `functionName`: The function name to retrieve aliases for.
#### GetFunctionDescription(string, int)
Returns the description for the function with the specified name.

```csharp
public static string GetFunctionDescription(string functionName, int argc)
```
**Parameters**

- `functionName`: The name of the function to get the description for.
- `argc`: The number of arguments in the overload to get the description for.
#### GetFunctionNames()
Enumerates the names of all available Rant functions.

```csharp
public static System.Collections.Generic.IEnumerable<string> GetFunctionNames()
```
#### GetFunctionNamesAndAliases()
Enumerates all function names and their aliases.

```csharp
public static System.Collections.Generic.IEnumerable<string> GetFunctionNamesAndAliases()
```
#### GetFunctions()
Enumerates the available functions.

```csharp
public static System.Collections.Generic.IEnumerable<Rant.Metadata.IRantFunctionGroup> GetFunctions()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## RNG class (Rant)
**Namespace:** Rant

**Inheritance:** Object → RNG

Represents a non-linear random number generator.

```csharp
public class RNG
```
### Constructors
#### RNG(long)
Creates a new RNG instance with the specified seed.

```csharp
public RNG(long seed)
```
**Parameters**

- `seed`: The seed for the generator.
#### RNG(long, long)
Creates a new RNG instance with the specified seed and generation.

```csharp
public RNG(long seed, long generation)
```
**Parameters**

- `seed`: The seed for the generator.
- `generation`: The generation to start at.
#### RNG()
Creates a new RNG instance seeded with the system tick count.

```csharp
public RNG()
```
### Properties
#### BaseSeed
The root seed.

```csharp
public long BaseSeed
{
    get;
    set;
}
```
#### Depth
The current branching depth of the generator.

```csharp
public int Depth
{
    get;
}
```
#### Generation
The current generation.

```csharp
public long Generation
{
    get;
    set;
}
```
#### Seed
The seed of the top branch.

```csharp
public long Seed
{
    get;
    set;
}
```
### Indexers
#### this[int g]
Calculates the raw 64-bit value for a given generation.

```csharp
public long this[int g]
{
    get;
}
```
### Methods
#### Branch(long)
Creates a new branch based off the current seed and the specified seed.

```csharp
public Rant.RNG Branch(long seed)
```
**Parameters**

- `seed`: The seed to create the branch with.
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### Finalize()
_No Summary_

```csharp
protected override void Finalize()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetRaw(long, long)
Calculates the raw 64-bit value for a given seed/generation pair.

```csharp
public static long GetRaw(long s, long g)
```
**Parameters**

- `s`: The seed.
- `g`: The generation.
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### MemberwiseClone()
_No Summary_

```csharp
protected override object MemberwiseClone()
```
#### Merge()
Removes the topmost branch and resumes generation on the next one down.

```csharp
public Rant.RNG Merge()
```
#### Next()
Calculates a 32-bit, non-negative integer from the next generation and increases the current generation by 1.

```csharp
public int Next()
```
#### Next(int)
Calculates a 32-bit integer between 0 and a specified upper bound for the current generation and increases the current
            generation by 1.

```csharp
public int Next(int max)
```
**Parameters**

- `max`: The exclusive maximum value.
#### Next(int, int)
Calculates a 32-bit integer between the specified minimum and maximum values for the current generation, and increases
            the current generation by 1.

```csharp
public int Next(int min, int max)
```
**Parameters**

- `min`: The inclusive minimum value.
- `max`: The exclusive maximum value.
#### NextBoolean()
Returns a random boolean value and advances the generation by 1.

```csharp
public bool NextBoolean()
```
#### NextDouble()
Returns a double-precision floating point number between 0 and 1, and advances the generation by 1.

```csharp
public double NextDouble()
```
#### NextDouble(double)
Returns a double-precision floating point number between 0 and the specified maximum value, and advances the generation
            by 1.

```csharp
public double NextDouble(double max)
```
**Parameters**

- `max`: (No Description)
#### NextDouble(double, double)
Returns a double-precision floating point number between the specified minimum and maximum values, and advances the
            generation by 1.

```csharp
public double NextDouble(double min, double max)
```
**Parameters**

- `min`: (No Description)
- `max`: (No Description)
#### NextRaw()
Calculates the raw 64-bit value for the next generation, and increases the current generation by 1.

```csharp
public long NextRaw()
```
#### Peek()
Calculates a 32-bit, non-negative integer for the current generation.

```csharp
public int Peek()
```
#### Peek(int)
Calculates a 32-bit integer between 0 and a specified upper bound for the current generation.

```csharp
public int Peek(int max)
```
**Parameters**

- `max`: The exclusive maximum value.
#### Peek(int, int)
Calculates a 32-bit integer between the specified minimum and maximum values for the current generation.

```csharp
public int Peek(int min, int max)
```
**Parameters**

- `min`: The inclusive minimum value.
- `max`: The exclusive maximum value.
#### PeekAt(long)
Calculates the 32-bitnon-negative integer for the specified generation.

```csharp
public int PeekAt(long generation)
```
**Parameters**

- `generation`: The generation to peek at.
#### PeekAt(long, int)
Calculates a 32-bit integer between 0 and a specified upper bound for the specified generation.

```csharp
public int PeekAt(long generation, int max)
```
**Parameters**

- `generation`: The generation whose value to calculate.
- `max`: The exclusive maximum value.
#### PeekAt(int, int, int)
Calculates a 32-bit integer between the specified minimum and maximum values for the specified generation.

```csharp
public int PeekAt(int generation, int min, int max)
```
**Parameters**

- `generation`: The generation whose value to calculate.
- `min`: The inclusive minimum value.
- `max`: The exclusive maximum value.
#### Prev()
Calculates a 32-bit, non-negative integer from the previous generation and decreases the current generation by 1.

```csharp
public int Prev()
```
#### Prev(int)
Calculates a 32-bit integer between 0 and a specified upper bound from the previous generation and decreases the
            current generation by 1.

```csharp
public int Prev(int max)
```
**Parameters**

- `max`: The exclusive maximum value.
#### Prev(int, int)
Calculates a 32-bit integer between the specified minimum and maximum values for the previous generation, and decreases
            the current generation by 1.

```csharp
public int Prev(int min, int max)
```
**Parameters**

- `min`: The inclusive minimum value.
- `max`: The exclusive maximum value.
#### PrevRaw()
Calculates the raw 64-bit value for the previous generation, and decreases the current generation by 1.

```csharp
public long PrevRaw()
```
#### Reset()
Sets the current generation to zero.

```csharp
public void Reset()
```
#### Reset(long)
Sets the seed to the specified value and the current generation to zero.

```csharp
public void Reset(long newSeed)
```
**Parameters**

- `newSeed`: The new seed to apply to the generator.
#### ToString()
_No Summary_

```csharp
public override string ToString()
```
## WritingSystem class (Rant.Formats)
**Namespace:** Rant.Formats

**Inheritance:** Object → WritingSystem

Represents configuration settings for a language's writing system.

```csharp
public sealed class WritingSystem
```
### Constructors
#### WritingSystem(IEnumerable<char>, string, QuotationMarks)
Creates a new writing system with the specified configuration.

```csharp
public WritingSystem(IEnumerable<char> alphabet, string space, QuotationMarks quotations)
```
**Parameters**

- `alphabet`: The alphabet to use.
- `space`: The standard space to use.
- `quotations`: The quotation marks to use.
#### WritingSystem()
Creates a new writing system with the default configuration.

```csharp
public WritingSystem()
```
### Properties
#### Quotations
The quotation marks used by the format.

```csharp
public QuotationMarks Quotations
{
    get;
}
```
#### Space
The standard space used by series and phrasals.

```csharp
public string Space
{
    get;
}
```
### Methods
#### Equals(object)
_No Summary_

```csharp
public override bool Equals(object obj)
```
**Parameters**

- `obj`: _No Description_
#### GetAlphabet()
The alphabet used by the format.

```csharp
public System.Collections.Generic.IEnumerable<char> GetAlphabet()
```
#### GetHashCode()
_No Summary_

```csharp
public override int GetHashCode()
```
#### GetType()
_No Summary_

```csharp
public override System.Type GetType()
```
#### ToString()
_No Summary_

```csharp
public override string ToString()
```

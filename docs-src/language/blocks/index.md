**Blocks** are sections of a pattern, consisting of one or more sequential parts,
which are subject to branching behavior. Blocks are surrounded with braces.

## Basic usage

Blocks may occupy one or more lines. This will not affect their output, and only serves
as a means of formatting your code.

```rant
{ block }

{
  multiline block
}
```

Separate branches of a block are delimited by the pipe character:

```rant
{ A | B | C | D | E }
```

Blocks can also be nested:

```rant
{
  { 1A | 1B | 1C }
  |
  { 2A | 2B | 2C | 2D }
  |
  { 3A | 3B | 3C | 3D | 3E }
}
```

The default behavior of a block is to randomly execute one of the items. However, this behavior
can be changed using [synchronizers](synchronizers.md).

## Weighting elements

Elements can be weighted to adjust the probability of them being chosen.
Weights will only be used when no synchronizer is applied.

A weight may be applied by inserting the weight value in parentheses at the start of the element.
The weight value may be a constant or a pattern; all weight-patterns are interpreted once when the block starts.
The value must be a normalized decimal number greater than or equal to zero, where 0 = 0% and 1 = 100%.
Any element with no weight value will default to 1.

```rant
{
    (weight-a) element-a
    |
    (weight-b) element-b
    |
    ...
}
```

!!! tip
    Weighted blocks with large amounts of elements may cause significant slowdowns, 
    especially if they are used repeatedly. Because of this, it is recommended only to
    use weights where they are absolutely necessary.
    
    If a block uses only small integer weight values, you can improve its performance by
    simply repeating elements rather than using weights.
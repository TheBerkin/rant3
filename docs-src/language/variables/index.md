Rant has a variable system that can create, store, and manipulate numbers, strings, callbacks, booleans, and lists.

## Scopes

The most important thing to know about variables in Rant is that they are defined within a scope.
Blocks in Rant are treated like variable scopes, and once you exit a block you've created a variable in,
that variable no longer exists (yes, even if your block is a repeater; it will have to create the variable again).

Variables created in the global scope of any pattern will persist between patterns. Likewise, if you set
a global variable through the RantEngine class, it will be available in any scope inside any pattern.

## Creating variables

Each variable type has a different function for creating it. Below is a table of these functions:

|Function name|Variable type|
|-------------|-------------|
|[`[vn]`](/language/functions#vn)|Number|
|[`[vs]`](/language/functions#vs)|String|
|[`[vb]`](/language/functions#vb)|Boolean|
|[`[vl]`](/language/functions#vl)|List|
|[`[vp]`](/language/functions#vp)|Callback|

Each of these functions requires at least two arguments: the variable's name, and its initial value.

```rant
[vn:a;14][vn:b;7]
[add:[v:a];[v:b]]
# Output: 21
```

## Value access and operations

Most variable value access is done via the [`[v]`](/language/functions#v) function.
This function simply prints the string representation of the variable's value.
All numeric and boolean operations in Rant are string-based.

At first glance, this may seem inefficient; why would someone want to pass numbers as
strings? However, if we look at more advanced examples, this behavior can be quite useful
for generating, for example, numbers with specific digit patterns.

```rant
# Generate a 5-digit number without any zeros, e.g. 25647
[vn:nozeros;\D,5]
```

```rant
# Generate a number with only 2, 3, and 4
# Also don't repeat the same digit twice in a row
[vn:num234;[r:[n:1;8]][x:x234;no-repeat]{2|3|4}]
```
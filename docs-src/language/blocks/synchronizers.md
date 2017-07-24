A **synchronizer** is a type of construct used to govern how a block selects elements.
There are many types of synchronizers, each with their own selection technique.

## Creating a synchronizer

Synchronizers are created using the [`[sync]`](functions/#sync) function. It also has a short name, `[x]`.

A simple example of a synchronizer is one which syncs the selection of two different blocks.

```rant
[x:foo;locked]{Foo|Bar}\s
[x:foo;locked]{Foo|Bar}
```
Even though the two blocks above are separate, the `[sync]` function forces them to choose the same element number;
because of this, the pattern has only two possible outputs:
```
Foo, Foo
```

```
Bar, Bar
```

## Types

### `none`

Selects an independently random element.
This is the default behavior of blocks.

### `forward`

The block will execute elements in left-to-right order, beginning with the first element.
Once the last element is used, it starts at the first one again.

### `reverse`

The block will execute elements in right-to-left order, beginning with the last element.
Once the first element is used, it starts at the last one again.

### `deck`

Elements are shuffled randomly and executed in the shuffled order. Once all elements are used,
they are shuffled again.

### `cdeck`

Elements are shuffled randomly and executed in the shuffled order. Elements are never reshuffled.

Also known as "cyclic deck".

### `locked`

An element number is chosen at random and the block will only select that one.

### `ping`

Starting at the first item, iterates through all elements in order and then reverses without repeating boundary elements.

### `pong`

Starting at the last item, iterates through all elements backwards and then reverses without repeating boundary elements.

### `no-repeat`

The same item is never chosen twice in a row, as long as the block has two or more items.
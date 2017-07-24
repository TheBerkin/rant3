Block attributes are special pattern-local properties that can be set to change the behavior of the next block.
These can be set through a collection of functions. Once a block uses block attributes you have set, all the
block attributes are reset to their default state.

## Types

Below is a table explaining each block attribute:

|Attribute|Function|Description|
|---------|--------|-----------|
|Repetitions|[`[rep]`](/language/functions#rep), [`[repeach]`](/language/functions#repeach)|The number of times the block should execute.|
|Separator|[`[sep]`](/language/functions#sep)|The separator pattern to run between repetitions.|
|Synchronizer|[`[sync]`](/language/functions#sync)|The [synchronizer](synchronizers) to apply to the block.|
|Chance|[`[chance]`](/language/functions#chance)|The probability, in percent, that the block will be run.|
|Start|[`[start]`](/language/functions#start)|The pattern to run before the block.|
|End|[`[end]`](/language/functions#end)|The pattern to run after the block.|
|Prefix|[`[before]`](/language/functions#before)|The pattern to run before each iteration.|
|Postfix|[`[after]`](/language/functions#after)|The pattern to run after each iteration.|
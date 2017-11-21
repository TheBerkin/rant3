Block state functions are a class of functions designed to access state information about the most recently-executed block.
These functions can access information such as the current iteration number, the separator pattern, and the total number of
repetitions the block will perform. Some can also perform certain actions depending on the current state of the block.

## List of functions

|Function|Description|
|--------|-----------|
|[`[repnum]`](/language/functions#repnum)|Current iteration number.<br/>**Sequence:** [1 ... n]|
|[`[repelapsed]`](/language/functions#repelapsed)|Number of finished iterations.<br/>**Sequence:** [0 ... n - 1]|
|[`[reprem]`](/language/functions#reprem)|Number of iterations remaining after current one.<br/>**Sequence:** [n - 1 ... 0]|
|[`[repqueued]`](/language/functions#repqueued)|Number of iterations not completed yet.<br/>**Sequence:** [n ... 1]|
|[`[repcount]`](/language/functions#repcount)|Repetition count.|
|[`[depth]`](/language/functions#depth)|Number of currently active blocks.|
|[`[index]`](/language/functions#index)|Zero-based index of current item.|
|[`[index1]`](/language/functions#index1)|One-based index of current item.|
|[`[even]`](/language/functions#even)|Runs callback if current iteration is even.|
|[`[odd]`](/language/functions#odd)|Runs callback if current iteration is odd.|
|[`[nth]`](/language/functions#nth)|Runs callback if current iteration is a multiple of specified number.|
|[`[notnth]`](/language/functions#notnth)|Runs callback if current iteration is not a multiple of specified number.|
|[`[ntho]`](/language/functions#ntho)|Runs callback if current iteration with offset is a multiple of specified number.|
|[`[notntho]`](/language/functions#notntho)|Runs callback if current iteration with offset is not a multiple of specified number.|
|[`[first]`](/language/functions#first)|Runs callback if current iteration is the first.|
|[`[last]`](/language/functions#last)|Runs callback if current iteration is the last.|
|[`[ends]`](/language/functions#ends)|Runs callback if current iteration is the first or last.|
|[`[middle]`](/language/functions#ends)|Runs callback if current iteration is neither the first nor last.|
|[`[sep]`](/language/functions#sep)|Runs separator pattern.|

> TODO: Needs examples
using Rant.Internals.Engine.Metadata;

namespace Rant.Internals.Engine.Constructs
{
    internal enum SyncType
    {
        [RantDescription("Random ordering.")]
        None,
        [RantDescription("Execute from left to right.")]
        Ordered,
        [RantDescription("Execute from right to left.")]
        Reverse,
        [RantDescription("Shuffle items and execute in order. Re-shuffled each time all items are iterated.")]
        Deck,
        [RantDescription("Shuffle items and execute in order. Same order is used for each traversal.")]
        Cdeck,
        [RantDescription("Same item is selected each time.")]
        Locked
    }
}
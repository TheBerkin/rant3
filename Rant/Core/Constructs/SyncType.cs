using Rant.Metadata;

namespace Rant.Core.Constructs
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
        Locked,
		[RantDescription("Starting from the first item, iterate through all elements in order and then reverse, without repeating the boundary elements.")]
		Ping,
		[RantDescription("Starting from the last item, iterate through all elements backwards and then reverse, without repeating the boundary elements.")]
		Pong
    }
}
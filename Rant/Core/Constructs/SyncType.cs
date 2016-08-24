using Rant.Metadata;

namespace Rant.Core.Constructs
{
	internal enum SyncType
	{
		[RantDescription("A random element is selected each time.")]
		None,
		[RantDescription("Executes from left to right.")]
		Forward,
		[RantDescription("Executes from right to left.")]
		Reverse,
		//[RantDescription("Executes from left to right, but randomly skips elements. This mode can reduce the iteration count when using [repeach].")]
		//ForwardSkip,
		//[RantDescription("Executes from right to left, but randomly skips elements. This mode can reduce the iteration count when using [repeach].")]
		//ReverseSkip,
		[RantDescription("Shuffles items and executes in order. Re-shuffled each time all items are used up.")]
		Deck,
		[RantDescription("Shuffles items and executes in order. The same order is reused for each traversal.")]
		Cdeck,
		[RantDescription("Chosen randomly, the same element is selected each time.")]
		Locked,
		[RantDescription("Starting at the first item, iterates through all elements in order and then reverses without repeating boundary elements.")]
		Ping,
		[RantDescription("Starting at the last item, iterates through all elements backwards and then reverses without repeating boundary elements.")]
		Pong
	}
}
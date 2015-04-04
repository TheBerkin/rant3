namespace Rant.Engine.ObjectModel
{
	internal enum Precedence
	{
		Never = 0,
		Assignment = 1,
		Sum = 2,
		Product = 3,
		Exponent = 4,
		Prefix = 5,
		Postfix = 6
	}
}